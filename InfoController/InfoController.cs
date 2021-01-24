using System;
using System.Collections.Generic;
using NetEti.Globals;

namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Singleton, dispatcht Meldungen unter Berücksichtigung
    /// ihrer Schweregrade.<br></br>
    /// Verwaltet eine Delegate-Liste, in die sich Viewer eintragen
    /// können, die dann bei Eingang einer neuen Message von
    /// InfoController informiert werden.
    /// </summary>
    /// <remarks>
    /// File: InfoController.cs<br></br>
    /// Autor: Erik Nagel, NetEti<br></br>
    ///<br></br>
    /// 08.03.2012 Erik Nagel: erstellt<br></br>
    /// 21.04.2013 Erik Nagel: Verarbeitung über Array-Kopie in informInfoReceivers.<br></br>
    /// 02.05.2014 Erik Nagel: Say implementiert;<br></br>
    ///                        Aufruf von Global.DynamicIs(instance, typeof(T)) geändert in
    ///                        typeof(T).IsAssignableFrom(instance.GetType()).<br></br>
    /// 02.05.2019 Erik Nagel: IDisposable implementiert.
    /// </remarks>
    public class InfoController : IInfoController, IDisposable, IFlushable
    {
        #region public members

        #region IDisposable Member

        private bool _disposed; // = false wird vom System vorbelegt;

        /// <summary>
        /// Öffentliche Methode zum Aufräumen.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Hier kann ggf. aufgeräumt werden.
        /// </summary>
        /// <param name="disposing">False, wenn die Methode vom eigenen Destruktor aufgerufen wurde.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                // eventuelle unmanaged resources hier freigeben
                if (disposing)
                {
                    // hier werden managed ressources freigegeben
                    this.DisposeAll();
                }
                this._disposed = true;
            }
        }

        /// <summary>
        /// Finalizer: wird vom GarbageCollector aufgerufen.
        /// </summary>
        ~InfoController()
        {
            this.Dispose(false);
        }

        #endregion IDisposable Member

        /// <summary>
        /// Returnt die statische Property der nestet Klasse NestedInstance (als IInfoPublisher),
        /// welche ihrerseits den privaten Konstruktor aufruft, wenn noch keine Instanz existiert.
        /// </summary>
        /// <returns>Die (einzige) gültige Instanz von InfoController</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Returns a lazy singleton - a single property doesn't seem to be appropriate")]
        public static IInfoPublisher GetInfoPublisher()
        {
            return (NestedInstance._itsMe);
        }

        /// <summary>
        /// Returnt die statische Property der nestet Klasse NestedInstance (als IInfoSource),
        /// welche ihrerseits den privaten Konstruktor aufruft, wenn noch keine Instanz existiert.
        /// </summary>
        /// <returns>Die (einzige) gültige Instanz von InfoController</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Returns a lazy singleton - a single property doesn't seem to be appropriate")]
        public static IInfoSource GetInfoSource()
        {
            return (NestedInstance._itsMe);
        }

        /// <summary>
        /// Returnt die statische Property der nestet Klasse NestedInstance (als IInfoController),
        /// welche ihrerseits den privaten Konstruktor aufruft, wenn noch keine Instanz existiert.
        /// </summary>
        /// <returns>Die (einzige) gültige Instanz von InfoController</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Returns a lazy singleton - a single property doesn't seem to be appropriate")]
        public static IInfoController GetInfoController()
        {
            return (NestedInstance._itsMe);
        }

        /// <summary>
        /// Hierüber wird 'mal eben' einen Message verbreitet.
        /// </summary>
        /// <param name="msg">Das zu verbreitende (Message-)Objekt</param>
        public static void Say(object msg)
        {
            GetInfoPublisher().Publish(msg);
        }

        /// <summary>
        /// Hierüber wird 'mal eben' einen Message verbreitet.
        /// </summary>
        /// <param name="msg">Das zu verbreitende (Message-)Objekt</param>
        public static void FlushAll()
        {
            GetInfoPublisher().Flush();
        }

        /// <summary>
        /// Hierüber wird einen neue Message verbreitet.
        /// </summary>
        /// <param name="sender">Der Absender der Nachricht</param>
        /// <param name="msg">Das zu verbreitende (Message-)Objekt</param>
        /// <param name="triggerInfoType">Der Message-Typ, z.B.: InfoType.Milestone</param>
        public void Publish(object sender, object msg, InfoType triggerInfoType)
        {
            object messageObject = msg;
            //string timestamp = System.String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:G}", new object[] { System.DateTime.Now });
            string timestamp = System.String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:yyyy.MM.dd HH:mm:ss,ffffff}", new object[] { System.DateTime.Now });
            informInfoReceivers(sender, messageObject, triggerInfoType, timestamp, ThreadInfos.GetThreadInfos());
        }

        /// <summary>
        /// Hierüber wird eine neue Message des Typs InfoType.Info verbreitet.
        /// </summary>
        /// <param name="sender">Der Absender der Nachricht</param>
        /// <param name="msg">Das zu verbreitende (Message-)Objekt</param>
        public void Publish(object sender, object msg)
        {
            this.Publish(sender, msg, InfoType.Info);
        }

        /// <summary>
        /// Hierüber wird eine neue Message des Typs InfoType.Info mit Absender null verbreitet.
        /// </summary>
        /// <param name="msg">Das zu verbreitende (Message-)Objekt</param>
        public void Publish(object msg)
        {
            this.Publish(null, msg, InfoType.Info);
        }

        /// <summary>
        /// Flusht alle flushable Viewer/Logger.
        /// </summary>
        public void Flush()
        {
            IInfoViewer[] viewers;
            lock (InfoController._lockMe)
            {
                // zur weiteren Verarbeitung threadsafe in ein entkoppeltes Array kopieren.
                // In einer Multithreading-Umgebung können den _infoMessageReceivers Viewer hinzugefügt
                // werden, während diese Routine gerade läuft, was bei der direkten Verwendung von
                // this._infoMessageReceivers zu der Exception führen würde, dass die Auflistung
                // während der Verarbeitung geändert wurde.
                // Alternative wäre, die gesamte Routine zu sperren. Das würde aber möglicherweise die
                // gesamte Verarbeitung ausbremsen, weshalb ich hier lieber eine im Extremfall verloren
                // gegangene Message in Kauf nehme.
                viewers = new IInfoViewer[this._infoMessageReceivers.Count];
                this._infoMessageReceivers.Keys.CopyTo(viewers, 0);
            }
            foreach (IInfoViewer viewer in viewers)
            {
                if (viewer is IFlushable)
                {
                    (viewer as IFlushable).Flush();
                }
            }
        }

        /// <summary>
        /// Disposed alle Viewer.
        /// </summary>
        public void DisposeAll()
        {
            IInfoViewer[] viewers;
            lock (InfoController._lockMe)
            {
                // zur weiteren Verarbeitung threadsafe in ein entkoppeltes Array kopieren.
                // In einer Multithreading-Umgebung können den _infoMessageReceivers Viewer hinzugefügt
                // werden, während diese Routine gerade läuft, was bei der direkten Verwendung von
                // this._infoMessageReceivers zu der Exception führen würde, dass die Auflistung
                // während der Verarbeitung geändert wurde.
                // Alternative wäre, die gesamte Routine zu sperren. Das würde aber möglicherweise die
                // gesamte Verarbeitung ausbremsen, weshalb ich hier lieber eine im Extremfall verloren
                // gegangene Message in Kauf nehme.
                viewers = new IInfoViewer[this._infoMessageReceivers.Count];
                this._infoMessageReceivers.Keys.CopyTo(viewers, 0);
            }
            foreach (IInfoViewer viewer in viewers)
            {
                if (viewer is IDisposable)
                {
                    (viewer as IDisposable).Dispose();
                }
            }
        }

        /// <summary>
        /// Hierüber trägt sich ein interessierter Viewer in die Liste der zu
        /// informierenden Viewer ein.
        /// </summary>
        /// <param name="viewer">Der Message-Empfänger</param>
        /// <param name="classType">Der Klassentyp, für den sich msgHandler zuständig erklärt</param>
        /// <param name="triggerInfoTypes">Liste der Message-Typen, für die sich der Viewer interessiert</param>
        public void RegisterInfoReceiver(IInfoViewer viewer, Type classType, InfoType[] triggerInfoTypes)
        {
            lock (InfoController._lockMe)
            {

                if (!this._infoMessageReceivers.ContainsKey(viewer))
                {
                    InfoReceiverProperties props;
                    props.ClassType = classType;
                    props.InfoTypes = triggerInfoTypes;
                    this._infoMessageReceivers.Add(viewer, props);
                }
            }
        }

        /// <summary>
        /// Hierüber trägt sich ein interessierter Viewer in die Liste der zu
        /// informierenden Viewer ein.
        /// </summary>
        /// <param name="viewer">Der Message-Empfänger</param>
        /// <param name="triggerInfoTypes">Liste der Message-Typen, für die sich der Viewer interessiert</param>
        public void RegisterInfoReceiver(IInfoViewer viewer, InfoType[] triggerInfoTypes)
        {
            this.RegisterInfoReceiver(viewer, typeof(string), triggerInfoTypes);
        }

        /// <summary>
        /// Hierüber meldet sich ein eingetragener Viewer wieder ab.
        /// </summary>
        /// <param name="viewer">Der Message-Empfänger</param>
        public void UnregisterInfoReceiver(IInfoViewer viewer)
        {
            lock (InfoController._lockMe)
            {
                if (this._infoMessageReceivers.ContainsKey(viewer))
                {
                    this._infoMessageReceivers.Remove(viewer);
                }
            }
        }

        #endregion public members

        #region private members

        private static object _lockMe = new object(); // nur für Threadlocks

        private struct InfoReceiverProperties
        {
            public Type ClassType;
            public InfoType[] InfoTypes;
        }

        private Dictionary<IInfoViewer, InfoReceiverProperties> _infoMessageReceivers;

        /// <summary>
        /// Privater Konstruktor, wird ggf. von der öffentlichen, statischen
        /// Methode GetInfoController aufgerufen: initialisiert (leert) die Viewer-Liste.
        /// </summary>
        private InfoController()
        {
            this._infoMessageReceivers = new Dictionary<IInfoViewer, InfoReceiverProperties>();
        }

        /// <summary>
        /// Aufruf der Callbacks aller eingetragener Viewer, die sich für die Message vom
        /// aktuellen Typ interessieren (threadsafe).
        /// </summary>
        /// <param name="sender">Der Absender der Nachricht</param>
        /// <param name="messageObject">Das verschickte Objekt</param>
        /// <param name="triggerInfoType">Der Typ der Message</param>
        /// <param name="timestamp">Der Timestamp der Message</param>
        /// <param name="threadInfos">Nummern des aktuellen Threads</param>
        private void informInfoReceivers(object sender, object messageObject, InfoType triggerInfoType, string timestamp, string threadInfos)
        {
            InfoArgs msgArgs = new InfoArgs(messageObject, triggerInfoType, timestamp, threadInfos);
            IInfoViewer[] viewers;
            lock (InfoController._lockMe)
            {
                // zur weiteren Verarbeitung threadsafe in ein entkoppeltes Array kopieren.
                // In einer Multithreading-Umgebung können den _infoMessageReceivers Viewer hinzugefügt
                // werden, während diese Routine gerade läuft, was bei der direkten Verwendung von
                // this._infoMessageReceivers zu der Exception führen würde, dass die Auflistung
                // während der Verarbeitung geändert wurde.
                // Alternative wäre, die gesamte Routine zu sperren. Das würde aber möglicherweise die
                // gesamte Verarbeitung ausbremsen, weshalb ich hier lieber eine im Extremfall verloren
                // gegangene Message in Kauf nehme.
                viewers = new IInfoViewer[this._infoMessageReceivers.Count];
                this._infoMessageReceivers.Keys.CopyTo(viewers, 0);
            }

            foreach (IInfoViewer viewer in viewers)
            {
                EventHandler<InfoArgs> msgHandler = viewer.HandleInfo;
                if (msgHandler != null)
                {
                    InfoReceiverProperties props;
                    if (this._infoMessageReceivers.TryGetValue(viewer, out props))
                    {
                        bool isAppropriate;
                        if (props.ClassType != null)
                        {
                            //isAppropriate = Global.DynamicIs(messageObject, props.ClassType);
                            isAppropriate = props.ClassType.IsAssignableFrom(messageObject.GetType());
                        }
                        else
                        {
                            isAppropriate = true;
                        }
                        if (isAppropriate)
                        {
                            InfoType[] alerts = props.InfoTypes;
                            if (alerts != null)
                            {
                                foreach (InfoType alert in alerts)
                                {
                                    if (triggerInfoType == alert)
                                    {
                                        // 08.03.2012 Nagel: kein catch (kann zu deadlocks führen)
                                        lock (InfoController._lockMe)
                                        {
                                            if (msgHandler != null)
                                            {
                                                msgHandler(sender, msgArgs);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    } // if (this._infoMessageReceivers.TryGetValue(viewer, out props))
                }
            }
        } // private void informInfoReceivers(object messageObject, InfoType triggerInfoType, string timestamp)

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "due to lazyness of the singleton instance")]
        private class NestedInstance
        {
            internal static readonly InfoController _itsMe;
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "due to lazyness of the singleton instance")]
            static NestedInstance()
            {
                _itsMe = new InfoController();
            }
        }

        #endregion private members

    }
}
