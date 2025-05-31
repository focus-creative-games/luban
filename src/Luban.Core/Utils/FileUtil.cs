using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Luban.Utils;

public static class FileUtil
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static string GetApplicationDirectory()
    {
        return AppContext.BaseDirectory;
    }

    public static string GetPathRelateApplicationDirectory(string relatePath)
    {
        return Path.Combine(GetApplicationDirectory(), relatePath);
    }

    public static string GetFileName(string path)
    {
        int index = path.Replace('\\', '/').LastIndexOf('/');
        return index >= 0 ? path[(index + 1)..] : path;
    }

    public static string GetParent(string path)
    {
        int index = path.Replace('\\', '/').LastIndexOf('/');
        return index >= 0 ? path[..index] : ".";
    }

    public static string GetFileNameWithoutExt(string file)
    {
        int index = file.LastIndexOf('.');
        return index >= 0 ? file.Substring(0, index) : file;
    }

    public static string GetFileExtension(string file)
    {
        int index = file.LastIndexOf('.');
        return index >= 0 ? file.Substring(index + 1) : "";
    }

    public static string GetPathRelateRootFile(string rootFile, string file)
    {
        return Combine(GetParent(rootFile), file);
    }

    public static bool IsFileExistsSenseCase(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }
        if (OperatingSystem.IsWindows())
        {
            var fileName = Path.GetFileName(path);
            var files = Directory.GetFiles(Path.GetDirectoryName(path), fileName, new EnumerationOptions() { MatchCasing = MatchCasing.CaseSensitive });
            return files.Length > 0;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    ///  忽略以 文件名以 '.' '_' '~' 开头的文件
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static bool IsValidInputFile(string file)
    {
        if (!File.Exists(file))
        {
            return false;
        }
        var f = new FileInfo(file);
        string fname = f.Name;
        return !fname.StartsWith('.') && !fname.StartsWith('_') && !fname.StartsWith('~');
    }

    public static string CalcMD5(byte[] srcBytes)
    {
        using MD5 md5 = MD5.Create();
        var md5Bytes = md5.ComputeHash(srcBytes);
        var s = new StringBuilder(md5Bytes.Length * 2);
        foreach (var b in md5Bytes)
        {
            s.Append(b.ToString("X"));
        }
        return s.ToString();
    }

    public static string Standardize(string path)
    {
        return path.Replace('\\', '/');
    }

    public static string Combine(string parent, string sub)
    {
        return Standardize(Path.Combine(parent, sub));
    }

    public static bool IsExcelFile(string fullName)
    {
        return fullName.EndsWith(".csv", StringComparison.Ordinal)
               || fullName.EndsWith(".xls", StringComparison.Ordinal)
               || fullName.EndsWith(".xlsx", StringComparison.Ordinal)
               || fullName.EndsWith(".xlsm", StringComparison.Ordinal);
    }

    public static (string, string) SplitFileAndSheetName(string url)
    {
        int sheetSepIndex = url.IndexOf('@');
        if (sheetSepIndex < 0)
        {
            return (url, null);
        }
        else
        {
            int lastPathSep = url.LastIndexOf('/', sheetSepIndex);
            if (lastPathSep >= 0)
            {
                return (url[0..(lastPathSep + 1)] + url[(sheetSepIndex + 1)..], url[(lastPathSep + 1)..sheetSepIndex]);
            }
            else
            {
                return (url[(sheetSepIndex + 1)..], url[(lastPathSep + 1)..sheetSepIndex]);
            }
        }
    }

    public static async Task SaveFileAsync(string relateDir, string filePath, byte[] content)
    {
        // 调用此接口时，已保证 文件必然是改变的，不用再检查对比文件
        var outputPath = Standardize(relateDir != null ? System.IO.Path.Combine(relateDir, filePath) : filePath);
        Directory.GetParent(outputPath).Create();
        if (File.Exists(outputPath))
        {
            s_logger.Info("[override] {file}", outputPath);
            if (File.GetAttributes(outputPath).HasFlag(FileAttributes.ReadOnly))
            {
                File.SetAttributes(outputPath, FileAttributes.Normal);
            }
        }
        else
        {
            s_logger.Info("[new] {file}", outputPath);
        }

        await WriteAllBytesAsync(outputPath, content);
    }

    public static async Task<byte[]> ReadAllBytesAsync(string file)
    {
        // File.ReadAllBytesAsync 无法读取被打开的excel文件，只好重新实现了一个
        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        long count = fs.Length;
        var bytes = new byte[count];
        int writeIndex = 0;
        while (writeIndex < count)
        {
            int n = await fs.ReadAsync(bytes, writeIndex, (int)count - writeIndex, default);
            writeIndex += n;
        }
        return bytes;
    }

    public static byte[] ReadAllBytes(string file)
    {
        // File.ReadAllBytesAsync 无法读取被打开的excel文件，只好重新实现了一个
        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        long count = fs.Length;
        var bytes = new byte[count];
        int writeIndex = 0;
        while (writeIndex < count)
        {
            int n = fs.Read(bytes, writeIndex, (int)count - writeIndex);
            writeIndex += n;
        }
        return bytes;
    }

    public static bool WriteAllBytes(string file, byte[] bytes)
    {
        if (File.Exists(file))
        {
            var oldBytes = ReadAllBytes(file);
            if (oldBytes.Length == bytes.Length &&
                StructuralComparisons.StructuralEqualityComparer.Equals(bytes, oldBytes))
            {
                return false;
            }
        }
        File.WriteAllBytes(file, bytes);
        return true;
    }

    public static async Task WriteAllBytesAsync(string file, byte[] bytes)
    {
        using var fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        long count = bytes.LongLength;
        fs.SetLength(count);
        fs.Seek(0, SeekOrigin.Begin);
        await fs.WriteAsync(bytes);
    }

    public static void DeleteDirectoryRecursive(string rootDir)
    {
        string[] files = Directory.GetFiles(rootDir);
        string[] dirs = Directory.GetDirectories(rootDir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectoryRecursive(dir);
        }

        Directory.Delete(rootDir, false);
    }

    public static bool IsIgnoreFile(string baseDir, string file)
    {
        baseDir = Path.GetFullPath(baseDir.Replace('\\', '/'));
        file = Path.GetFullPath(file.Replace('\\', '/'));
        // if baseDir contains '.' or '_', we shouldn't ignore it.
        if (file.Length > baseDir.Length && file.StartsWith(baseDir) && (file[baseDir.Length] == '\\' || file[baseDir.Length] == '/'))
        {
            file = file[(baseDir.Length + 1)..];
        }
        return file.Split('\\', '/').Any(fileName => fileName.StartsWith(".") || fileName.StartsWith("_") || fileName.StartsWith("~"));
    }

    public static List<string> GetFileOrDirectory(string baseDir, string fileOrDirectory)
    {
        var files = new List<string>();
        if (Directory.Exists(fileOrDirectory))
        {
            foreach (var file in Directory.GetFiles(fileOrDirectory, "*", SearchOption.AllDirectories))
            {
                if (IsIgnoreFile(baseDir, file))
                {
                    continue;
                }
                if (file.EndsWith(".meta", StringComparison.Ordinal))
                {
                    continue;
                }
                files.Add(file.Replace('\\', '/'));
            }
            // must sort files for making generation stable.
            files.Sort(string.CompareOrdinal);
        }
        else
        {
            files.Add(fileOrDirectory);
        }
        return files;
    }

    public static string GetExtensionWithoutDot(string file)
    {
        string ext = Path.GetExtension(file);
        if (string.IsNullOrEmpty(ext))
        {
            return "";
        }
        return ext.Substring(1);
    }
}
