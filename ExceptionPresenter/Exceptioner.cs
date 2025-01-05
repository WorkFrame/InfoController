using System.Runtime.Serialization;
using System.Windows;

namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Sellt über HandleInfoForException eine Exception incl. ihrer ggf.
    /// inner Exception und des zugehörigen Call-Stacks in einer MessageBox dar.<br></br>
    /// Nutzt dafür die Klasse ExtendedException, die ebenfalls hier bereitgestellt wird.
    /// </summary>
    /// <remarks>
    /// File: Exceptioner.cs<br></br>
    /// Autor: Erik Nagel, NetEti<br></br>
    ///<br></br>
    /// 08.03.2012 Erik Nagel: erstellt<br></br>
    /// 08.03.2012 Erik Nagel: HandleInfoForException berücksichtigt jetzt bei ExtendedException
    ///                        die InnerException, die die eigentliche Exception ist;<br></br>
    ///                        HandleInfoForException ist jetzt nicht mehr statisch;<br></br>
    ///                        Neue öffentliche Mehoden RegisterAt und UnregisterAt,
    ///                        dafür ist HandleInfoForException jetzt private.
    /// </remarks>
    public class Exceptioner : IInfoViewer
    {
        #region IInfoViewer Members

        /// <summary>
        /// Callback zum Ausgeben von Exceptions: wird vom zuständigen InfoController
        /// aufgerufen, eine entsprechende vorherige Registrierung vorausgesetzt.
        /// </summary>
        /// <param name="sender">Der InfoController</param>
        /// <param name="msgArgs">Container mit Zusatzinfos</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "only left to right reading")]
        public void HandleInfo(object? sender, InfoArgs msgArgs)
        {
            object actMessageObject = msgArgs.MessageObject;
            if (actMessageObject is Exception)
            {
                string msg = ((Exception)actMessageObject).Message;
                if (actMessageObject is ExtendedException)
                {
                    object? messageObject = ((ExtendedException)actMessageObject).InnerException;
                    if (messageObject != null)
                    {
                        actMessageObject = messageObject;
                    }
                }
                string messageObjectType = actMessageObject.GetType().ToString();
                Exception ex = (Exception)actMessageObject;
                if (ex.InnerException != null)
                {
                    msg += Environment.NewLine + messageObjectType + " (" + ex.InnerException.Message + ")";
                }
                else
                {
                    msg += Environment.NewLine + "(" + messageObjectType + ")";
                }
                if (actMessageObject is ExtendedException)
                {
                    ExtendedException ex2 = (ExtendedException)actMessageObject;
                    if ((ex2 != null) && (ex2._suffix != null))
                    {
                        msg += Environment.NewLine + ex2._suffix;
                    }
                }
                if (msgArgs.LogLevel != InfoType.Debug)
                {
                    string productName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown Assembly";
                    MessageBox.Show(msg, "Fehler in " + productName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion IInfoViewer Members

        private Exceptioner()
        {
            // Verhindert die direkte Instanziierung. Dadurch kann diese Klasse (normalerweise)
            // nur noch durch GenericSingletonProvider instanziiert werden.
        }

    }

    /// <summary>
    /// Erweiterte Exception für die Ausgabe zusätzlicher Informationen,
    /// wie evtl. InnerException und StackTrace.
    /// </summary>
    [SerializableAttribute]
    public class ExtendedException : ApplicationException, ISerializable
    {
        internal string? _suffix;
        private string? _exStackTrace; // = null wird vom System vorbelegt

        /// <summary>
        /// Konstruktor: übernimmt zusätzlich zur Exception je einen
        /// (optionalen) Prefix- oder Suffix-String.
        /// </summary>
        /// <param name="prefix">Ein optionaler Text, der vor der Exception ausgegeben wird.</param>
        /// <param name="ex">Die eigentliche Exception</param>
        /// <param name="suffix">Ein optionaler Text, der nach der Exception ausgegeben wird.</param>
        public ExtendedException(string prefix, Exception ex, string? suffix)
          : base(prefix + (ex == null ? "" : ex.Message), ex)
        {
            if (ex != null)
            {
                this._exStackTrace = ex.StackTrace;
                base.HelpLink = ex.HelpLink;
                base.Source = ex.Source;
            }
            this._suffix = suffix;
        }

        /// <summary>
        /// Konstruktor: übernimmt zusätzlich zur Exception einen
        /// (optionalen) Prefix-String.
        /// </summary>
        /// <param name="prefix">Ein optionaler Text, der vor der Exception ausgegeben wird.</param>
        /// <param name="ex">Die eigentliche Exception.</param>
        public ExtendedException(string prefix, Exception ex)
          : this(prefix, ex, null) { }

        /* Komplett auskommentiert, siehe https://github.com/dotnet/docs/issues/34893
        /// <summary>
        /// De-Serialisierungs-Konstruktor;
        /// erzeugt aus dem SerialisierungsContainer die zusätzlichen Felder "Suffix" und "ExStackTrace".
        /// </summary>
        /// <param name="info">Der Serialisierungs-Container</param>
        /// <param name="context">Beschreibung des Serialisierungs-Streams</param>
#if NET8_0_OR_GREATER
        [Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
#endif
        protected ExtendedException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
            this._exStackTrace = info.GetString("ExStackTrace");
            this._suffix = info.GetString("Suffix");
        }
        */

        /* Komplett auskommentiert, siehe https://github.com/dotnet/docs/issues/34893
        /// <summary>
        /// Interne Erweiterung von GetOjectData der Basisklasse Exception;
        /// fügt die neuen Member "Suffix" und "ExStackTrace" hinzu.
        /// </summary>
        /// <param name="info">Der Serialisierungs-Container</param>
        /// <param name="context">Beschreibung des Serialisierungs-Streams</param>
        // [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        // SYSLIB0003 "SecurityPermissionFlag" ist veraltet: "Code Access Security is not supported or honored by the runtime."
#if NET8_0_OR_GREATER
        [Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to GetObjectData
#endif
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Suffix", _suffix);
            info.AddValue("ExStackTrace", _exStackTrace);
        }
        */

        /* Komplett auskommentiert, siehe https://github.com/dotnet/docs/issues/34893
        /// <summary>
        /// Überschriebene Schnittstellenmethode des Objekt-Serialisierers;
        /// ruft die interne Erweiterung von GetOjectData der Basisklasse Exception auf.
        /// </summary>
        /// <param name="info">Der Serialisierungs-Container</param>
        /// <param name="context">Beschreibung des Serialisierungs-Streams</param>
        // [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        // SYSLIB0003 "SecurityPermissionFlag" ist veraltet: "Code Access Security is not supported or honored by the runtime."
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            GetObjectData(info, context);
        }
        */

        /// <summary>
        /// Konvertierung der ExtendedException nach string ohne Prefix und Suffix
        /// </summary>
        /// <returns>Exception + Stacktrace als string</returns>
        public override string ToString()
        {
            return base.ToString() + Environment.NewLine + this._exStackTrace;
        }
    }
}
