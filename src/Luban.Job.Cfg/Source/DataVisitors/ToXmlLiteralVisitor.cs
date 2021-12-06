using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using System.Collections.Generic;
using System.Text;

namespace Luban.Job.Cfg.DataVisitors
{
    class ToXmlLiteralVisitor : ToJsonLiteralVisitor
    {
        public static new ToXmlLiteralVisitor Ins { get; } = new();

        public override string Accept(DText type)
        {
            return $"{{{DText.KEY_NAME}='{type.Key}',{DText.TEXT_NAME}='{DataUtil.EscapeString(type.TextOfCurrentAssembly)}'}}";
        }

        public override string Accept(DBean type)
        {
            var x = new StringBuilder();
            var bean = type.ImplType;
            if (bean.IsAbstractType)
            {
                x.Append($"{{ _name='{type.ImplType.Name}',");
            }
            else
            {
                x.Append('{');
            }

            int index = 0;
            foreach (var f in type.Fields)
            {
                var defField = type.ImplType.HierarchyFields[index++];
                x.Append(defField.Name).Append('=');
                if (f != null)
                {
                    x.Append(f.Apply(this));
                }
                else
                {
                    x.Append("nil");
                }
                x.Append(',');
            }
            x.Append('}');
            return x.ToString();
        }


        override protected void Append(List<DType> datas, StringBuilder x)
        {
            x.Append('{');
            foreach (var e in datas)
            {
                x.Append(e.Apply(this));
                x.Append(',');
            }
            x.Append('}');
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
