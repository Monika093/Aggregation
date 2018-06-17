using System.IO;
using System.Text;

namespace SimpleAggregation
{
    class Logger : ILogger
    {
        private StringBuilder logsb;
        public Logger()
        {
            logsb = new StringBuilder();
            logsb.AppendLine("Datafile" + ",Number of Line" + ",Text");
        }
        string mydocpath = @"C:\Users\Atanas Trpceski\Desktop\Monika Excersises\ErrorParsingLogger.csv";

        public void WriteLine(string message, long lineNumber)
        {
            logsb.AppendLine("household_power_consumption_OutputFile.csv" + "," + lineNumber + "," + message);
        }

        public void WriteToFile()
        {
            File.WriteAllText(mydocpath, logsb.ToString());
        }
    }
    public interface ILogger
    {
        void WriteLine(string message, long lineNumber);
        void WriteToFile();
    }
}
