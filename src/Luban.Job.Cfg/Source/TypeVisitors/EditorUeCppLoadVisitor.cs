using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class EditorUeCppLoadVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static EditorUeCppLoadVisitor Ins { get; } = new EditorUeCppLoadVisitor();


        private string DefaultLoad(string json, string field)
        {
            return $"if (!JsonUtil::Read({json}, \"{field}\", this->{field})) {{ return false;  }}";
        }

        public string Accept(TBool type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TByte type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TShort type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TFshort type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TInt type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TFint type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TLong type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TFlong type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TFloat type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TDouble type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TEnum type, string json, string field)
        {
            //return $"{{FString _enumStr_;  if(!JsonUtil::Read({json}, \"{field}\", _enumStr_)) {{ return false;  }}  if(!{type.DefineEnum.UeFname}FromString(_enumStr_, this->{field})) {{ return false;}}  }}";
            throw new NotImplementedException();
        }

        public string Accept(TString type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TBytes type, string json, string field)
        {
            throw new NotImplementedException();
        }

        public string Accept(TText type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TBean type, string json, string field)
        {
            if (type.Bean.IsAbstractType)
            {
                return $"if(!JsonUtil::ReadDynamicBean({json}, \"{field}\", this->{field})){{ return false; }}";
            }
            else
            {
                return $"if(!JsonUtil::ReadBean({json}, \"{field}\", this->{field})) {{ return false; }}";
            }
        }

        private string LoadList(TType elementType, string json, string field)
        {
            return $@"
            {{
                const TArray<TSharedPtr<FJsonValue>>* _parr;
                if ({json}->TryGetArrayField(""{field}"", _parr))
                {{
                    for (const TSharedPtr<FJsonValue>& e : *_parr)
                    {{
                        {elementType.Apply(EditorUeCppDefineTypeVisitor.Ins)} _v;
                        {elementType.Apply(EditorUeCppLoadVisitor2.Ins, "e.Get()", "_v")}
                        this->{field}.Add(_v);
                    }}
                }}
                else
                {{
                    return false;
                }}
            }}
";
        }

        public string Accept(TArray type, string json, string field)
        {
            return LoadList(type.ElementType, json, field);
        }

        public string Accept(TList type, string json, string field)
        {
            return LoadList(type.ElementType, json, field);
        }

        public string Accept(TSet type, string json, string field)
        {
            return LoadList(type.ElementType, json, field);
        }

        public string Accept(TMap type, string json, string field)
        {
            return $@"
            {{
                const TArray<TSharedPtr<FJsonValue>>* _parr;
                if ({json}->TryGetArrayField(""{field}"", _parr))
                {{
                    for (const TSharedPtr<FJsonValue>& e : *_parr)
                    {{
                        const TArray<TSharedPtr<FJsonValue>>* _pentry;
                        if (!e->TryGetArray(_pentry) || _pentry->Num() != 2) return false;

                        {type.KeyType.Apply(EditorUeCppDefineTypeVisitor.Ins)} _k;
                        {type.KeyType.Apply(EditorUeCppLoadVisitor2.Ins, "(*_pentry)[0].Get()", "_k")}
                        {type.ValueType.Apply(EditorUeCppDefineTypeVisitor.Ins)} _v;
                        {type.ValueType.Apply(EditorUeCppLoadVisitor2.Ins, "(*_pentry)[1].Get()", "_v")}
                        this->{field}.Add(_k, _v);
                    }}
                }}
                else
                {{
                    return false;
                }}
            }}
";
        }

        public string Accept(TVector2 type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TVector3 type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TVector4 type, string json, string field)
        {
            return DefaultLoad(json, field);
        }

        public string Accept(TDateTime type, string json, string field)
        {
            return DefaultLoad(json, field);
        }
    }
}
