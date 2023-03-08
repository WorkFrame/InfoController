using System.Text;
using System.Timers;
using System.Text.RegularExpressions;

namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Ermöglicht Statistische Auswertungen ohne das System
    /// bei häufig durchlaufenen Zählstellen mit zu vielen
    /// Log-Aufrufen zu belasten.
    /// Kann über Timer oder Anzahl Zählvorgänge getriggert werden.
    /// </summary>
    /// <remarks>
    /// File: Statistics.cs
    /// Autor: Erik Nagel
    ///
    /// 28.09.2013 Erik Nagel: erstellt
    /// 14.01.2018 Erik Nagel: Wegen Memory-Leaks und Zombie-Tasks überarbeitet; Ausgabeformat optimiert.<br></br>
    /// </remarks>
    public static class Statistics
    {
        #region public members

        /// <summary>
        /// Muss am Schluss der Verarbeitung aufgerufen werden, um die letzte
        /// Statistik auszugeben und den Timer zu stoppen.
        /// </summary>
        public static void Stop()
        {
            triggerStatistic();
            IsTimerTriggered = false;
            resetStartTimer();
            //MessageBox.Show("Statistics-Stop");
        }

        /// <summary>
        /// Bei True ist die Statistik-Ausgabe zeitgesteuert.
        /// LoggingTriggerCounter gibt dann die Anzahl Millisekunden
        /// bis zum nächsten Trigger-Event vor.
        /// Bei False wird die Statistik-Ausgabe durch die Gesamtzahl
        /// Zählvorgänge gesteuert. LoggingTriggerCounter definiert
        /// hier die Anzahl Zählvorgänge, nach der die Ausgabe erfolgt;
        /// Default: True.
        /// </summary>
        public static bool IsTimerTriggered
        {
            get
            {
                return _isTimerTriggered;
            }
            set
            {
                _isTimerTriggered = value;
                if (!_isTimerTriggered)
                {
                    resetStartTimer();
                }
            }
        }

        /// <summary>
        /// Anzahl Millisekunden oder Anzahl Zählvorgänge bis zur
        /// nächsten Statistik-Ausgabe;
        /// Default: 5000.
        /// </summary>
        public static long LoggingTriggerCounter { get; set; }

        /// <summary>
        /// Nur Zeilen, die diesen regulären Ausdruck erfüllen, werden geloggt.
        /// </summary>
        public static string RegexFilter
        {
            get
            {
                return _regexFilter;
            }
            set
            {
                _regexFilter = value;
                _compiledRegexFilter = new Regex(_regexFilter);
            }
        }

        /// <summary>
        /// Erhöht den Zähler mit dem übergebenen Namen um 1.
        /// Der Zähler wird bei der ersten Referenzierung neu erzeugt.
        /// </summary>
        /// <param name="name">Name des Zählers.</param>
        public static void Inc(string name)
        {
            lock (_locker)
            {
                if (!_incrementer.ContainsKey(name))
                {
                    _incrementer.Add(name, 1);
                }
                else
                {
                    _incrementer[name]++;
                    _overallIncrementCounter++;
                }
                if (IsTimerTriggered)
                {
                    if (_loggingTimer == null)
                    {
                        resetStartTimer();
                    }
                }
                else
                {
                    if (_overallIncrementCounter % LoggingTriggerCounter == 0)
                    {
                        triggerStatistic();
                    }
                }
            }
        }

        /// <summary>
        /// Setzt den Zähler mit dem übergebenen Namen auf 0.
        /// Der Zähler wird bei der ersten Referenzierung neu erzeugt.
        /// Wird als Zähler-Name null übergeben, werden alle Zähler
        /// auf 0 gesetzt und der interne Trigger zurückgesezt.
        /// </summary>
        /// <param name="name">Name des Zählers oder null.</param>
        public static void Reset(string name)
        {
            lock (_locker)
            {
                if (name != null)
                {
                    if (!_incrementer.ContainsKey(name))
                    {
                        _incrementer.Add(name, 0);
                    }
                    else
                    {
                        _incrementer[name] = 0;
                    }
                }
                else
                {
                    foreach (string registeredName in _incrementer.Keys)
                    {
                        _incrementer[registeredName] = 0;
                    }
                    _overallIncrementCounter = 0;
                    if (IsTimerTriggered)
                    {
                        resetStartTimer();
                    }
                }
            }
        }

        #endregion public members

        #region private members

        private static void triggerStatistic()
        {
            StringBuilder message = new StringBuilder();
            foreach (string registeredName in _incrementer.Keys.OrderBy(x => x).ToList())
            {
                bool logIt = true;
                if (!String.IsNullOrEmpty(RegexFilter))
                {
                    MatchCollection? alleTreffer = _compiledRegexFilter?.Matches(registeredName);
                    logIt = alleTreffer?.Count > 0;
                }
                if (logIt)
                {
                    message.Append(String.Format("{0}: {1}", registeredName, _incrementer[registeredName]) + Environment.NewLine);
                }
            }
            if (message.Length > 0)
            {
                InfoController.GetInfoController().Publish(null, message.ToString(), InfoType.Statistics);
            }
        }

        static Statistics()
        {
            LoggingTriggerCounter = 5000; // 5000 Zählvorgänge oder Millisekunden
            IsTimerTriggered = true;
            _regexFilter = "";
            _locker = new object();
        }

        private static object _locker; // für Thread-Locks

        private static long _overallIncrementCounter = 0;
        private static Dictionary<string, long> _incrementer = new Dictionary<string, long>() { };

        private static System.Timers.Timer? _loggingTimer;

        private static bool _isTimerTriggered;
        private static string _regexFilter;
        private static Regex? _compiledRegexFilter;

        private static void resetStartTimer()
        {
            if (_loggingTimer != null)
            {
                _loggingTimer.Stop();
                _loggingTimer.Dispose();
            }
            if (IsTimerTriggered)
            {
                _loggingTimer = new System.Timers.Timer(LoggingTriggerCounter);
                _loggingTimer.Elapsed += new ElapsedEventHandler(loggingTimer_Elapsed);
                _loggingTimer.Start();
            }
        }

        private static void loggingTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _loggingTimer?.Stop();
            lock (_locker)
            {
                triggerStatistic();
            }
            _loggingTimer?.Start();
        }

        #endregion private members
    }
}
