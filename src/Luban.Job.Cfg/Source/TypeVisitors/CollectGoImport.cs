using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System.Collections.Generic;

namespace Luban.Job.Cfg.TypeVisitors
{
    class CollectGoImport : ITypeActionVisitor<HashSet<string>>
    {
        public static CollectGoImport Ins { get; } = new CollectGoImport();

        public void Accept(TBool type, HashSet<string> x)
        {

        }

        public void Accept(TByte type, HashSet<string> x)
        {

        }

        public void Accept(TShort type, HashSet<string> x)
        {

        }

        public void Accept(TFshort type, HashSet<string> x)
        {

        }

        public void Accept(TInt type, HashSet<string> x)
        {

        }

        public void Accept(TFint type, HashSet<string> x)
        {

        }

        public void Accept(TLong type, HashSet<string> x)
        {

        }

        public void Accept(TFlong type, HashSet<string> x)
        {

        }

        public void Accept(TFloat type, HashSet<string> x)
        {

        }

        public void Accept(TDouble type, HashSet<string> x)
        {

        }

        public void Accept(TEnum type, HashSet<string> x)
        {

        }

        public void Accept(TString type, HashSet<string> x)
        {

        }

        public void Accept(TBytes type, HashSet<string> x)
        {

        }

        public void Accept(TText type, HashSet<string> x)
        {

        }

        public void Accept(TBean type, HashSet<string> x)
        {

        }

        public void Accept(TArray type, HashSet<string> x)
        {
            type.ElementType.Apply(this, x);
        }

        public void Accept(TList type, HashSet<string> x)
        {

            type.ElementType.Apply(this, x);
        }

        public void Accept(TSet type, HashSet<string> x)
        {
            type.ElementType.Apply(this, x);
        }

        public void Accept(TMap type, HashSet<string> x)
        {

            type.KeyType.Apply(this, x);
            type.ValueType.Apply(this, x);
        }

        public void Accept(TVector2 type, HashSet<string> x)
        {
            x.Add("bright/math");
        }

        public void Accept(TVector3 type, HashSet<string> x)
        {
            x.Add("bright/math");
        }

        public void Accept(TVector4 type, HashSet<string> x)
        {
            x.Add("bright/math");
        }

        public void Accept(TDateTime type, HashSet<string> x)
        {

        }
    }
}
