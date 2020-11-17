using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Db.Defs;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Db.Generate
{
    class CsRender
    {
        public string RenderAny(object o)
        {
            switch (o)
            {
                case DefConst c: return Render(c);
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                case DefTable p: return Render(p);
                default: throw new Exception($"unknown render type:{o}");
            }
        }

        public string Render(DefConst c)
        {
            return RenderUtil.RenderCsConstClass(c);
        }

        public string Render(DefEnum e)
        {
            return RenderUtil.RenderCsEnumClass(e);
        }

        [ThreadStatic]
        private static Template t_beanRender;
        public string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(@"
{{
    name = x.name
    parent_def_type = x.parent_def_type
    fields = x.fields
    hierarchy_fields = x.hierarchy_fields
    is_abstract_type = x.is_abstract_type
}}
using Bright.Serialization;

namespace {{x.namespace_with_top_module}}
{
   
public {{x.cs_class_modifier}} class {{name}} : {{if parent_def_type}} {{x.parent}} {{else}} Bright.Transaction.TxnBeanBase {{end}}
{
    {{~ for field in fields~}}
        {{if is_abstract_type}}protected{{else}}private{{end}} {{db_cs_define_type field.ctype}} {{field.internal_name}};
    {{~end}}

    public {{name}}()
    {
        {{~ for field in fields~}}
        {{if cs_need_init field.ctype}}{{db_cs_init_field field.internal_name field.log_type field.ctype }} {{end}}
        {{~end~}}
    }

    {{~ for field in fields~}}
        {{~if has_setter field.ctype~}}

    private sealed class {{field.log_type}} :  Bright.Transaction.FieldLogger<{{name}}, {{db_cs_define_type field.ctype}}>
    {
        public {{field.log_type}}({{name}} self, {{db_cs_define_type field.ctype}} value) : base(self, value) {  }

        public override long FieldId => host._objectId_ + {{field.id}};

        public override void Commit() { this.host.{{field.internal_name}} = this.Value; }


        public override void WriteBlob(ByteBuf _buf)
        {
            _buf.WriteInt(FieldTag.{{tag_name field.ctype}});
            {{cs_write_blob '_buf' 'this.Value' field.ctype}}
        }
    }

    public {{db_cs_define_type field.ctype}} {{field.cs_style_name}}
    { 
        get
        {
            if (this.InitedObjectId)
            {
                var txn = Bright.Transaction.TransactionContext.ThreadStaticTxn;
                if (txn == null) return {{field.internal_name}};
                var log = ({{field.log_type}})txn.GetField(_objectId_ + {{field.id}});
                return log != null ? log.Value : {{field.internal_name}};
            }
            else
            {
                return {{field.internal_name}};
            }
        }
        set
        {
            {{~if db_field_cannot_null~}}
            if (value == null) throw new ArgumentNullException();
            {{~end~}}
            if (this.InitedObjectId)
            {
                var txn = Bright.Transaction.TransactionContext.ThreadStaticTxn;
                txn.PutField(_objectId_ + {{field.id}}, new {{field.log_type}}(this, value));
                {{~if field.ctype.need_set_children_root}}
                value?.InitRoot(GetRoot());
                {{end}}
            }
            else
            {
                {{field.internal_name}} = value;
            } 
        }
    }
        {{~else~}}
            {{~if field.ctype.is_collection~}}
        private class {{field.log_type}} : {{db_cs_define_type field.ctype}}.Log
        {
            private readonly {{name}} host;
            public {{field.log_type}}({{name}} host, {{cs_immutable_type field.ctype}} value) : base(value) { this.host = host;  }

            public override long FieldId => host._objectId_ + {{field.id}};

            public override Bright.Transaction.TxnBeanBase Host => host;

            public override void Commit()
            {
                Commit(host.{{field.internal_name}});
            }

            public override void WriteBlob(ByteBuf _buf)
            {
                _buf.WriteInt(FieldTag.{{tag_name field.ctype}});
                {{cs_write_blob '_buf' 'this.Value' field.ctype}}
            }
        }
            {{~end~}}

         public {{db_cs_define_type field.ctype}} {{field.cs_style_name}} => {{field.internal_name}};
        {{~end~}}
    {{~end~}}

    {{~if is_abstract_type~}}
    public static void Serialize{{name}}(ByteBuf _buf, {{name}} x)
    {
        if (x == null) { _buf.WriteInt(0); return; }
        _buf.WriteInt(x.GetTypeId());
        x.Serialize(_buf);
    }

    public static {{name}} Deserialize{{name}}(ByteBuf _buf)
    {
        {{name}} x;
        switch (_buf.ReadInt())
        {
            case 0 : return null;
        {{~ for child in x.hierarchy_not_abstract_children~}}
            case {{child.full_name}}.ID: x = new {{child.full_name}}(); break;
        {{~end~}}
            default: throw new SerializationException();
        }
        x.Deserialize(_buf);
        return x;
    }
    {{~else~}}
    public override void Serialize(ByteBuf _buf)
    {
        _buf.WriteLong(_objectId_);
        {{~ for field in hierarchy_fields~}}
        { _buf.WriteInt(FieldTag.{{tag_name field.ctype}} | ({{field.id}} << FieldTag.TAG_SHIFT)); {{db_cs_compatible_serialize '_buf' field.internal_name field.ctype}} }
        {{~end}}
    }

    public override void Deserialize(ByteBuf _buf)
    {
        _objectId_ = _buf.ReadLong();
        while(_buf.NotEmpty)
        {
            int _tag_ = _buf.ReadInt();
            switch (_tag_)
            {
            {{~ for field in hierarchy_fields~}}
            case FieldTag.{{tag_name field.ctype}} | ({{field.id}} << FieldTag.TAG_SHIFT) : { {{db_cs_compatible_deserialize '_buf' field.internal_name field.ctype}}  break; }
            {{~end~}}
            default: { _buf.SkipUnknownField(_tag_); break; }
            }
        }
    }

    public const int ID = {{x.id}};
    public override int GetTypeId() => ID;
    {{~end~}}

    protected override void InitChildrenRoot(Bright.Transaction.TKey root)
    {
        {{~ for field in hierarchy_fields~}}
        {{if need_set_children_root field.ctype}}{{field.internal_name}}?.InitRoot(root);{{end}}
        {{~end}}
    }

    public override string ToString()
    {
        return ""{{full_name}}{ ""
    {{~ for field in hierarchy_fields~}}
        + ""{{field.cs_style_name}}:"" + {{cs_to_string field.cs_style_name field.ctype}} + "",""
    {{~end~}}
        + ""}"";
    }
}

}
");
            var result = template.RenderCode(b);
            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(@"
{{
    name = x.name
    key_ttype = x.key_ttype
    value_ttype = x.value_ttype
    base_table_type = x.base_table_type
    internal_table_type = x.internal_table_type
}}
using System;
using System.Threading.Tasks;

namespace {{x.namespace_with_top_module}}
{

public sealed class {{name}}
{
    public static {{base_table_type}} Table { get; } = new {{internal_table_type}}({{x.table_uid}});

        private class {{internal_table_type}} : {{base_table_type}}
        {
            public {{internal_table_type}}(int tableId) : base(tableId)
            {

            }
        };

    public static {{db_cs_define_type value_ttype}} Get({{db_cs_define_type key_ttype}} key)
    {
        return Table.Get(key);
    }

    public static {{db_cs_define_type value_ttype}} CreateIfNotExist({{db_cs_define_type key_ttype}} key)
    {
        return Table.CreateIfNotExist(key);
    }

    public static void Insert({{db_cs_define_type key_ttype}} key, {{db_cs_define_type value_ttype}} value)
    {
        Table.Insert(key, value);
    }

    public static void Remove({{db_cs_define_type key_ttype}} key)
    {
        Table.Remove(key);
    }

    public static void Put({{db_cs_define_type key_ttype}} key, {{db_cs_define_type value_ttype}} value)
    {
        Table.Put(key, value);
    }

    public static {{db_cs_define_type value_ttype}} Select({{db_cs_define_type key_ttype}} key)
    {
        return Table.Select(key);
    }
}
}

");
            var result = template.RenderCode(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public string RenderTables(string name, string module, List<DefTable> tables)
        {
            var template = t_stubRender ??= Template.Parse(@"
using Bright.Serialization;

namespace {{namespace}}
{
   
public static class {{name}}
{
        public static System.Collections.Generic.List<Bright.Transaction.TxnTable> TableList { get; } = new System.Collections.Generic.List<Bright.Transaction.TxnTable>
        {
        {{~ for table in tables~}}
            {{table.full_name}}.Table,
        {{~end}}
        };
}

}

");
            var result = template.Render(new
            {
                Name = name,
                Namespace = module,
                Tables = tables,
            });

            return result;
        }
    }
}
