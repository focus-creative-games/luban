using Luban.Job.Common.Types;
using Luban.Job.Db.RawDefs;
using Luban.Job.Db.TypeVisitors;
using System;

namespace Luban.Job.Db.Defs
{
    class DefTable : DbDefTypeBase
    {
        public DefTable(Table b)
        {
            Name = b.Name;
            Namespace = b.Namespace;
            TableUId = b.Id;
            KeyType = b.KeyType;
            ValueType = b.ValueType;
            IsPersistent = b.IsPersistent;
            Comment = b.Comment;
        }

        public string KeyType { get; }

        public string ValueType { get; }

        public bool IsPersistent { get; }

        public TType KeyTType { get; private set; }

        public TBean ValueTType { get; private set; }

        public int TableUId { get; set; }

        public string InternalTableType => "_" + Name;

        public string BaseTableType => $"BrightDB.Transaction.{(KeyTType is TLong ? "Long" : "String")}TxnTable<{ValueTType.Apply(DbCsDefineTypeVisitor.Ins)}>";

        public override void Compile()
        {
            var ass = Assembly;

            ass.AddDbTable(this);

            if ((KeyTType = ass.CreateType(Namespace, KeyType, false)) == null)
            {
                throw new Exception($"table:{FullName} key:{KeyType} 类型不合法");
            }

            if (!(KeyTType is TLong) && !(KeyTType is TString))
            {
                throw new Exception($"table:{FullName} key:{KeyTType} 不支持。只支持long与string类型");
            }

            if ((ValueTType = (TBean)ass.CreateType(Namespace, ValueType, false)) == null)
            {
                throw new Exception($"table:{FullName} value:{ValueType} 类型不合法");
            }
        }
    }
}
