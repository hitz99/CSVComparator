using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVComparatorTool
{
    public class Logger
    {
        string filePath = "C:\\SeleniumAssignment\\CSVComparator\\CSVComparisonLogs.txt";
        public Logger()
        {

            if (File.Exists(filePath))
            {
                // if file with same name exist then append the timestanp and create new file .
                string newFileName = $"{Path.GetFileNameWithoutExtension(filePath)}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}{Path.GetExtension(filePath)}";
                filePath = Path.Combine(Path.GetDirectoryName(filePath), newFileName);
                File.Create(filePath).Dispose();
            }
            else
            {
                // no file exist so create new one.
                File.Create(filePath).Dispose();
            }
        }
        public void Log(string message)
        {
            using (var writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            }
        }
    }
}
