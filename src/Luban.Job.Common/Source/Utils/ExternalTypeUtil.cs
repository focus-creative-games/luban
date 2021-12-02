using Luban.Job.Common.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.Utils
{
    public static class ExternalTypeUtil
    {
        public static string CsMapperToExternalType(DefTypeBase type)
        {
            var mapper = type.CurrentExternalTypeMapper;
            return mapper != null ? mapper.TypeName : type.CsFullName;
        }

        public static string CsCloneToExternal(DefTypeBase type, string src)
        {
            var mapper = type.CurrentExternalTypeMapper;
            if (mapper == null)
            {
                return src;
            }
            if (string.IsNullOrWhiteSpace(mapper.CreateExternalObjectFunction))
            {
                throw new Exception($"type:{type.FullName} externaltype:{type.ExternalType.Name} lan:{mapper.Lan} selector:{mapper.Selector} 未定义clone_to_external元素");
            }
            return $"{mapper.CreateExternalObjectFunction}({src})";
        }
    }
}
