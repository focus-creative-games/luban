using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class PyDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static PyDeserializeVisitor Py3Ins { get; } = new PyDeserializeVisitor(true);


        public static PyDeserializeVisitor Py27Ins { get; } = new PyDeserializeVisitor(false);

        public PyDeserializeVisitor(bool py3)
        {
            Python3 = py3;

            UnderringVisitor = py3 ? PyUnderingDeserializeVisitor.Py3Ins : PyUnderingDeserializeVisitor.Py27Ins;
        }

        public bool Python3 { get; }

        PyUnderingDeserializeVisitor UnderringVisitor { get; }

        public override string DoAccept(TType type, string jsonFieldName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"if {jsonFieldName} != None: {type.Apply(UnderringVisitor, jsonFieldName, fieldName)}";
            }
            else
            {
                return type.Apply(UnderringVisitor, jsonFieldName, fieldName);
            }
        }
    }
}
