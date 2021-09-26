using Luban.Common.Protos;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Luban.Server.Common
{
    public interface IAgent
    {
        Task<byte[]> GetFromCacheOrReadAllBytesAsync(string file, string md5);

        Task<byte[]> ReadAllBytesAsync(string file);

        Task<GetImportFileOrDirectoryRes> GetFileOrDirectoryAsync(string file, params string[] searchPatterns);

        Task<QueryFilesExistsRes> QueryFileExistsAsync(QueryFilesExistsArg arg);

        Task<XElement> OpenXmlAsync(string xmlFile);

        void Error(string fmt, params object[] objs);

        void Info(string fmt, params object[] objs);
    }
}
