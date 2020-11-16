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
using Bright.Serialization;

namespace {{namespace_with_top_module}}
{
   
{{if !is_value_type}}
public {{cs_class_modifier}} class {{name}} : {{if parent_def_type}} {{parent}} {{else}} Bright.Transaction.BeanBase {{end}}
{
        {{- for field in fields }}
        {{if is_abstract_type}}protected{{else}}private{{end}} {{field.ctype.db_cs_define_type}} {{field.internal_name}};
        {{-end}}

    public {{name}}()
    {
        {{- for field in fields }}
        {{if field.ctype.need_init}}{{field.db_cs_init_field}} {{end}}
        {{end}}
    }

        {{- for field in fields }}
        {{if field.ctype.has_setter}}

        private sealed class {{field.log_type}} :  Bright.Transaction.FieldLogger<{{name}}, {{field.ctype.db_cs_define_type}}>
        {
            public {{field.log_type}}({{name}} self, {{field.ctype.db_cs_define_type}} value) : base(self, value) {  }

            public override long FieldId => host._objectId_ + {{field.id}};

            public override void Commit() { this.host.{{field.internal_name}} = this.Value; }


            public override void WriteBlob(ByteBuf _buf)
            {
                _buf.WriteInt(FieldTag.{{field.ctype.tag_name}});
                {{field.db_write_blob}}
            }
        }

        public {{field.ctype.db_cs_define_type}} {{field.public_name}}
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
                {{if db_field_cannot_null}}if (value == null) throw new ArgumentNullException();{{end}}
                if (this.InitedObjectId)
                {
                    var txn = Bright.Transaction.TransactionContext.ThreadStaticTxn;
                    txn.PutField(_objectId_ + {{field.id}}, new {{field.log_type}}(this, value));
                    {{-if field.ctype.need_set_children_root}}
                    value?.InitRoot(GetRoot());
                    {{end}}
                }
                else
                {
                    {{field.internal_name}} = value;
                } 
            }
        }
        {{else}}

        {{if field.ctype.is_collection}}

        private class {{field.log_type}} : {{field.ctype.db_cs_define_type}}.Log
        {
            private readonly {{name}} host;
            public {{field.log_type}}({{name}} host, {{field.ctype.immutable_type}} value) : base(value) { this.host = host;  }

            public override long FieldId => host._objectId_ + {{field.id}};

            public override Bright.Transaction.BeanBase Host => host;

            public override void Commit()
            {
                Commit(host.{{field.internal_name}});
            }

            public override void WriteBlob(ByteBuf _buf)
            {
                _buf.WriteInt(FieldTag.{{field.ctype.tag_name}});
                {{field.db_write_blob}}
            }
        }
        {{end}}

         public {{field.ctype.db_cs_define_type}} {{field.public_name}} => {{field.internal_name}};
        {{end}}
        {{-end}}

    {{if is_abstract_type}}
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
        {{- for child in hierarchy_not_abstract_children}}
            case {{child.full_name}}.ID: x = new {{child.full_name}}(); break;
        {{-end}}
            default: throw new SerializationException();
        }
        x.Deserialize(_buf);
        return x;
    }
    {{else}}
    public override void Serialize(ByteBuf _buf)
    {
        _buf.WriteLong(_objectId_);
        {{- for field in hierarchy_fields }}
        { _buf.WriteInt(FieldTag.{{field.ctype.tag_name}} | ({{field.id}} << FieldTag.TAG_SHIFT)); {{field.db_serialize_compatible}} }
        {{-end}}
    }

    public override void Deserialize(ByteBuf _buf)
    {
        _objectId_ = _buf.ReadLong();
        while(_buf.NotEmpty)
        {
            int _tag_ = _buf.ReadInt();
            switch (_tag_)
            {
            {{- for field in hierarchy_fields }}
            case FieldTag.{{field.ctype.tag_name}} | ({{field.id}} << FieldTag.TAG_SHIFT) : { {{field.db_deserialize_compatible}}  break; }
            {{-end}}
            default: { _buf.SkipUnknownField(_tag_); break; }
            }
        }
    }

    public const int ID = {{id}};
    public override int GetTypeId() => ID;

    {{end}}

    protected override void InitChildrenRoot(Bright.Transaction.TKey root)
    {
        {{- for field in hierarchy_fields }}
        {{if field.ctype.need_set_children_root}}this.{{field.internal_name}}?.InitRoot(root);{{end}}
        {{-end}}
    }

    public override string ToString()
    {
        return ""{{full_name}}{ ""
    {{- for field in hierarchy_fields }}
        + ""{{field.public_name}}:"" + {{field.db_cs_to_string}} + "",""
    {{-end}}
        + ""}"";
    }
}

