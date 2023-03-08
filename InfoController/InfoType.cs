using System.Collections.ObjectModel;

namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Aufzählung der möglichen Typen einer Information.
    /// </summary>
    public enum InfoType
    {
        /// <summary>Meldungen werden nur von InfoReceivern, die sich für diesen Typ für zuständig erklären, verarbeitet.</summary>
        Statistics,
        /// <summary>Meldungen werden nur geloggt</summary>
        Debug,
        /// <summary>Meldungen werden geloggt und von InfoReceivern, die sich für diesen Typ für zuständig erklären, verarbeitet.</summary>
        Info,
        /// <summary>Meldungen werden geloggt und von InfoReceivern, die sich für diesen Typ für zuständig erklären, verarbeitet.</summary>
        Warn,
        /// <summary>Meldungen werden geloggt und von InfoReceivern, die sich für diesen Typ für zuständig erklären, verarbeitet.</summary>
        Milestone,
        /// <summary>Meldungen werden geloggt und von InfoReceivern, die sich für diesen Typ für zuständig erklären, verarbeitet.</summary>
        Error,
        /// <summary>Meldungen werden geloggt und von InfoReceivern, die sich für diesen Typ für zuständig erklären, verarbeitet.</summary>
        Exception,
        /// <summary>Meldungen werden ohne Einschränkung durch einen möglichen User-Filter geloggt.</summary>
        NoRegex,
        /// <summary>Meldungen werden nicht geloggt.</summary>
        Nolog,
        /// <summary>Unbekannter InfoType für syntaktische Vorbelegungen.</summary>
        unknown
    };

    /// <summary>
    /// Die Gesamt-Information, die vom InfoController verarbeitet und weitergegeben wird:
    /// Message-Object + Message-Typ + Timestamp.
    /// </summary>
    public class InfoArgs : EventArgs
    {
        /// <summary>
        /// Der parametrisierte Konstruktor, der Message-Text + Message-Typ + Timestamp
        /// in die Properties übernimmt.
        /// </summary>
        /// <param name="messageInstance">Das verschickte Message-Objekt</param>
        /// <param name="_LogLevel">Der Message-Typ, z.B.: InfoType.Error</param>
        /// <param name="_Timestamp">Datum und Uhrzeit im Format '21.03.2012 12:22:36'</param>
        public InfoArgs(object messageInstance, InfoType _LogLevel, string _Timestamp) : this(messageInstance, _LogLevel, _Timestamp, "") { }

        /// <summary>
        /// Der parametrisierte Konstruktor, der Message-Text + Message-Typ + Timestamp + ThreadInfos
        /// in die Properties übernimmt.
        /// </summary>
        /// <param name="messageInstance">Das verschickte Message-Objekt</param>
        /// <param name="_LogLevel">Der Message-Typ, z.B.: InfoType.Error</param>
        /// <param name="_Timestamp">Datum und Uhrzeit im Format '21.03.2012 12:22:36'</param>
        /// <param name="threadInfos">Informationen zum aktuellern Thread.</param>
        public InfoArgs(object messageInstance, InfoType _LogLevel, string _Timestamp, string threadInfos)
        {
            this._MessageObject = messageInstance;
            this._LogLevel = _LogLevel;
            this._Timestamp = _Timestamp;
            this.ThreadInfos = threadInfos;
            this._threadInfos = threadInfos;
        }

        private object _MessageObject;
        /// <summary>
        /// Das Message-Objekt
        /// </summary>
        public object MessageObject
        {
            get
            {
                return (this._MessageObject);
            }
        }

        private InfoType _LogLevel;
        /// <summary>
        /// Der InfoType der Message, z.B.: InfoType.Error.
        /// </summary>
        public InfoType LogLevel
        {
            get
            {
                return this._LogLevel;

            }
        }

        /// <summary>
        /// Konvertiert einen InfoType in sein String-Äquivalent, also z.B.:
        /// InfoType.Error in "ERROR".
        /// </summary>
        public string LogLevelText
        {
            get
            {
                return this._LogLevel.ToString();

            }
        }

        private string _Timestamp;
        /// <summary>
        /// Der Timestamp der Message im Format '31.01.2009 12:22:36'.
        /// </summary>
        public string Timestamp
        {
            get
            {
                return this._Timestamp;
            }
        }

        private string _threadInfos;
        /// <summary>
        /// Informationen zum aktuellen Thread.
        /// </summary>
        public string ThreadInfos
        {
            get
            {
                return this._threadInfos;
            }
            set
            {
                this._threadInfos = value;
            }
        }


    } // public class InfoArgs : EventArgs

    /// <summary>
    /// Liefert Typen und Klassen für den InfoController.<br></br>
    /// </summary>
    /// <remarks>
    /// File: InfoType.cs<br></br>
    /// Autor: Erik Nagel, NetEti<br></br>
    ///<br></br>
    /// 08.03.2012 Erik Nagel: erstellt<br></br>
    /// 21.09.2013 Erik Nagel: InfoArgs.ThreadInfos hinzugefügt.<br></br>
    /// 26.09.2013 Erik Nagel: Typ 'Statistics' hinzugefügt..<br></br>
    /// </remarks>
    public static class InfoTypes
    {
        /// <summary>
        /// { }
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "ReadOnlyCollection is in fact readonly")]
        public static readonly ReadOnlyCollection<InfoType> None = new ReadOnlyCollection<InfoType>(new InfoType[] { });
        /// <summary>
        /// { InfoType.Debug, InfoType.Info, InfoType.Warn, InfoType.Milestone, InfoType.Error, InfoType.Exception }.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "ReadOnlyCollection is in fact readonly")]
        public static readonly ReadOnlyCollection<InfoType> All = new ReadOnlyCollection<InfoType>(new InfoType[] { InfoType.Debug, InfoType.Info, InfoType.Warn, InfoType.Milestone, InfoType.Error, InfoType.Exception, InfoType.NoRegex });
        /// <summary>
        /// { InfoType.Milestone, InfoType.Error, InfoType.Exception }.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "ReadOnlyCollection is in fact readonly")]
        public static readonly ReadOnlyCollection<InfoType> Serious = new ReadOnlyCollection<InfoType>(new InfoType[] { InfoType.Milestone, InfoType.Error, InfoType.Exception });
        /// <summary>
        /// { InfoType.Info, InfoType.Warn, InfoType.Milestone, InfoType.Error, InfoType.Exception }.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "ReadOnlyCollection is in fact readonly")]
        public static readonly ReadOnlyCollection<InfoType> Average = new ReadOnlyCollection<InfoType>(new InfoType[] { InfoType.Info, InfoType.Warn, InfoType.Milestone, InfoType.Error, InfoType.Exception });
        /// <summary>
        /// { InfoType.Info, InfoType.Milestone }.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "ReadOnlyCollection is in fact readonly")]
        public static readonly ReadOnlyCollection<InfoType> Expected = new ReadOnlyCollection<InfoType>(new InfoType[] { InfoType.Info, InfoType.Milestone });
        /// <summary>
        /// { InfoType.Warn, InfoType.Error, InfoType.Exception }.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "ReadOnlyCollection is in fact readonly")]
        public static readonly ReadOnlyCollection<InfoType> Unexpected = new ReadOnlyCollection<InfoType>(new InfoType[] { InfoType.Warn, InfoType.Error, InfoType.Exception });

        /// <summary>
        /// Konvertiert eine InfoType-Collection in das entsprechende InfoType-Array.
        /// </summary>
        /// <param name="infoTypeCollection">ReadOnlyCollection&lt;InfoType&gt;</param>
        /// <returns>Ein entsprechendes InfoType[]</returns>
        public static InfoType[] Collection2InfoTypeArray(ReadOnlyCollection<InfoType> infoTypeCollection)
        {
            InfoType[] rtn = new InfoType[infoTypeCollection.Count];
            infoTypeCollection.CopyTo(rtn, 0);
            return rtn;
        }

        /// <summary>
        /// Konvertiert einen String wie z.B.: "DEBUG|INFO|WARN|MILESTONE|ERROR"
        /// in das entsprechende InfoType-Array.
        /// </summary>
        /// <param name="infoPipeInfo">String mit durch Pipe getrennten Info-Typen</param>
        /// <returns>InfoType[] mit den entsprechenden, echten InfoType(n)</returns>
        public static InfoType[] String2InfoTypeArray(string infoPipeInfo)
        {
            return InfoTypes.Collection2InfoTypeArray(InfoTypes.String2InfoType(infoPipeInfo));
        }

        /// <summary>
        /// Konvertiert einen String wie z.B.: "DEBUG|INFO|WARN|MILESTONE|ERROR"
        /// in die entsprechende InfoType-Collection.
        /// </summary>
        /// <param name="infoPipeInfo">String mit durch Pipe getrennten Info-Typen</param>
        /// <returns>ReadOnlyCollection&lt;InfoType&gt; mit den entsprechenden, echten InfoType(n)</returns>
        public static ReadOnlyCollection<InfoType> String2InfoType(string infoPipeInfo)
        {
            System.Collections.ArrayList tmpTypes = new System.Collections.ArrayList();
            ReadOnlyCollection<InfoType>? tmpInfoType = null;
            InfoType[] tmpInfoTypeArray;
            string[] typeStrings = infoPipeInfo.Split('|');
            foreach (string infoString in typeStrings)
            {
                bool goOn = false;
                switch (infoString.ToUpper(System.Globalization.CultureInfo.CurrentCulture))
                {
                    // Zuerst werden die Gruppen-Typen abgehandelt, diese setzen das gesamte
                    // tmpInfoType und führen direkt zum Verarbeitungsende.
                    case "NONE": tmpInfoType = InfoTypes.None; break;
                    case "ALL": tmpInfoType = InfoTypes.All; break;
                    case "SERIOUS": tmpInfoType = InfoTypes.Serious; break;
                    case "AVERAGE": tmpInfoType = InfoTypes.Average; break;
                    case "EXPECTED": tmpInfoType = InfoTypes.Expected; break;
                    case "UNEXPECTED": tmpInfoType = InfoTypes.Unexpected; break;
                    default:
                        // Hier werden die Teilstrings behandelt, die einzelnen InfoType(s) entsprechen.
                        goOn = true;
                        try
                        {
                            InfoType infoTypeMember = (InfoType)Enum.Parse(typeof(InfoType), infoString, true);
                            tmpTypes.Add(infoTypeMember);
                        }
                        catch (System.ArgumentException) { }
                        break;
                }
                if (!goOn)
                {
                    break;
                }
            }
            if (tmpInfoType != null)
            {
                return tmpInfoType;
            }
            else
            {
                tmpInfoTypeArray = new InfoType[tmpTypes.Count];
                if (tmpTypes.Count > 0)
                {
                    for (int i = 0; i < tmpTypes.Count; i++)
                    {
                        tmpInfoTypeArray[i] = InfoType.unknown;
                        object? obj = tmpTypes[i];
                        if (obj is InfoType)
                        {
                            tmpInfoTypeArray[i] = (InfoType)obj;
                        }
                    }
                }
                return (new ReadOnlyCollection<InfoType>(tmpInfoTypeArray));
            }
        }

        /// <summary>
        /// Konvertiert ein InfoType-Array, also z.B.: InfoType[] SERIOUS in
        /// einen entsprechenden String wie z.B.: "MILESTONE|ERROR|EXCEPTION".
        /// </summary>
        /// <param name="ita">InfoType[] mit den entsprechenden, echten InfoType(n)</param>
        /// <returns>String mit durch Pipe getrennten InfoType-Strings</returns>
        public static string InfoTypeArray2String(InfoType[] ita)
        {
            string debugInfoStr = "";
            string delim = "";
            foreach (InfoType it in ita)
            {
                debugInfoStr += delim + it.ToString();
                delim = "|";
            }
            return (debugInfoStr);
        }
    }
}
