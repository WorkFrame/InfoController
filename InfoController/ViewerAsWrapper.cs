namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Wrapperklasse um einen EventHandler; Implementiert IInfoViewer.
    /// Eine Instanz dieser Klasse kann um einen EventHandler&lt;InfoArgs&gt;
    /// gewrapt werden und als Viewer weiterverwendet werden.
    /// </summary>
    /// <remarks>
    /// File: ViewerAsWrapper
    /// Autor: Erik Nagel, NetEti
    ///
    /// 19.03.2012 Erik Nagel: erstellt
    /// </remarks>
    public class ViewerAsWrapper : IInfoViewer
    {
        #region IInfoViewer Members

        /// <summary>
        /// Handler für die Messages.
        /// </summary>
        /// <param name="sender">Der Absender der Message</param>
        /// <param name="msgArgs">Die Message mit Message-Object, Typ und Timestamp</param>
        public void HandleInfo(object? sender, InfoArgs msgArgs)
        {
            this._msgHandler(sender, msgArgs);
        }

        #endregion IInfoViewer Members

        /// <summary>
        /// Konstruktor - übernimmt einen geeiggnete EventHandler.
        /// </summary>
        /// <param name="msgHandler">Die Callback-Routine für den Viewer.</param>
        public ViewerAsWrapper(EventHandler<InfoArgs> msgHandler)
        {
            this._msgHandler = msgHandler;
        }

        private EventHandler<InfoArgs> _msgHandler;

    }
}
