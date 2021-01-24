namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Bildet die Sicht auf den InfoController ab,
    ///           die das Abschicken von Nachrichten umfasst.
    /// </summary>
    /// <remarks>
    /// File: IInfoPublisher.cs<br></br>
    /// Autor: Erik Nagel, NetEti<br></br>
    ///<br></br>
    /// 08.03.2012 Erik Nagel: erstellt<br></br>
    /// </remarks>
    public interface IInfoPublisher: IFlushable
    {
        /// <summary>
        /// Hierüber wird einen neue Message verbreitet.
        /// </summary>
        /// <param name="sender">Der Absender der Nachricht</param>
        /// <param name="msg">Das zu verbreitende (Message-)Objekt</param>
        /// <param name="triggerInfoType">Der Message-Typ, z.B.: InfoType.Milestone</param>
        void Publish(object sender, object msg, InfoType triggerInfoType);

        /// <summary>
        /// Hierüber wird eine neue Message des Typs InfoType.Info verbreitet.
        /// </summary>
        /// <param name="sender">Der Absender der Nachricht</param>
        /// <param name="msg">Das zu verbreitende (Message-)Objekt</param>
        void Publish(object sender, object msg);

        /// <summary>
        /// Hierüber wird eine neue Message des Typs InfoType.Info mit Absender null verbreitet.
        /// </summary>
        /// <param name="msg">Das zu verbreitende (Message-)Objekt</param>
        void Publish(object msg);
    }
}
