using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using System.Collections.Generic;
using System.Text;

namespace Luban.Job.Cfg.DataVisitors
{
    class ToLuaLiteralVisitor : ToLiteralVisitorBase
    {
        public static ToLuaLiteralVisitor Ins { get; } = new();

        public override string Accept(DString type)
        {
            return DataUtil.EscapeLuaStringWithQuote(type.Value);
        }

        public override string Accept(DText type)
        {
            return $"{{{DText.KEY_NAME}='{type.Key}',{DText.TEXT_NAME}={DataUtil.EscapeLuaStringWithQuote(type.TextOfCurrentAssembly)}}}";
        }

        public override string Accept(DBean type)
        {
            var x = new StringBuilder();
            if (type.Type.IsAbstractType)
            {
                x.Append($"{{ {DefBean.LUA_TYPE_NAME_KEY}='{DataUtil.GetImplTypeName(type)}',");
            }
            else
            {
                x.Append('{');
            }

            int index = 0;
            foreach (var f in type.Fields)
            {
                var defField = (DefField)type.ImplType.HierarchyFields[index++];
                if (f == null || !defField.NeedExport)
                {
                    continue;
                }
                x.Append(defField.Name).Append('=');
                x.Append(f.Apply(this));
                x.Append(',');
            }
            x.Append('}');
            return x.ToString();
        }


        private void Append(List<DType> datas, StringBuilder x)
        {
            x.Append('{');
            foreach (var e in datas)
            {
                x.Append(e.Apply(this));
                x.Append(',');
            }
            x.Append('}');
        }

        public override string Accept(DArray type)
        {
            var x = new StringBuilder();
            Append(type.Datas, x);
            return x.ToString();
        }

        public override string Accept(DList type)
        {
            var x = new StringBuilder();
            Append(type.Datas, x);
            return x.ToString();
        }

        public override string Accept(DSet type)
        {
            var x = new StringBuilder();
            Append(type.Datas, x);
            return x.ToString();
        }

        public override string Accept(DMap type)
        {
            var x = new StringBuilder();
            x.Append('{');
            foreach (var e in type.Datas)
            {
                x.Append('[');
                x.Append(e.Key.Apply(this));
                x.Append(']');
                x.Append('=');
                x.Append(e.Value.Apply(this));
                x.Append(',');
            }
            x.Append('}');
            return x.ToString();
        }

        public override string Accept(DVector2 type)
        {
            var v = type.Value;
            return $"{{x={v.X},y={v.Y}}}";
        }

        public override string Accept(DVector3 type)
        {
            var v = type.Value;
            return $"{{x={v.X},y={v.Y},z={v.Z}}}";
        }

        public override string Accept(DVector4 type)
        {
            var v = type.Value;
            return $"{{x={v.X},y={v.Y},z={v.Z},w={v.W}}}";
        }
    }
}
