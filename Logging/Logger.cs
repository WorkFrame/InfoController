using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Schreibt Messages mit hinzugefügten Timestamps in Logfiles;
    /// implementiert IInfoViewer.
    /// </summary>
    /// <remarks>
    /// Autor: Erik Nagel, NetEti
    ///
    /// 08.03.2012 Erik Nagel: erstellt
    /// 15.03.2014 Erik Nagel: Statistics eingebaut; Regex-Filter implementiert.
    /// 14.07.2016 Erik Nagel: Exceptions werden jetzt auf jeden Fall geloggt.
    /// 14.01.2018 Erik Nagel: Wegen Memory-Leaks überarbeitet (StringBuilder); Ausgabeformat optimiert.
    /// 21.01.2022 Erik Nagel: _replaceRegex implementiert.
    /// 06.11.2023 Erik Nagel: IShowable implementiert.
    /// </remarks>
    public class Logger : LoggerBase, IShowable
    {

        #region LoggerBase Members

        /// <summary>
        /// Callback zum Loggen der Message: wird vom zuständigen InfoController
        /// aufgerufen, eine entsprechende vorherige Registrierung vorausgesetzt.
        /// </summary>
        /// <param name="sender">Der InfoController</param>
        /// <param name="msgArgs">Die Message mit Text, Typ und Timestamp</param>
        public override void HandleInfo(object? sender, InfoArgs msgArgs)
        {
            string message = msgArgs.MessageObject.ToString() ?? "";
            bool logIt = true;
            if (!String.IsNullOrEmpty(this._regexFilter) && msgArgs.LogLevel != InfoType.Exception && msgArgs.LogLevel != InfoType.NoRegex)
            {
                MatchCollection alleTreffer = this._compiledRegexFilter.Matches(message);
                if (alleTreffer.Count > 0)
                {
                    if (this._replaceRegex)
                    {
                        message = message.Replace(alleTreffer[0].Value, "").Trim();
                    }
                }
                else
                {
                    logIt = false;
                }
            }
            if (logIt)
            {
                if (this.PlainMessage)
                {
                    this.Log(message);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(msgArgs.Timestamp);
                    sb.Append(" ");
                    if ((msgArgs.ThreadInfos ?? "").Length > 0)
                    {
                        sb.Append(msgArgs.ThreadInfos);
                        sb.Append(" ");
                    }
                    sb.Append(msgArgs.LogLevel.ToString().Replace("NoRegex", "System").PadRight(10));
                    if (message.Contains(Environment.NewLine))
                    {
                        sb.Append(Environment.NewLine + " ".PadRight(this.StandardIndent));
                        sb.Append(message.Replace(Environment.NewLine, Environment.NewLine + " ".PadRight(this.StandardIndent)));
                    }
                    else
                    {
                        sb.Append(message);
                    }
                    this.Log(sb.ToString());
                }
            }
        }

        /// <summary>
        /// Schreibt einen Eintrag in's Logfile, wird aber von außen
        /// nicht direkt aufgerufen; vielmehr muss man den Logger als Viewer über
        /// seine Methode HandleInfoForLog beim InfoController eintragen.
        /// </summary>
        /// <param name="message">Der Message-Text</param>
        // 08.03.2012 Nagel: FxCop Suppression
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "logging exception")]
        protected override void WriteLog(string message)
        {
            int maxTries = 5;
            lock (_locker)
            {
                StreamWriter? streamWriter;
                int i = 0;
                do
                {
                    streamWriter = null;
                    try
                    {
                        streamWriter = new StreamWriter(new FileStream(this.LogTargetInfo, FileMode.Append, FileAccess.Write), Encoding.Default);
                        streamWriter.WriteLine(message);
                        i = maxTries;
                    }
                    catch (SystemException)
                    {
                        Thread.Sleep(10);
                    }
                    finally
                    {
                        if (streamWriter != null)
                        {
                            try
                            {
                                streamWriter.Close();
                            }
                            catch { }
                            streamWriter.Dispose();
                        }
                    }
                } while (++i < maxTries);
            }
        }

        /// <summary>
        /// Benennt das Log (DebugFile) nach einer definierten Zeitspanne DebugFileArchivingInterval um
        /// und löscht Logs (DebugFiles), die älter als DebugFileArchiveLifetime sind.
        /// Um eventuelles Sperren des DebugFiles muss sich der aufrufende Prozess kümmern.
        /// </summary>
        /// <param name="debugArchivingInterval">Zeitabstand, in dem das aktuelle Log (DebugFile)
        /// unter einem Namen mit Zeitangabe gesichert und geleert wird.</param>
        /// <param name="debugArchiveMaxCount">Maximale Lebensdauer von archivierten Logs (DebugFiles).
        /// Ältere Logs werden gelöscht.</param>
        protected override void OrganizeLoggings(TimeSpan debugArchivingInterval, int debugArchiveMaxCount)
        {
            try
            {
                if (File.Exists(this.LogTargetInfo))
                {
                    FileInfo actDebugFile = new FileInfo(this.LogTargetInfo);
                    if (DateTime.UtcNow - actDebugFile.CreationTimeUtc > debugArchivingInterval)
                    {
                        File.Move(this.LogTargetInfo,
                            this.LogTargetInfo + "." + DateTime.Now.ToString("MMdd_HHmmss"));
                    }
                }

            }
            catch { }
            if (debugArchiveMaxCount > 0)
            {
                List<FileInfo> archiveFiles =
                    new DirectoryInfo(Path.GetDirectoryName(this.LogTargetInfo) ?? "")
                        .GetFiles(Path.GetFileName(this.LogTargetInfo) + ".*").ToList();
                if (archiveFiles.Count > debugArchiveMaxCount)
                {
                    foreach (FileInfo fileInfo in archiveFiles
                        .OrderBy(f => f.CreationTimeUtc)
                        .Take(archiveFiles.Count - debugArchiveMaxCount))
                    {
                        try
                        {
                            File.Delete(fileInfo.FullName);
                        }
                        catch { }
                    }
                }
            }
        }

        #endregion LoggerBase Members

        #region public members

        /// <summary>
        /// Parameterloser Konstruktor: setzt das Logfile auf
        /// GetEnvironmentVariable("TEMP") + \ + ProductName + @".log"
        /// und ruft den nächsten Konstruktor auf.
        /// </summary>
        public Logger() : this(getTempLogPath()) { }

        /// <summary>
        /// Konstruktor: übernimmt plainMessage,
        /// setzt das Logfile auf GetEnvironmentVariable("TEMP") + \ + ProductName + @".log"
        /// und ruft den nächsten Konstruktor auf.
        /// </summary>
        /// <param name="plainMessage">Bei True werden keine Zusatzinformationen ausgegeben (Default: false).</param>
        public Logger(bool plainMessage) : this(getTempLogPath(), plainMessage) { }

        /// <summary>
        /// Konstruktor: übernimmt logFilePathName,
        /// setzt plainMessage auf false
        /// und ruft den nächsten Konstruktor auf.
        /// </summary>
        /// <param name="logFilePathName">Pfad- und Name des Logfiles</param>
        public Logger(string logFilePathName) : this(logFilePathName, false) { }

        /// <summary>
        /// Konstruktor: übernimmt logFilePathName und plainMessage,
        /// setzt regexFilter auf ""
        /// und ruft den nächsten Konstruktor auf.
        /// </summary>
        /// <param name="logFilePathName">Pfad- und Name des Logfiles</param>
        /// <param name="plainMessage">Bei True werden keine Zusatzinformationen ausgegeben (Default: false).</param>
        public Logger(string logFilePathName, bool plainMessage) : this(logFilePathName, "", plainMessage) { }

        /// <summary>
        /// Konstruktor: übernimmt plainMessage und regexFilter,
        /// setzt das Logfile auf GetEnvironmentVariable("TEMP") + \ + ProductName + @".log"
        /// und ruft den nächsten Konstruktor auf.
        /// </summary>
        /// <param name="plainMessage">Bei True werden keine Zusatzinformationen ausgegeben (Default: false).</param>
        /// <param name="regexFilter">Nur Zeilen, die diesen regulären Ausdruck erfüllen, werden geloggt.</param>
        public Logger(bool plainMessage, string regexFilter) : this(getTempLogPath(), regexFilter?? "", plainMessage) { }

        /// <summary>
        /// Vollständiger Konstruktor.
        /// </summary>
        /// <param name="logFilePathName">Pfad- und Name des Logfiles</param>
        /// <param name="regexFilter">Nur Zeilen, die diesen regulären Ausdruck erfüllen, werden geloggt.</param>
        /// <param name="plainMessage">Bei True werden keine Zusatzinformationen ausgegeben (Default: false).</param>
        public Logger(string logFilePathName, string regexFilter, bool plainMessage) : base(logFilePathName)
        {
            this._regexFilter = regexFilter;
            this._locker = new object();
            this.PlainMessage = plainMessage;
            this.StandardIndent = 4;
            if (!this._regexFilter.StartsWith("@"))
            {
                this._replaceRegex = false;
            }
            else
            {
                this._replaceRegex = true;
                this._regexFilter = this._regexFilter.Substring(1);
            }
            this._compiledRegexFilter = new Regex(this._regexFilter);
        }

        /// <summary>
        /// Kann überschrieben werden um das Log anzuzeigen;
        /// bei Textdateien z.B. über Ausgabe in den Standard-Editor.
        /// </summary>
        public override void Show()
        {
            this.Flush();
            Thread.Sleep(100);
            if (File.Exists(this.LogTargetInfo))
            {
                try { Process.Start("notepad.exe", this.LogTargetInfo); }
                catch (Exception /* ex */)
                {
                    // MessageBox.Show(ex.Message, "Kann das Logfile nicht anzeigen.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Einrückung der Folgezeilen bei mehrzeiligen Messages.
        /// Default: 4.
        /// </summary>
        public int StandardIndent { get; set; }

        #endregion public members

        #region private members

        private readonly object _locker;
        private string _regexFilter;
        private Regex _compiledRegexFilter;
        private bool _replaceRegex;

        /// <summary>
        /// Statisch, ermittelt den Default für Logfile- Pfad und Namen.
        /// </summary>
        /// <returns></returns>
        private static string getTempLogPath()
        {
            string logFilePathName = System.Environment.GetEnvironmentVariable("TEMP") ?? "";
            string productName = "";
            System.Reflection.Assembly? assembly = System.Reflection.Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                // rtn = System.Windows.Forms.Application.ProductName; break;
                productName = assembly.GetName().Name ?? "unbekannte Assembly";
            }
            return Path.Combine(logFilePathName, productName + @".log");
        }

        #endregion private members

    }
}
