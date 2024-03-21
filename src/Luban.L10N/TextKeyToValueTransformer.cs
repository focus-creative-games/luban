using Luban.Datas;
using Luban.DataTransformer;
using Luban.DataVisitors;
using Luban.Types;
using Luban.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.L10N;

public class TextKeyToValueTransformer : DataTransfomerBase, IDataFuncVisitor2<DType>
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly ITextProvider _provider;

    public TextKeyToValueTransformer(ITextProvider provider)
    {
        _provider = provider;
    }

    DType IDataFuncVisitor2<DType>.Accept(DString data, TType type)
    {
        if(string.IsNullOrEmpty(data.Value) || !type.HasTag("text"))
        {
            return data;
        }
        if (_provider.TryGetText(data.Value, out var text))
        {
            return DString.ValueOf(type, text);
        }
        s_logger.Error("can't find target language text of text id:{} ", data.Value);
        //_provider.AddUnknownKey(data.Value);
        return data;
    }
}
