using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class PyUnderingDeserializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static PyUnderingDeserializeVisitor Py3Ins { get; } = new PyUnderingDeserializeVisitor(true);

        public static PyUnderingDeserializeVisitor Py27Ins { get; } = new PyUnderingDeserializeVisitor(false);

        public PyUnderingDeserializeVisitor(bool py3)
        {
            Python3 = py3;
        }

        public bool Python3 { get; }

        public string Accept(TBool type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TByte type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TShort type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TFshort type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TInt type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TFint type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TLong type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TFlong type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TFloat type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TDouble type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TEnum type, string jsonVarName, string fieldName)
        {
            return Python3 ? $"{fieldName} = {type.DefineEnum.PyFullName}({jsonVarName})" : $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TString type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TBytes type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TText type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }

        public string Accept(TBean type, string jsonVarName, string fieldName)
        {
            if (type.Bean.IsAbstractType)
            {
                return $"{fieldName} = {type.Bean.PyFullName}.fromJson({jsonVarName})";
            }
            else
            {
                return $"{fieldName} = {type.Bean.PyFullName}({jsonVarName})";
            }
        }

        public string Accept(TArray type, string jsonVarName, string fieldName)
        {
            if (type.Apply(SimpleJsonTypeVisitor.Ins))
            {
                return $"{fieldName} = {jsonVarName}";
            }
            else
            {
                return $"{fieldName} = []\n        for _ele in {jsonVarName}: {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.append(_e)";
            }
        }

        public string Accept(TList type, string jsonVarName, string fieldName)
        {
            if (type.Apply(SimpleJsonTypeVisitor.Ins))
            {
                return $"{fieldName} = {jsonVarName}";
            }
            else
            {
                return $"{fieldName} = []\n        for _ele in {jsonVarName}: {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.append(_e)";
            }
        }

        public string Accept(TSet type, string jsonVarName, string fieldName)
        {
            if (type.Apply(SimpleJsonTypeVisitor.Ins))
            {
                return $"{fieldName} = {jsonVarName}";
            }
            else
            {
                return $"{fieldName} = set()\n        for _ele in {jsonVarName}: {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.add(_e)";
            }
        }

        public string Accept(TMap type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {{}}\n        for _ek, _ev in {jsonVarName}: {type.KeyType.Apply(this, "_ek", "_k")}; {type.ValueType.Apply(this, "_ev", "_v")}; {fieldName}[_k] =_v";
        }

        public string Accept(TVector2 type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = Vector2.fromJson({jsonVarName})";
        }

        public string Accept(TVector3 type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = Vector3.fromJson({jsonVarName})";
        }

        public string Accept(TVector4 type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = Vector4.fromJson({jsonVarName})";
        }

        public string Accept(TDateTime type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }
    }
}
