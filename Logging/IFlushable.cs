namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Zeigt an, dass die implementierende Instanz eine
    /// parameterlose Methode zum Aufräumen und ggf.
    /// Wegschreiben gesammelter Informationen bietet.
    /// </summary>
    /// <remarks>
    /// Autor: Erik Nagel, NetEti<br></br>
    ///<br></br>
    /// 25.07.2019 Erik Nagel: erstellt<br></br>
    /// </remarks>
    public interface IFlushable
    {
        /// <summary>
        /// Sorgt dafür, dass alle anstehenden Aktionen ausgeführt
        /// werden, z.B. gefüllte Zwischentabellen (Buffer)
        /// abgearbeitet (ge-flusht) werden.
        /// </summary>
        void Flush();
    }
}
