using CommandLine;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2TextDiff
{
    class Excel2TextWriter
    {
        class CommandLineOptions
        {
            [Option('p', "port", Required = false, HelpText = "listen port")]
            public int Port { get; set; } = 8899;
        }

        public void TransformToTextAndSave(string excelFile, string outputTextFile)
        {
            var lines = new List<string>();
            using var excelFileStream = new FileStream(excelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            string ext = Path.GetExtension(excelFile);
            using (var reader = ext != ".csv" ? ExcelReaderFactory.CreateReader(excelFileStream) : ExcelReaderFactory.CreateCsvReader(excelFileStream))
            {
                do
                {
                    lines.Add($"===[{reader.Name ?? ""}]===");
                    LoadRemainRows(reader, lines);
                } while (reader.NextResult());
            }
            File.WriteAllLines(outputTextFile, lines, System.Text.Encoding.UTF8);
        }

        private void LoadRemainRows(IExcelDataReader reader, List<string> lines)
        {
            int rowIndex = 0;
            while (reader.Read())
            {
                ++rowIndex; // 第一行是 meta ，跳过
                var row = new List<string>();
                for (int i = 0, n = reader.FieldCount; i < n; i++)
                {
                    object cell = reader.GetValue(i);
                    row.Add(cell != null ? cell.ToString() : "");
                }
                int lastNotEmptyIndex = row.FindLastIndex(s => !string.IsNullOrEmpty(s));
                if (lastNotEmptyIndex >= 0)
                {
                    row = row.GetRange(0, lastNotEmptyIndex + 1);
                }
                else
                {
                    row.Clear();
                }
                lines.Add(string.Join(',', row));
            }
        }
    }
}
