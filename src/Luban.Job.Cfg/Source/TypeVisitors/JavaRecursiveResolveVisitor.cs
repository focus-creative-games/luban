using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class JavaRecursiveResolveVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static JavaRecursiveResolveVisitor Ins { get; } = new JavaRecursiveResolveVisitor();

        public string Accept(TBool type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TByte type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TShort type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TFshort type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TInt type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TFint type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TLong type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TFlong type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TFloat type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TDouble type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TEnum type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TString type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TBytes type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TText type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TBean type, string fieldName, string tablesName)
        {
            return $"if ({fieldName} != null) {{{fieldName}.resolve({tablesName});}}";
        }

        public string Accept(TArray type, string fieldName, string tablesName)
        {
            return $"for({type.ElementType.Apply(JavaDefineTypeName.Ins)} _e : {fieldName}) {{ if (_e != null) _e.resolve({tablesName}); }}";
        }

        public string Accept(TList type, string fieldName, string tablesName)
        {
            return $"for({type.ElementType.Apply(JavaBoxDefineTypeName.Ins)} _e : {fieldName}) {{ if (_e != null) _e.resolve({tablesName}); }}";
        }

        public string Accept(TSet type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TMap type, string fieldName, string tablesName)
        {
            return $"for({type.ValueType.Apply(JavaBoxDefineTypeName.Ins)} _e : {fieldName}.values()) {{ if (_e != null) _e.resolve({tablesName}); }}";
        }

        public string Accept(TVector2 type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TVector3 type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TVector4 type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }

        public string Accept(TDateTime type, string fieldName, string tablesName)
        {
            throw new NotImplementedException();
        }
    }
}
