using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class EditorUeCppSaveVisitor2 : ITypeFuncVisitor<string, string, string>
    {

        public static EditorUeCppSaveVisitor2 Ins { get; } = new EditorUeCppSaveVisitor2();

        private string DefaultSave(string json, string field)
        {
            return $"if(!JsonUtil::Write({json}, {field})) {{ return false;}}";
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
            //return $"{{FString _enumStr_; if(!{type.DefineEnum.UeFname}ToString({field}, _enumStr_)) {{ return false;}}  if(!JsonUtil::Write({json}, _enumStr_)) {{ return false;}}  }}";
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

                return $"if(!JsonUtil::WriteDynamicBean({json}, {field})) {{ return false; }}";
            }
            else
            {

                return $"if(!JsonUtil::WriteBean({json}, {field})) {{ return false; }}";
            }
        }

        public string Accept(TArray type, string json, string field)
        {
            return "";
        }

        public string Accept(TList type, string json, string field)
        {
            return "";
        }

        public string Accept(TSet type, string json, string field)
        {
            return "";
        }

        public string Accept(TMap type, string json, string field)
        {
            return "";
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
