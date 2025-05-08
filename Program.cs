using CSVComparatorTool;
using System;

namespace CSVComparatorTool
{
    public class MainClass
    {
        static void Main(string[] args)
        {
            //If you wish to debug/ pass arguments from program then uncomment the below code.
            //string sourceFile = "C:\\SeleniumAssignment\\CSVComparator\\Benchmark.csv";
            //string targetFile = "C:\\SeleniumAssignment\\CSVComparator\\Test1.csv";
            //string resultFile = "C:\\SeleniumAssignment\\CSVComparator\\comparison_results.csv";
            //string keyColumn = "ID"; // Column used as the unique key


            /**Pass arguments as 1. ActualCSV Path .
                                 2. Expected CSV Path.
                                 3. Result CSV Path.
                                 4. Primary key column name. */

            CsvComparer _obj = new CsvComparer();
            _obj.CompareCSV(args[0], args[1], args[2], args[3]);
        }
    }
}