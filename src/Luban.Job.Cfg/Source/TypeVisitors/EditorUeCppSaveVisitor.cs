using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class EditorUeCppSaveVisitor : ITypeFuncVisitor<string, string, string>
    {

        public static EditorUeCppSaveVisitor Ins { get; } = new EditorUeCppSaveVisitor();

        private string DefaultSave(string json, string field)
        {
            return $"if (!JsonUtil::Write({json}, \"{field}\", this->{field})) {{ delete {json}; return false; }}";
        }

        public string Accept(TBool type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TByte type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TShort type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TFshort type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TInt type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TFint type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TLong type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TFlong type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TFloat type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TDouble type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TEnum type, string json, string field)
        {
            //return $"{{FString _enumStr_; if(!{type.DefineEnum.UeFname}ToString(this->{field}, _enumStr_)) {{ return false;}}  if(!JsonUtil::Write({json}, \"{field}\", _enumStr_)) {{ return false;}}  }}";
            throw new NotImplementedException();
        }

        public string Accept(TString type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TBytes type, string json, string field)
        {
            throw new NotImplementedException();
        }

        public string Accept(TText type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TBean type, string json, string field)
        {
            if (type.Bean.IsAbstractType)
            {

                return $"if (!JsonUtil::WriteDynamicBean({json}, \"{field}\", this->{field})) {{ delete {json}; return false;  }}";
            }
            else
            {

                return $"if (!JsonUtil::WriteBean({json}, \"{field}\", this->{field})) {{ delete {json}; return false;  }}";
            }
        }

        private string SaveList(TType elementType, string json, string field)
        {
            return $@"
            {{
                TArray<TSharedPtr<FJsonValue>> _arr;
                for (auto _v : this->{field})
                {{
                    {elementType.Apply(EditorUeCppTypeJsonValueTypeNameVisitor.Ins)}* _vj = nullptr;
                    {elementType.Apply(EditorUeCppSaveVisitor2.Ins, "_vj", "_v")}
                    _arr.Add(TSharedPtr<FJsonValue>(_vj));
                }}
                {json}->SetArrayField(""{field}"", _arr);
            }}
";
        }

        public string Accept(TArray type, string json, string field)
        {
            return SaveList(type.ElementType, json, field);
        }

        public string Accept(TList type, string json, string field)
        {
            return SaveList(type.ElementType, json, field);
        }

        public string Accept(TSet type, string json, string field)
        {
            return SaveList(type.ElementType, json, field);
        }

        public string Accept(TMap type, string json, string field)
        {
            return $@"
            {{
                TArray<TSharedPtr<FJsonValue>> _arr;
                for (auto& _e : this->{field})
                {{
                    TArray<TSharedPtr<FJsonValue>> _entry;
                    {type.KeyType.Apply(EditorUeCppTypeJsonValueTypeNameVisitor.Ins)}* _kj = nullptr;
                    {type.KeyType.Apply(EditorUeCppSaveVisitor2.Ins, "_kj", "_e.Key")}
                    _entry.Add(TSharedPtr<FJsonValue>(_kj));

                    {type.ValueType.Apply(EditorUeCppTypeJsonValueTypeNameVisitor.Ins)}* _vj = nullptr;
                    {type.ValueType.Apply(EditorUeCppSaveVisitor2.Ins, "_vj", "_e.Value")}
                    _entry.Add(TSharedPtr<FJsonValue>(_vj));

                    _arr.Add(TSharedPtr<FJsonValue>(new FJsonValueArray(_entry)));
                }}
                {json}->SetArrayField(""{field}"", _arr);
            }}
";
        }

        public string Accept(TVector2 type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TVector3 type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TVector4 type, string json, string field)
        {
            return DefaultSave(json, field);
        }

        public string Accept(TDateTime type, string json, string field)
        {
            return DefaultSave(json, field);
        }
    }
}
