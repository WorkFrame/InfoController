using System.Timers;
using NetEti.Globals;

namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Abstrakte Basis für diverse Logger;
    /// implementiert IInfoViewer.
    /// </summary>
    /// <remarks>
    /// File: LoggerBase.cs<br></br>
    /// Autor: Erik Nagel, NetEti<br></br>
    ///<br></br>
    /// 08.03.2012 Erik Nagel: erstellt<br></br>
    /// 08.03.2012 Erik Nagel: CutLog(...) eingebaut.<br></br>
    /// 10.05.2013 Erik Nagel: CutLog(string logPath, long countItemsFromStart, long countItemsToEnd, bool countLines) neu;
    ///                        beim Wegschreiben des gekürzten Logs System.Text.Encoding.Default eingefügt.<br></br>
    /// 27.08.2015 Erik Nagel: Asynchrones Logging implementiert.
    /// 11.08.2018 Erik Nagel: Sortieren nach Timestamp bei PlainMessage==False implementiert.
    /// </remarks>
    public abstract class LoggerBase : IInfoViewer, IFlushable, IDisposable
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
                    this.flushBuffer(true);
                }
                this._disposed = true;
            }
        }

        /// <summary>
        /// Finalizer: wird vom GarbageCollector aufgerufen.
        /// </summary>
        ~LoggerBase()
        {
            this.Dispose(false);
        }

        #endregion IDisposable Member

        #region IInfoViewer Members

        /// <summary>
        /// Callback zum Loggen der Message: wird vom zuständigen InfoController
        /// aufgerufen, eine entsprechende vorherige Registrierung vorausgesetzt.
        /// </summary>
        /// <param name="sender">Der InfoController</param>
        /// <param name="msgArgs">Die Message mit Text, Typ und Timestamp</param>
        public abstract void HandleInfo(object? sender, InfoArgs msgArgs);

        #endregion IInfoViewer Members

        #region IFlushable Members

        /// <summary>
        /// Sorgt dafür, dass alle anstehenden Aktionen ausgeführt
        /// werden, z.B. gefüllte Zwischentabellen (Buffer)
        /// abgearbeitet (ge-flusht) werden.
        /// </summary>
        public void Flush()
        {
            this.flushBuffer(true);
        }

        #endregion IFlushable Members 

        /// <summary>
        /// Ziel-Pfad, z.B. bei Text-Logs Name und Pfad des Logfiles.
        /// </summary>
        public string LogTargetInfo
        {
            get
            {
                return (this._logTargetPathInfo) ?? "";
            }
            set
            {
                this._logTargetPathInfo = Path.GetFullPath(value);
            }
        }

        /// <summary>
        /// Bei True ist die Logging-Ausgabe zeitgesteuert.
        /// LoggingTriggerCounter gibt dann die Anzahl Millisekunden
        /// bis zum nächsten Trigger-Event vor.
        /// Bei False wird die Ausgabe durch die Gesamtzahl
        /// Zählvorgänge gesteuert. LoggingTriggerCounter definiert
        /// hier die Anzahl Zählvorgänge, nach der die Ausgabe erfolgt;
        /// Default: True.
        /// </summary>
        public bool IsTimerTriggered
        {
            get
            {
                return this._isTimerTriggered;
            }
            set
            {
                if (this._isTimerTriggered != value)
                {
                    this._isTimerTriggered = value;
                    this.resetStartTimer();
                }
            }
        }

        /// <summary>
        /// Anzahl Millisekunden oder Anzahl Zählvorgänge bis zur
        /// nächsten Logging-Ausgabe;
        /// Default: 5000.
        /// </summary>
        public long LoggingTriggerCounter {
            get
            {
                return this._loggingTriggerCounter;
            }
            set
            {
                if (this._loggingTriggerCounter != value)
                {
                    this._loggingTriggerCounter = value;
                    this.resetStartTimer();
                }
            }
        }

        /// <summary>
        /// Maximale Anzahl Zeilen, die ein MessageBuffer aufnehmen kann,
        /// bevor er zwangzweise geflusht wird. Diese Einstellung wirkt
        /// auch bei Timer-gesteuerten Logging.
        /// Default: 10000.
        /// </summary>
        public int MaxBufferLineCount { get; set; }

        /// <summary>
        /// Maximale Anzahl von archivierten Logs (DebugFiles, o.ä.).
        /// Bei Überzahl werden jeweils die ältesten gelöscht.
        /// Default: 0 (entspricht unendlich vielen).
        /// </summary>
        public int DebugArchiveMaxCount { get; set; }

        /// <summary>
        /// Zeitabstand, in dem das aktuelle Logging (DebugFile, o.ä.)
        /// archiviert und geleert wird. Muss von außen gesetzt werden.
        /// Default: TimeSpan.Zero.
        /// </summary>
        public TimeSpan DebugArchivingInterval { get; set; }

        /// <summary>
        /// Übernimmt einen Eintrag für das Logging.
        /// Bietet Timer- oder Counter-gesteuertes Puffern von Messages.
        /// Loggt selbst nicht direkt, sondern ruft die hier abstract definierte Methode
        /// WriteLog auf, in der dann das physikalische Schreiben erfolgt.
        /// Flusht dann asynchron den Puffer um die Logging-Performance zu verbessern.
        /// </summary>
        /// <param name="message">Der Message-Text</param>
        public void Log(string message)
        {
            lock (this._lockMe)
            {
                this.messageBuffer.Add(message);
            }
            if (this.IsTimerTriggered)
            {
                if (this._loggingTimer == null)
                {
                    this.resetStartTimer();
                }
            }
            else
            {
                if (++_overallIncrementCounter > LoggingTriggerCounter)
                {
                    this._overallIncrementCounter = 0;
                    this.flushBuffer(false);
                }
            }
        }

        /// <summary>
        /// Konstruktor: setzt den Pfadnamen für das Logfile.
        /// </summary>
        /// <param name="logTargetInfo">Pfad- und Name des Logfiles, Tabellenname, etc.</param>
        public LoggerBase(string logTargetInfo)
        {
            this.LogTargetInfo = logTargetInfo;
            this.MaxBufferLineCount = 10000; // Maximale Anzahl Zeilen, die ein MessageBuffer aufnehmen kann
            this.LoggingTriggerCounter = 5000; // 5000 Zählvorgänge oder Millisekunden
            this._overallIncrementCounter = 0;
            this.IsTimerTriggered = true;
            this._lockMe = new object();
            this._lockMe2 = new object();
            this.messageBuffer = new List<string>();
            this.messageFlushBuffer = new List<string>();
            this.DebugArchivingInterval = TimeSpan.Zero;
            this.DebugArchiveMaxCount = 0;
        }

        /// <summary>
        /// Kürzt das Log-File 'logPath' auf die letzten 'countItems' Einträge.
        /// Das ursprüngliche Log-File wird mit der Extension '.last' gesichert.
        /// Wenn 'countLines' false ist, bleiben die Einträge mit den letzten 'countItems'
        /// Datumswerten erhalten, ansonsten die letzten 'countItems' Zeilen.
        /// </summary>
        /// <param name="logPath">Pfad + Name des Logfiles.</param>
        /// <param name="countItemsToEnd">Anzahl Datumseinträge oder Zeilen, die am Ende erhalten bleiben sollen.</param>
        /// <param name="countLines">Bei True werden Zeilen gezählt, ansonsten Datumseinträge.</param>
        public static void CutLog(string logPath, long countItemsToEnd, bool countLines)
        {
            LoggerBase.CutLog(logPath, 0, countItemsToEnd, countLines);
        }

        /// <summary>
        /// Kürzt das Log-File 'logPath' auf die letzten 'countItems' Einträge.
        /// Das ursprüngliche Log-File wird mit der Extension '.last' gesichert.
        /// Wenn 'countLines' false ist, bleiben die Einträge mit den letzten 'countItems'
        /// Datumswerten erhalten, ansonsten die letzten 'countItems' Zeilen.
        /// </summary>
        /// <param name="logPath">Pfad + Name des Logfiles.</param>
        /// <param name="countItemsFromStart">Anzahl Datumseinträge oder Zeilen, die am Anfang erhalten bleiben sollen.</param>
        /// <param name="countItemsToEnd">Anzahl Datumseinträge oder Zeilen, die am Ende erhalten bleiben sollen.</param>
        /// <param name="countLines">Bei True werden Zeilen gezählt, ansonsten Datumseinträge.</param>
        public static void CutLog(string logPath, long countItemsFromStart, long countItemsToEnd, bool countLines)
        {
            if (File.Exists(logPath))
            {
                List<string> logLines = new List<string>();
                List<string> daten = new List<string>();
                string letztesDatum = "";
                StreamReader actFile = new StreamReader(logPath, System.Text.Encoding.Default);
                while (!actFile.EndOfStream)
                {
                    string zeile = actFile.ReadLine() ?? "";
                    logLines.Add(zeile);
                    if (!countLines)
                    {
                        string datum = (zeile + "          ").Substring(0, 10).Trim();
                        if (Global.IsDate(datum) && !datum.Equals(letztesDatum))
                        {
                            daten.Add(datum);
                            letztesDatum = datum;
                        }
                    }
                }
                actFile.Close();
                actFile.Dispose();
                string savPfadname = Path.Combine(Path.GetDirectoryName(logPath) ?? "",
                    Path.GetFileNameWithoutExtension(logPath) + ".last");
                if (File.Exists(savPfadname))
                {
                    File.Delete(savPfadname);
                }
                File.Move(logPath, savPfadname);

                if (!countLines)
                {
                    if (daten.Count > countItemsFromStart + countItemsToEnd)
                    {
                        daten.RemoveRange(Convert.ToInt32(countItemsFromStart), daten.Count - Convert.ToInt32(countItemsFromStart + countItemsToEnd));
                    }
                }
                else
                {
                    if (logLines.Count > countItemsFromStart + countItemsToEnd)
                    {
                        logLines.RemoveRange(Convert.ToInt32(countItemsFromStart), logLines.Count - Convert.ToInt32(countItemsFromStart + countItemsToEnd));
                    }
                }

                StreamWriter newFile = new StreamWriter(logPath, false, System.Text.Encoding.Default);

                foreach (string zeile in logLines)
                {
                    bool found = true;
                    if (!countLines)
                    {
                        string datum = (zeile + "          ").Substring(0, 10).Trim();
                        if (!daten.Contains(datum))
                        {
                            found = false;
                        }
                    }
                    if (found)
                    {
                        newFile.WriteLine(zeile);
                    }
                }
                newFile.Close();
                newFile.Dispose();
            }
        }

        #endregion public members

        #region protected members

        /// <summary>
        /// Bei True werden Messages unverändert ausgegeben; wegen der Asynchronität
        /// des Message-Handlings kann es hier zu Reihenfolge-Vertauschungen kommen.
        /// Bei False werden Messages mit einem Timestamp versehen und vor 
        /// Flush des Message-Buffers nach dem Timestamp sortiert.
        /// Default: False.
        /// </summary>
        protected bool PlainMessage { get; set; }

        /// <summary>
        /// Schreibt einen Eintrag in's Logfile (oder irgendwo hin), wird aber von außen
        /// nicht direkt aufgerufen; vielmehr sollte man den Logger als Viewer
        /// beim InfoController eintragen.
        /// </summary>
        /// <param name="message">Der Message-Text</param>
        protected abstract void WriteLog(string message);

        /// <summary>
        /// Bietet die Möglichkeit, Loggings aufzuräumen. Macht selbst nichts, kann überschrieben werden.
        /// </summary>
        /// <param name="debugArchivingInterval">Zeitabstand, in dem das aktuelle Logging
        /// archiviert und geleert wird.</param>
        /// <param name="debugArchiveMaxCount">Maximale Anzahl von archivierten
        /// Loggings bis sie gelöscht werden.</param>
        protected virtual void OrganizeLoggings(TimeSpan debugArchivingInterval, int debugArchiveMaxCount) { }

        #endregion protected members

        #region private members

        private string? _logTargetPathInfo;
        private System.Timers.Timer? _loggingTimer;
        private bool _isTimerTriggered;
        private object _lockMe;
        private object _lockMe2;
        private Thread? _worker;
        private List<String> messageBuffer;
        private List<String> messageFlushBuffer;
        private long _overallIncrementCounter;
        private long _loggingTriggerCounter;

        private void resetStartTimer()
        {
            if (this._loggingTimer != null)
            {
                this._loggingTimer.Stop();
                this._loggingTimer.Dispose();
            }
            if (IsTimerTriggered)
            {
                this._loggingTimer = new System.Timers.Timer(LoggingTriggerCounter);
                this._loggingTimer.Elapsed += new ElapsedEventHandler(loggingTimer_Elapsed);
                this._loggingTimer.Start();
            }
        }

        private void loggingTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            this._loggingTimer?.Stop();
            this.flushBuffer(false);
            this._loggingTimer?.Start();
        }

        private void flushBuffer(bool sync)
        {
            if (this._worker != null)
            {
                if (this._worker.IsAlive)
                {
                    this._worker.Join();
                }
            }
            this._worker = new Thread(this.flushBufferAsync);
            this._worker.IsBackground = false; // auf jeden Fall noch beenden
            this._overallIncrementCounter = 0;
            if (this._loggingTimer != null)
            {
                this._loggingTimer.Stop();
            }
            lock (this._lockMe)
            {
                this.messageFlushBuffer.Clear();
                for (int i = 0; i < this.messageBuffer.Count; i++)
                {
                    this.messageFlushBuffer.Add(this.messageBuffer[i]);
                }
                this.messageBuffer.Clear();
            }
            this._worker.Start();
            if (sync)
            {
                this._worker.Join();
            }
        }

        private void flushBufferAsync()
        {
            lock (this._lockMe2)
            {
                if (!this.PlainMessage)
                {
                    this.messageFlushBuffer.Sort();
                    //this.WriteLog(String.Format($"--- MaxBufferLineCount: {MaxBufferLineCount}, IsTimerTriggered: {IsTimerTriggered}, LoggingTriggerCounter: {LoggingTriggerCounter} ---"));
                }
                for (int i = 0; i < this.messageFlushBuffer.Count; i++)
                {
                    this.WriteLog(this.messageFlushBuffer[i]);
                }
                if (this.DebugArchivingInterval > TimeSpan.Zero)
                {
                    this.OrganizeLoggings(this.DebugArchivingInterval, this.DebugArchiveMaxCount);
                }
            }
        }

        #endregion private members

    }
}
