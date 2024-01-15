using NetEti.ApplicationControl;

namespace LoggerStressTest
{
    internal class Program
    {
        private static IInfoController? _publisher;
        private static Logger? _logger;

        static void Main(string[] args)
        {
            _publisher = InfoController.GetInfoController();

            // Globales Logging installieren
            InfoType[] loggerInfos = InfoTypes.Collection2InfoTypeArray(InfoTypes.All);

            string loggingRegexFilter = ""; // Alles wird geloggt (ist der Default).
                                            //string loggingRegexFilter = @"(?:_NOPPES_)"; // Nichts wird geloggt, bzw. nur Zeilen, die "_NOPPES_" enthalten.
            _logger = new Logger(false, loggingRegexFilter);
            _publisher.RegisterInfoReceiver(_logger, loggerInfos);

            MainLoop();
        }

        private static void MainLoop()
        {
            int waitingLoopCounter = 0;
            while (true)
            {
                Thread.Sleep(100);
                if (waitingLoopCounter++ > 20)
                {
                    //if (this is NodeParent || this.LogicalState == NodeLogicalState.Done)
                    //{
                    //    this.State = NodeState.None; // TODO: State-Mischmasch sauber lösen.
                    //}
                    //else
                    //{
                        // Hier nur loggen, kein Abbruch mit Exception.
                        InfoController.GetInfoPublisher().Publish("Test",
                            String.Format($"InternalError Id/Name: {"Harry"}, State: {"Hirsch"}, LogicalState: Fault"),
                            InfoType.NoRegex
                        );
                        //this.State = NodeState.InternalError;
                    //}
                }
            }
        }
    }
}
