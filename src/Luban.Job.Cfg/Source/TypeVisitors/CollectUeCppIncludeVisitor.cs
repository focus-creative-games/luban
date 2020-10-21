using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.TypeVisitors
{
    class CollectUeCppIncludeVisitor : ITypeActionVisitor<HashSet<DefTypeBase>>
    {
        public static CollectUeCppIncludeVisitor Ins { get; } = new CollectUeCppIncludeVisitor();

        public void Accept(TBool type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TByte type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TShort type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TFshort type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TInt type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TFint type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TLong type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TFlong type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TFloat type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TDouble type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TEnum type, HashSet<DefTypeBase> x)
        {
            x.Add(type.DefineEnum);
        }

        public void Accept(TString type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TBytes type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TText type, HashSet<DefTypeBase> x)
        {
            throw new NotImplementedException();
        }

        public void Accept(TBean type, HashSet<DefTypeBase> x)
        {
            //x.Add(type.Bean);
        }

        public void Accept(TArray type, HashSet<DefTypeBase> x)
        {
            type.ElementType.Apply(this, x);
        }

        public void Accept(TList type, HashSet<DefTypeBase> x)
        {
            type.ElementType.Apply(this, x);
        }

        public void Accept(TSet type, HashSet<DefTypeBase> x)
        {
            type.ElementType.Apply(this, x);
        }

        public void Accept(TMap type, HashSet<DefTypeBase> x)
        {
            type.KeyType.Apply(this, x);
            type.ValueType.Apply(this, x);
        }

        public void Accept(TVector2 type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TVector3 type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TVector4 type, HashSet<DefTypeBase> x)
        {

        }

        public void Accept(TDateTime type, HashSet<DefTypeBase> x)
        {

        }
    }
}
