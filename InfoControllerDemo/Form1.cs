using System;
using System.Windows.Forms;
using System.Threading;
using NetEti.Globals;
using NetEti.ApplicationControl;

namespace NetEti.DemoApplications
{
    /// <summary>
    /// Demo für den InfoController und Hilfsklassen
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            this.initialize();
        }

        #region private members

        private IInfoController _publisher;
        private Logger _logger;
        private Exceptioner _exceptioner;

        private ViewerAsWrapper _tbxMessagesViewer;
        private ViewerAsWrapper _lblMessageViewer;
        private Logger _logger2;

        private void initialize()
        {
            this._publisher = InfoController.GetInfoController();

            // Globales Logging installieren
            InfoType[] loggerInfos = InfoTypes.Collection2InfoTypeArray(InfoTypes.All);

            string loggingRegexFilter = ""; // Alles wird geloggt (ist der Default).
                                            //string loggingRegexFilter = @"(?:_NOPPES_)"; // Nichts wird geloggt, bzw. nur Zeilen, die "_NOPPES_" enthalten.
            this._logger = new Logger(false, loggingRegexFilter);
            this._publisher.RegisterInfoReceiver(this._logger, loggerInfos);

            string loggingRegexFilter2 = "@(?:#xxx#)"; // Alles wird geloggt (ist der Default).
                                            //string loggingRegexFilter = @"(?:_NOPPES_)"; // Nichts wird geloggt, bzw. nur Zeilen, die "_NOPPES_" enthalten.
            this._logger2 = new Logger("replace.log", loggingRegexFilter2, true);
            this._publisher.RegisterInfoReceiver(this._logger2, loggerInfos);

            this._exceptioner = GenericSingletonProvider.GetInstance<Exceptioner>();
            this._publisher.RegisterInfoReceiver(this._exceptioner, typeof(Exception), loggerInfos);
            this._lblMessageViewer = new ViewerAsWrapper(this.handleMsgForLabel);
            this._tbxMessagesViewer = new ViewerAsWrapper(this.handleMsgForTextBox);
            this._publisher.RegisterInfoReceiver(this._lblMessageViewer, new[] { InfoType.Info });
        }

        private void handleMsgForTextBox(object sender, InfoArgs msgArgs)
        {
            this.tbxMessages.Text += (Environment.NewLine + msgArgs.MessageObject.ToString());
        }

        private void handleMsgForLabel(object sender, InfoArgs msgArgs)
        {
            this.lblMessage.Text = msgArgs.MessageObject.ToString();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            this.generateMessages();
        }

        private void generateMessages()
        {
            this._publisher.RegisterInfoReceiver(this._tbxMessagesViewer, new[] { InfoType.Info });
            this._publisher.Publish(this, "Message 1");
            Thread.Sleep(300);
            Application.DoEvents();
            Thread.Sleep(300);
            this._publisher.Publish(this, "Message 2");
            Thread.Sleep(300);
            Application.DoEvents();
            Thread.Sleep(300);
            this._publisher.Publish(this, "Message 3a" + Environment.NewLine + "Message 3b" + Environment.NewLine + "Message 3c");
            Thread.Sleep(300);
            Application.DoEvents();
            this._publisher.UnregisterInfoReceiver(this._tbxMessagesViewer);
            Thread.Sleep(300);
            this._publisher.Publish(this, "Message 4");
            Thread.Sleep(300);
            this._publisher.Publish(this, "#xxx# Message 4a");
            Thread.Sleep(300);
            Application.DoEvents();
            Thread.Sleep(300);
            this._publisher.Publish(this, "Message 5");
            Thread.Sleep(300);
            Application.DoEvents();

            int a = 0;
            int b = 10;
            try
            {
                int c = b / a;
            }
            catch (Exception ex)
            {
                this._publisher.Publish(this, new ExtendedException("Fehler am Wassergraben: ", ex, "hahahaha"));
                Thread.Sleep(300);
                Application.DoEvents();
            }
        }

        #endregion private members

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (this._logger != null)
            //{
            //    this._logger.Dispose();
            //}
            InfoController.GetInfoController().Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string log = @"CutLogTest.log";
            LoggerBase.CutLog(log, 1, 3, false);
        }

    }
}
