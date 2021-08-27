using Bright.Net.ServiceModes.Managers;
using Luban.Common.Protos;
using Luban.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
