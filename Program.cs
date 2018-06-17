using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAggregation
{
    public class Aggragate
    {
        public double value;
        public int count;
    }

    static class Program
    {
        static void Main(string[] args)
        {
            var pathIn = @"C:\Users\Atanas Trpceski\Desktop\Monika Excersises\household_power_consumption.csv";
            var pathOut = @"C:\Users\Atanas Trpceski\Desktop\Monika Excersises\household_power_consumption_OutputFile.csv";

            Stopwatch sw = new Stopwatch();
            sw.Start();
            ExamineFile(pathIn, pathOut);
            sw.Stop();
            Console.WriteLine(sw.Elapsed.Milliseconds);
            Console.ReadLine();
        }

        public static void ExamineFile(string pathIn, string pathOut)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                ILogger _log = new Logger();
                var activePowerRecords = new ConcurrentDictionary<DateTime, Aggragate>();

                Parallel.ForEach(File.ReadLines(pathIn), (line, _, lineNumber) =>
                {
                    if (lineNumber == 0)
                        return;

                    var newline = line.Split(',');
                    var dateString = newline[0];
                    DateTime date;
                    if (!DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        _log.WriteLine(dateString, lineNumber);
                    }

                    var global_active_power_value_string = newline[2];
                    double global_active_power_value;
                    if (double.TryParse(global_active_power_value_string, out global_active_power_value))
                    {
                        _log.WriteLine(global_active_power_value_string, lineNumber);
                    }

                    var global_reactive_power_value_string = newline[3];
                    double global_reactive_power_value;
                    if (!double.TryParse(global_reactive_power_value_string, out global_reactive_power_value))
                    {
                        _log.WriteLine(global_reactive_power_value_string, lineNumber);
                    }

                    if (global_active_power_value > 2.5)
                    {
                        if (activePowerRecords.ContainsKey(date))
                        {
                            activePowerRecords[date].value += global_reactive_power_value;
                            var keyForNumberOfDatapoints = activePowerRecords[date];
                            keyForNumberOfDatapoints.count++;
                        }
                        else
                        {
                            activePowerRecords.TryAdd(date,
                                new Aggragate { count = 1, value = global_reactive_power_value });
                        }
                    }
                });

                var sortedList = activePowerRecords.Keys.ToList();
                sortedList.Sort();

                sb.AppendLine("Month/Year,  Number of datapoints ,  Sum of values");
                foreach (var item in sortedList)
                {
                    var datemonth = String.Format("{0:00}", item.Month);
                    var dateyear = String.Format("{0:0000}", item.Year);
                    sb.AppendLine(datemonth + "/" + dateyear + ", " + activePowerRecords[item].count + ", " +
                                  activePowerRecords[item].value + ",");
                }

                _log.WriteToFile();
                File.WriteAllText(pathOut, sb.ToString());
            }

            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
    }
}

