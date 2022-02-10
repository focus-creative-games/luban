using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.RawDefs
{

    public class ExternalTypeMapper
    {
        public string Selector { get; set; }

        public ELanguage Lan { get; set; }

        public string TargetTypeName { get; set; }

        public string CreateExternalObjectFunction { get; set; }
    }

    public class ExternalType
    {
        public string Name { get; set; }

        public string OriginTypeName { get; set; }

        public List<ExternalTypeMapper> Mappers { get; set; } = new List<ExternalTypeMapper>();
    }
}