{{else}}
  public struct {{name}} : ISerializable
{
        {{- for field in fields }}
        private {{field.ctype.db_cs_define_type}} {{field.internal_name}};
        {{-end}}

        {{- for field in fields }}
        public {{field.ctype.db_cs_define_type}} {{field.public_name}} {get => {{field.internal_name}}; set => {{field.internal_name}} = value;}
        {{-end}}

    public void Serialize(ByteBuf _buf)
    {
        _buf.WriteSize({{hierarchy_fields.size}});
        {{- for field in hierarchy_fields }}
        { _buf.WriteInt(FieldTag.{{field.ctype.tag_name}} | ({{field.id}} << FieldTag.TAG_SHIFT)); {{field.db_serialize_compatible}} }
        {{-end}}
    }

    public void Deserialize(ByteBuf _buf)
    {
        for (int _var_num_ = _buf.ReadSize(); -- _var_num_ >= 0;)
        {
            int _tag_ = _buf.ReadInt();
            switch (_tag_)
            {
            {{- for field in hierarchy_fields }}
            case FieldTag.{{field.ctype.tag_name}} | ({{field.id}} << FieldTag.TAG_SHIFT) : { {{field.db_deserialize_compatible}}  break; }
            {{-end}}
            default: { _buf.SkipUnknownField(_tag_); break; }
            }
        }
    }

    public int GetTypeId() => 0;

        public override string ToString()
        {
            return ""{{full_name}}{ ""
        {{- for field in hierarchy_fields }}
            + ""{{field.public_name}}:"" + {{field.db_cs_to_string}} + "",""
        {{-end}}
            + ""}"";
        }
}  

{{end}}

}
");
            var result = template.Render(b);
            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(@"
using System;
using System.Threading.Tasks;

namespace {{namespace_with_top_module}}
{

public sealed class {{name}}
{
    public static {{base_table_type}} Table { get; } = new {{internal_table_type}}({{table_uid}});

        private class {{internal_table_type}} : {{base_table_type}}
        {
            public {{internal_table_type}}(int tableId) : base(tableId)
            {

            }
        };

    public static {{value_ttype.cs_define_type}} Get({{key_ttype.cs_define_type}} key)
    {
        return Table.Get(key);
    }

    public static {{value_ttype.cs_define_type}} CreateIfNotExist({{key_ttype.cs_define_type}} key)
    {
        return Table.CreateIfNotExist(key);
    }

    public static void Insert({{key_ttype.cs_define_type}} key, {{value_ttype.cs_define_type}} value)
    {
        Table.Insert(key, value);
    }

    public static void Remove({{key_ttype.cs_define_type}} key)
    {
        Table.Remove(key);
    }

    public static void Put({{key_ttype.cs_define_type}} key, {{value_ttype.cs_define_type}} value)
    {
        Table.Put(key, value);
    }

    public static {{value_ttype.cs_define_type}} Select({{key_ttype.cs_define_type}} key)
    {
        return Table.Select(key);
    }
}
}

");
            var result = template.Render(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public string RenderTables(string name, string module, List<DbDefTypeBase> tables)
        {
            var template = t_stubRender ??= Template.Parse(@"
using Bright.Serialization;

namespace {{namespace}}
{
   
public static class {{name}}
{
        public static System.Collections.Generic.List<Bright.Transaction.TxnTable> TableList { get; } = new System.Collections.Generic.List<Bright.Transaction.TxnTable>
        {
        {{- for table in tables }}
            {{table.full_name}}.Table,
        {{-end}}
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
