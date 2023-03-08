namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Bildet die Sicht auf den InfoController ab,
    ///           die das Empfangen von Nachrichten umfasst.
    /// </summary>
    /// <remarks>
    /// File: IInfoSource<br></br>
    /// Autor: Erik Nagel, NetEti<br></br>
    ///<br></br>
    /// 08.03.2012 Erik Nagel: erstellt<br></br>
    /// </remarks>
    public interface IInfoSource
    {
        /// <summary>
        /// Hierüber trägt sich ein interessierter Viewer in die Liste der zu
        /// informierenden Viewer ein.
        /// </summary>
        /// <param name="viewer">Der Message-Empfänger</param>
        /// <param name="classType">Der Klassentyp, für den sich msgHandler zuständig erklärt</param>
        /// <param name="triggerInfoTypes">Liste der Message-Typen, für die sich der Viewer interessiert</param>
        void RegisterInfoReceiver(IInfoViewer viewer, Type? classType, InfoType[] triggerInfoTypes);

        /// <summary>
        /// Hierüber trägt sich ein interessierter Viewer in die Liste der zu
        /// informierenden Viewer ein.
        /// </summary>
        /// <param name="viewer">Der Message-Empfänger</param>
        /// <param name="triggerInfoTypes">Liste der Message-Typen, für die sich der Viewer interessiert</param>
        void RegisterInfoReceiver(IInfoViewer viewer, InfoType[] triggerInfoTypes);

        /// <summary>
        /// Hierüber meldet sich ein eingetragener Viewer wieder ab.
        /// </summary>
        /// <param name="viewer">Der Message-Empfänger</param>
        void UnregisterInfoReceiver(IInfoViewer viewer);
    }
}
