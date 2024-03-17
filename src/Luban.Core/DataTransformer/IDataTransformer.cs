using Luban.Datas;
using Luban.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataTransformer;

public interface IDataTransformer
{
    DType Transform(DType originalData, TType type);
}
