using Luban.Common.Protos;
using System.Threading.Tasks;

namespace Luban.Server.Common
{
    public interface IJobController
    {
        Task GenAsync(RemoteAgent agent, GenJob rpc);
    }
}
