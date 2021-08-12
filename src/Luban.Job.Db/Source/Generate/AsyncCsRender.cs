using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Db.Defs;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Db.Generate
{
    class AsyncCsRender
    {
        public string RenderAny(object o)
        {
            switch (o)
            {
                case DefConst c: return Render(c);
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                case DefTable p: return Render(p);
                default: throw new Exception($"unknown render type:'{o}'");
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
    full_name = x.full_name
    parent_def_type = x.parent_def_type
    fields = x.fields
    hierarchy_fields = x.hierarchy_fields
    is_abstract_type = x.is_abstract_type
    readonly_name = ""IReadOnly"" + name
}}
using Bright.Serialization;

namespace {{x.namespace_with_top_module}}
{

{{~if x.comment != '' ~}}
/// <summary>
/// {{x.comment}}
/// </summary>
{{~end~}}
public interface {{readonly_name}} {{if parent_def_type}}: IReadOnly{{x.parent_def_type.name}} {{end}}
{
    {{~ for field in fields~}}
    {{db_cs_readonly_define_type field.ctype}} {{field.cs_style_name}} {get;}
    {{~end~}}
}

{{~if x.comment != '' ~}}
/// <summary>
/// {{x.comment}}
/// </summary>
{{~end~}}
public {{x.cs_class_modifier}} class {{name}} : {{if parent_def_type}} {{x.parent}} {{else}} Bright.Transaction.TxnBeanBase {{end}}, {{readonly_name}}
{
    {{~ for field in fields~}}
    {{if is_abstract_type}}protected{{else}}private{{end}} {{db_cs_define_type field.ctype}} {{field.internal_name}};
    {{~end}}

    public {{name}}()
    {
        {{~ for field in fields~}}
        {{if cs_need_init field.ctype}}{{db_cs_init_field field.internal_name field.ctype}} {{end}}
        {{~end~}}
    }

    {{~ for field in fields~}}
        {{ctype = field.ctype}}
        {{~if has_setter ctype~}}

    private sealed class {{field.log_type}} :  Bright.Transaction.FieldLogger<{{name}}, {{db_cs_define_type ctype}}>
    {
        public {{field.log_type}}({{name}} self, {{db_cs_define_type ctype}} value) : base(self, value) {  }

        public override long FieldId => this._host.GetObjectId() + {{field.id}};

        public override int TagId => FieldTag.{{tag_name ctype}};

        public override void Commit() { this._host.{{field.internal_name}} = this.Value; }


        public override void WriteBlob(ByteBuf _buf)
        {
            {{cs_write_blob '_buf' 'this.Value' ctype}}
        }
    }

{{~if field.comment != '' ~}}
    /// <summary>
    /// {{field.comment}}
    /// </summary>
{{~end~}}
    public {{db_cs_define_type ctype}} {{field.cs_style_name}}
    { 
        get
        {
            if (this.IsManaged)
            {
                var txn = Bright.Transaction.TransactionContext.ThreadStaticCtx;
                if (txn == null) return {{field.internal_name}};
                var log = ({{field.log_type}})txn.GetField(this.GetObjectId() + {{field.id}});
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
            if (this.IsManaged)
            {
                var txn = Bright.Transaction.TransactionContext.ThreadStaticCtx;
                txn.PutField(this.GetObjectId() + {{field.id}}, new {{field.log_type}}(this, value));
                {{~if ctype.need_set_children_root}}
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
{{~if field.comment != '' ~}}
        /// <summary>
        /// {{field.comment}}
        /// </summary>
{{~end~}}
         public {{db_cs_define_type ctype}} {{field.cs_style_name}} => {{field.internal_name}};
        {{~end~}}

        {{~if ctype.bean || ctype.element_type ~}}
{{~if field.comment != '' ~}}
        /// <summary>
        /// {{field.comment}}
        /// </summary>
{{~end~}}
        {{db_cs_readonly_define_type ctype}} {{readonly_name}}.{{field.cs_style_name}} => {{field.internal_name}};
        {{~else if ctype.is_map~}}
{{~if field.comment != '' ~}}
        /// <summary>
        /// {{field.comment}}
        /// </summary>
{{~end~}}
        {{db_cs_readonly_define_type ctype}} {{readonly_name}}.{{field.cs_style_name}} => new Bright.Transaction.Collections.PReadOnlyMap<{{db_cs_readonly_define_type ctype.key_type}}, {{db_cs_readonly_define_type ctype.value_type}}, {{db_cs_define_type ctype.value_type}}>({{field.internal_name}});
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
        _buf.WriteLong(this.GetObjectId());
        {{~ for field in hierarchy_fields~}}
        { _buf.WriteInt(FieldTag.{{tag_name field.ctype}} | ({{field.id}} << FieldTag.TAG_SHIFT)); {{db_cs_compatible_serialize '_buf' field.internal_name field.ctype}} }
        {{~end}}
    }

    public override void Deserialize(ByteBuf _buf)
    {
        this.SetObjectId(_buf.ReadLong());
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

    protected override void InitChildrenRoot(Bright.Storage.TKey root)
    {
        {{~ for field in hierarchy_fields~}}
        {{~if need_set_children_root field.ctype~}}
        UnsafeUtil.InitRoot({{field.internal_name}}, root);
        {{~end~}}
        {{~end~}}
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

{{~if x.comment != '' ~}}
/// <summary>
/// {{x.comment}}
/// </summary>
{{~end~}}
public sealed class {{name}}
{
    public static {{base_table_type}} Table { get; } = new {{internal_table_type}}();

        private class {{internal_table_type}} : {{base_table_type}}
        {
            public {{internal_table_type}}() : base({{x.table_uid}}, ""{{x.full_name}}"")
            {

            }
        };

    public static ValueTask<{{db_cs_define_type value_ttype}}> GetAsync({{db_cs_define_type key_ttype}} key)
    {
        return Table.GetAsync(key);
    }

    public static ValueTask<{{db_cs_define_type value_ttype}}> CreateIfNotExistAsync({{db_cs_define_type key_ttype}} key)
    {
        return Table.CreateIfNotExistAsync(key);
    }

    public static Task InsertAsync({{db_cs_define_type key_ttype}} key, {{db_cs_define_type value_ttype}} value)
    {
        return Table.InsertAsync(key, value);
    }

    public static Task RemoveAsync({{db_cs_define_type key_ttype}} key)
    {
        return Table.RemoveAsync(key);
    }

    public static Task PutAsync({{db_cs_define_type key_ttype}} key, {{db_cs_define_type value_ttype}} value)
    {
        return Table.PutAsync(key, value);
    }

    public static ValueTask<{{db_cs_readonly_define_type value_ttype}}> SelectAsync({{db_cs_define_type key_ttype}} key)
    {
        return Table.SelectAsync<{{db_cs_readonly_define_type value_ttype}}>(key);
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
