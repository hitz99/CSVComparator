using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
namespace CSVComparatorTool
{
    public class CsvComparer
    {
        Logger LogMessage = new Logger();

        /// <summary>
        /// Compares the actualFile csv with benchmark csv file with primary key KeyColumn and create new csv where deifference is stored.
        /// </summary>
        /// <param name="actualFile">benchmark csv file</param>
        /// <param name="expectedFile">actual csv file</param>
        /// <param name="resultFile">result file where the difference is stored.</param>
        /// <param name="keyColumn">Unique key in csv file</param>
        public void CompareCSV(string actualFile, string expectedFile, string resultFile, string keyColumn)
        {
            try
            {
                LogMessage.Log("Starting CSV comparison from file " + actualFile + " with " + expectedFile + "");
                LogMessage.Log("Getting the data from file " + actualFile);
                Dictionary<string, List<string>> benchmarkData = GetDataFromCSV(actualFile, keyColumn, out string[]? headers);
                LogMessage.Log("Getting the data from file " + expectedFile);
                Dictionary<string, List<string>> actualFileData = GetDataFromCSV(expectedFile, keyColumn, out _); // Ignore headers from target file
                LogMessage.Log("Comparing the files.");
                CompareAndSave(benchmarkData, actualFileData, resultFile, headers);
                LogMessage.Log("Comparison completed successfully. Results saved in " + resultFile);
                Console.WriteLine("Comparison finished. Please check results in " + resultFile);
            }
            catch (Exception exception)
            {
                LogMessage.Log("Error: " + exception.Message);
                Console.WriteLine("Error while comparing the files please check the log file for details.");
            }
        }

        /// <summary>
        /// Reads a CSV file and stores data in a dictionary with key as a specified key column.
        /// </summary>
        /// <param name="filePath">Path of csv file</param>
        /// <param name="keyColumn">Primary key</param>
        /// <param name="headers">this is out parameter no need to assign any value.</param>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetDataFromCSV(string filePath, string keyColumn, out string[]? headers)
        {
            Dictionary<string, List<string>> records = new Dictionary<string, List<string>>();
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    headers = reader.ReadLine()?.Split(',');
                    if (headers == null)
                    {
                        LogMessage.Log("Error: No headers found in CSV " + filePath);
                        throw new Exception($"Invalid CSV format: {filePath}");
                    }
                    int keyIndex = Array.IndexOf(headers, keyColumn);
                    if (keyIndex == -1)
                    {
                        LogMessage.Log("Error: No Primary key column found with name " + keyColumn);
                        throw new Exception($"Key column '{keyColumn}' not found in {filePath}");
                    }
                    while (!reader.EndOfStream)
                    {
                        string[]? line = reader.ReadLine()?.Split(',');
                        if (line == null || line.Length != headers.Length) continue;

                        string key = line[keyIndex];
                        records[key] = new List<string>(line);
                    }
                }

                return records;
            }
            catch (Exception exception)
            {
                LogMessage.Log("Error: " + exception.Message);
                Console.WriteLine("Error while getting records from files please check the log file for details.");
                headers = null;
                return records;
            }
        }

        /// <summary>
        /// Compares the the data from Dictionary and write the difference in outfile file csv
        /// </summary>
        /// <param name="sourceData">Benchmark file data</param>
        /// <param name="targetData">Actual file data</param>
        /// <param name="outputFile">CSV file to write the difference.</param>
        /// <param name="headers">headers of csv to add in output csv file.</param>
        public void CompareAndSave(Dictionary<string, List<string>> actualFile,
                                   Dictionary<string, List<string>> expectedFile,
                                   string outputFile,
                                   string[] headers)
        {
            try
            {
                if (File.Exists(outputFile))
                {
                    string newFileName = $"{Path.GetFileNameWithoutExtension(outputFile)}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}{Path.GetExtension(outputFile)}";
                    outputFile = Path.Combine(Path.GetDirectoryName(outputFile), newFileName);
                    File.Create(outputFile).Dispose();
                }

                bool isEqual = true;
                using (var writer = new StreamWriter(outputFile))
                {
                    LogMessage.Log("Writing the CSV headers.");
                    writer.WriteLine($"ChangeType,{string.Join(",", headers)}");

                    LogMessage.Log("Writing the Not found/Removed records.");
                    foreach (var key in actualFile.Keys)
                    {
                        if (!expectedFile.ContainsKey(key))
                        {
                            isEqual = false;
                            writer.WriteLine($"Removed,{string.Join(",", actualFile[key])}");
                            
                        }
                    }

                    LogMessage.Log("Writing the extra/Added and modified records.");
                    foreach (var key in expectedFile.Keys)
                    {
                        if (!actualFile.ContainsKey(key))
                        {
                            isEqual = false;
                            writer.WriteLine($"Added,{string.Join(",", expectedFile[key])}");
                        }
                        else if (!actualFile[key].SequenceEqual(expectedFile[key]))
                        {
                            isEqual = false;
                            writer.WriteLine($"Modified,{string.Join(",", expectedFile[key])}");
                            //Added code to fullfill case study requirement ..to add log with actual mismatched value.
                            for (int i = 0; i < actualFile[key].Count; i++)
                            {
                                if (actualFile[key][i] != expectedFile[key][i])
                                {
                                    LogMessage.Log("Fieldname: " + headers[i] + " | Expected Input Value: \"" + actualFile[key][i] + "\" | Actual Value: \"" + expectedFile[key][i] + "\" | for record having unique field Name: " + key);
                                }
                            }
                        }
                    }
                }
                if (isEqual)
                {
                    LogMessage.Log("Files are identical");
                    Console.WriteLine("Files are identical");
                }
                else
                {
                    LogMessage.Log("Files are not identical,Please view output csv file for changes " + outputFile);
                    Console.WriteLine("Files are not identical");
                }
            }
            catch (Exception exception)
            {
                LogMessage.Log("Error: " + exception.Message);
                Console.WriteLine("Error while comparing the files.See log file for more details." + outputFile);
            }
        }

    }
}
