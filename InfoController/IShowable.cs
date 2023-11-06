namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Zeigt an, dass die implementierende Instanz eine parameterlose Methode zur Ausgabe bietet;
    /// bei Loggern z.B. zum Ausgeben des Logs.
    /// </summary>
    /// <remarks>
    /// Autor: Erik Nagel, NetEti
    ///
    /// 06.11.2023 Erik Nagel: erstellt.
    /// </remarks>
    public interface IShowable
    {
        /// <summary>
        /// Sorgt dafür, dass die implementierende Instanz ausgegeben/angezeigt wird.
        /// </summary>
        void Show();
    }
}
