using Bright.Net.ServiceModes.Managers;
using Luban.Common.Utils;
using System.Threading.Tasks;

namespace Luban.Server.Common
{
    public class LocalAgent : RemoteAgent
    {
        public LocalAgent(SessionBase session) : base(session)
        {
        }

        public override Task<byte[]> ReadAllBytesAsync(string file)
        {
            return FileUtil.ReadAllBytesAsync(file);
        }
    }
}
