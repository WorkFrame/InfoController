namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Bildet die komplette Sicht auf den InfoController ab,
    ///           umfasst das Abschicken und Empfangen von Nachrichten.
    /// </summary>
    /// <remarks>
    /// File: IInfoController<br></br>
    /// Autor: Erik Nagel, NetEti<br></br>
    ///<br></br>
    /// 08.03.2012 Erik Nagel: erstellt<br></br>
    /// </remarks>
    public interface IInfoController : IInfoPublisher, IInfoSource, IDisposable
    {
    }
}
