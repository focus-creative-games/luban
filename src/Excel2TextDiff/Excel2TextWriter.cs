using ExcelDataReader;
using System.Collections.Generic;
using System.IO;

namespace Excel2TextDiff
{
    class Excel2TextWriter
    {
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
                    LoadRows(reader, lines);
                } while (reader.NextResult());
            }
            File.WriteAllLines(outputTextFile, lines, System.Text.Encoding.UTF8);
        }

        private void LoadRows(IExcelDataReader reader, List<string> lines)
        {
            var row = new List<string>();
            while (reader.Read())
            {
                row.Clear();
                for (int i = 0, n = reader.FieldCount; i < n; i++)
                {
                    object cell = reader.GetValue(i);
                    row.Add(cell != null ? cell.ToString() : "");
                }
                // 只保留到最后一个非空白单元格
                int lastNotEmptyIndex = row.FindLastIndex(s => !string.IsNullOrEmpty(s));
                if (lastNotEmptyIndex >= 0)
                {
                    lines.Add(string.Join(',', row.GetRange(0, lastNotEmptyIndex + 1)));
                }
                else
                {
                    // 忽略空白行，没必要diff这个
                }
            }
        }
    }
}
