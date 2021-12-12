using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using System.Collections.Generic;
using System.Text;

namespace Luban.Job.Cfg.DataVisitors
{
    class ToStringVisitor : ToLiteralVisitorBase
    {
        public static ToStringVisitor Ins { get; } = new ToStringVisitor();

        public override string Accept(DText type)
        {
            return $"\"{type.Key}#{type.TextOfCurrentAssembly}\"";
        }

        public override string Accept(DBean type)
        {
            var x = new StringBuilder();
            if (type.Type.IsAbstractType)
            {
                x.Append($"{{ _name:\"{type.ImplType.Name}\",");
            }
            else
            {
                x.Append('{');
            }

            int index = 0;
            foreach (var f in type.Fields)
            {
                var defField = type.ImplType.HierarchyFields[index++];
                x.Append(defField.Name).Append(':');
                if (f != null)
                {
                    x.Append(f.Apply(this));
                }
                else
                {
                    x.Append("null");
                }
                x.Append(',');
            }
            x.Append('}');
            return x.ToString();
        }


        protected void Append(List<DType> datas, StringBuilder x)
        {
            x.Append('[');
            foreach (var e in datas)
            {
                x.Append(e.Apply(this)).Append(',');
            }
            x.Append(']');
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
                x.Append('"').Append(e.Key.ToString()).Append('"');
                x.Append(':');
                x.Append(e.Value.Apply(this)).Append(',');
            }
            x.Append('}');
            return x.ToString();
        }

        public override string Accept(DVector2 type)
        {
            var v = type.Value;
            return $"{{x:{v.X},y:{v.Y}}}";
        }

        public override string Accept(DVector3 type)
        {
            var v = type.Value;
            return $"{{x:{v.X},y:{v.Y},z:{v.Z}}}";
        }

        public override string Accept(DVector4 type)
        {
            var v = type.Value;
            return $"{{x:{v.X},y:{v.Y},z:{v.Z},w:{v.W}}}";
        }
    }
}
