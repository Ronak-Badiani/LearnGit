/// <summary>
/// Program to precheck the keys before pulling data from upstream  
/// </summary>
namespace PreCheckUtility
{
    using Logger;
    using System;

    class Program
    {
        internal static string errorValue;
        internal static ILogger loggerDetails = InitializeLogger.GetEventLogger(new EventLoggerConfig(Constants.APPLICATION, Constants.ARGUMENTS, Constants.EVENT_ID)); // EventViewer returns error with ID 108
        internal static string logFilePath = string.Concat(Environment.CurrentDirectory, Constants.LOG_FILE_PATH);

        static int Main(string[] args)
        {
            Helper.SetOutputWindow(logFilePath);
            if (Helper.ReadXML() == 1)
            {
                return 1;
            }
            return 0;
        }
    }
}