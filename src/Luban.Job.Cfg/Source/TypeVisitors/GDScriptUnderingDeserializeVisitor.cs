using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.TypeVisitors
{
    class GDScriptUnderingDeserializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static GDScriptUnderingDeserializeVisitor Ins { get; } = new();

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
            return $"{fieldName} = {jsonVarName}";
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
            return $"{fieldName} = {jsonVarName}['{DText.TEXT_NAME}']";
        }

        public string Accept(TBean type, string jsonVarName, string fieldName)
        {
            if (type.Bean.IsAbstractType)
            {
                return $"{fieldName} = {type.Bean.GDScriptFullName}.from_json({jsonVarName})";
            }
            else
            {
                return $"{fieldName} = {type.Bean.GDScriptFullName}.new({jsonVarName})";
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
                return $"{fieldName} = []\n        for _ele in {jsonVarName}:\n            var {type.ElementType.Apply(this, "_ele", "_e")};\n            {fieldName}.append(_e)";
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
                return $"{fieldName} = []\n        for _ele in {jsonVarName}:\n            var {type.ElementType.Apply(this, "_ele", "_e")};\n            {fieldName}.append(_e)";
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
                return $"{fieldName} = set()\n        for _ele in {jsonVarName}:\n            var  {type.ElementType.Apply(this, "_ele", "_e")};\n            {fieldName}.add(_e)";
            }
        }

        public string Accept(TMap type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {{}}\n        for _v in {jsonVarName}:\n            {fieldName}[_v[0]] =_v[1]";
        }

        public string Accept(TVector2 type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = Vector2({jsonVarName}['x'], {jsonVarName}['y'])";
        }

        public string Accept(TVector3 type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = Vector3({jsonVarName}['x'], {jsonVarName}['y'], {jsonVarName}['z'])";
        }

        public string Accept(TVector4 type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = Vector3({jsonVarName}['x'], {jsonVarName}['y'], {jsonVarName}['z'])";
        }

        public string Accept(TDateTime type, string jsonVarName, string fieldName)
        {
            return $"{fieldName} = {jsonVarName}";
        }
    }
}
