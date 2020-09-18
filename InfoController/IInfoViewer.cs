using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetEti.ApplicationControl
{
    /// <summary>
    /// Muss von einem Viewer implementiert werden, der sich beim
    /// InfoController (IInfoSource) anmelden will.
    /// </summary>
    /// <remarks>
    /// File: IInfoViewer<br></br>
    /// Autor: Erik Nagel, NetEti<br></br>
    ///<br></br>
    /// 19.03.2012 Erik Nagel: erstellt<br></br>
    /// </remarks>
    public interface IInfoViewer
    {
        /// <summary>
        /// Hierüber verarbeitet der Viewer eine Information.
        /// </summary>
        /// <param name="sender">Der Absender der Information.</param>
        /// <param name="msgArgs">Erweiterte Informationen.</param>
        void HandleInfo(object sender, InfoArgs msgArgs);
    }
}
