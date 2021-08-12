using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Db.Defs;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Db.Generate
{
    class TypescriptRender
    {
        public string RenderAny(object o)
        {
            switch (o)
            {
                case DefConst c: return Render(c);
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                case DefTable r: return Render(r);
                default: throw new Exception($"unknown render type:{o}");
            }
        }

        private string Render(DefConst c)
        {
            return RenderUtil.RenderTypescriptConstClass(c);
        }

        private string Render(DefEnum e)
        {
            return RenderUtil.RenderTypescriptEnumClass(e);
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

{{x.typescript_namespace_begin}}
{{~if x.comment != '' ~}}
/**
 * {{x.comment}}
 */
{{~end~}}
export {{x.ts_class_modifier}} class {{name}} extends {{if parent_def_type}} {{x.parent}} {{else}} TxnBeanBase {{end}}{
    {{~ for field in fields~}}
{{~if field.comment != '' ~}}
    /**
     * {{field.comment}}
     */
{{~end~}}
    {{if is_abstract_type}}protected{{else}}private{{end}} {{field.internal_name}}: {{db_ts_define_type field.ctype}} 
    {{~end}}

    constructor() {
        super()
        {{~ for field in fields~}}
        {{db_ts_init_field field.internal_name_with_this field.log_type field.ctype }}
        {{~end~}}
    }

    {{~ for field in fields~}}
        {{~ctype = field.ctype~}}
        {{~if has_setter ctype~}}
    private static {{field.log_type}} = class extends FieldLoggerGeneric2<{{name}}, {{db_ts_define_type ctype}}> {
        constructor(self:{{name}}, value: {{db_ts_define_type ctype}}) { super(self, value) }

        get fieldId(): number { return this.host.getObjectId() + {{field.id}} }

        get tagId(): number { return FieldTag.{{tag_name ctype}} }

        commit() { this.host.{{field.internal_name}} = this.value }

        writeBlob(_buf: ByteBuf) {
            {{ts_write_blob '_buf' 'this.value' ctype}}
        }
    }

{{~if field.comment != '' ~}}
    /**
     * {{field.comment}}
     */
{{~end~}}
    get {{field.ts_style_name}}(): {{db_ts_define_type ctype}} {
        if (this.isManaged) {
            var txn = TransactionContext.current
            if (txn == null) return {{field.internal_name_with_this}}
            let log: any = txn.getField(this.getObjectId() + {{field.id}})
            return log != null ? log.value : {{field.internal_name_with_this}}
        } else {
            return {{field.internal_name_with_this}};
        }
    }

{{~if field.comment != '' ~}}
    /**
     * {{field.comment}}
     */
{{~end~}}
    set {{field.ts_style_name}}(value: {{db_ts_define_type ctype}}) {
        {{~if db_field_cannot_null~}}
        if (value == null) throw new Error()
        {{~end~}}
        if (this.isManaged) {
            let txn = TransactionContext.current!
            txn.putFieldLong(this.getObjectId() + {{field.id}}, new {{name}}.{{field.log_type}}(this, value))
            {{~if ctype.need_set_children_root~}}
            value?.initRoot(this.getRoot())
            {{~end~}}
        } else {
            {{field.internal_name_with_this}} = value
        } 
    }

        {{~else~}}
{{~if field.comment != '' ~}}
    /**
     * {{field.comment}}
     */
{{~end~}}
    get {{field.ts_style_name}}(): {{db_ts_define_type ctype}}  { return {{field.internal_name_with_this}} }
        {{~end~}}
    {{~end~}}

    {{~if is_abstract_type~}}
    static serialize{{name}}Any(_buf: ByteBuf, x: {{name}}) {
        if (x == null) { _buf.WriteInt(0); return }
        _buf.WriteInt(x.getTypeId())
        x.serialize(_buf)
    }

    deserialize{{name}}Any(_buf: ByteBuf): {{name}}{
        let x: {{name}}
        switch (_buf.ReadInt()) {
        {{~ for child in x.hierarchy_not_abstract_children~}}
            case {{child.full_name}}.ID: x = new {{child.full_name}}(); break
        {{~end~}}
            default: throw new Error()
        }
        x.deserialize(_buf)
        return x
    }
    {{~else~}}
    serialize(_buf: ByteBuf) {
        _buf.WriteNumberAsLong(this.getObjectId())
        {{~ for field in hierarchy_fields~}}
        { _buf.WriteInt(FieldTag.{{tag_name field.ctype}} | ({{field.id}} << FieldTag.TAG_SHIFT)); {{db_ts_compatible_serialize '_buf' field.internal_name_with_this field.ctype}} }
        {{~end}}
    }

    deserialize(_buf: ByteBuf) {
        this.setObjectId(_buf.ReadLongAsNumber())
        while(_buf.NotEmpty) {
            let _tag_ = _buf.ReadInt()
            switch (_tag_) {
            {{~ for field in hierarchy_fields~}}
            case FieldTag.{{tag_name field.ctype}} | ({{field.id}} << FieldTag.TAG_SHIFT) : { {{db_ts_compatible_deserialize '_buf' field.internal_name_with_this field.ctype}}  break; }
            {{~end~}}
            default: { _buf.SkipUnknownField(_tag_); break; }
            }
        }
    }

    static readonly ID = {{x.id}}
    getTypeId(): number { return {{name}}.ID }
    {{~end~}}

    initChildrenRoot(root: TKey) {
        {{~ for field in hierarchy_fields~}}
        {{~if need_set_children_root field.ctype~}}
        {{field.internal_name_with_this}}?.initRoot(root)
        {{~end~}}
        {{~end}}
    }

    toString(): string {
        return ""{{full_name}}{ ""
    {{~ for field in hierarchy_fields~}}
        + ""{{field.ts_style_name}}:"" + {{ts_to_string ('this.' + field.ts_style_name) field.ctype}} + "",""
    {{~end~}}
        + ""}""
    }
}

{{x.typescript_namespace_end}}
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
    internal_table_type = x.internal_table_type
}}

{{x.typescript_namespace_begin}}
 class {{internal_table_type}} extends TxnTableGeneric<{{db_ts_define_type key_ttype}},{{db_ts_define_type value_ttype}}> {
    constructor() {
        super({{x.table_uid}}, ""{{x.full_name}}"")
    }

    newValue(): {{db_ts_define_type value_ttype}} { return new {{db_ts_define_type value_ttype}}() }

    serializeKey(buf: ByteBuf, key: {{db_ts_define_type key_ttype}}) {
        {{db_ts_compatible_serialize_without_segment 'buf' 'key' key_ttype}}
    }

    serializeValue(buf: ByteBuf, value: {{db_ts_define_type value_ttype}}) {
        {{db_ts_compatible_serialize_without_segment 'buf' 'value' value_ttype}}
    }

    deserializeKey(buf: ByteBuf): {{db_ts_define_type key_ttype}} {
        let key: {{db_ts_define_type key_ttype}}
        {{db_ts_compatible_deserialize_without_segment 'buf' 'key' key_ttype}}
        return key
    }

    deserializeValue(buf: ByteBuf): {{db_ts_define_type value_ttype}} {
        let value = new {{db_ts_define_type value_ttype}}()
        {{db_ts_compatible_deserialize_without_segment 'buf' 'value' value_ttype}}
        return value
    }
}

{{~if x.comment != '' ~}}
/**
 * {{x.comment}}
 */
{{~end~}}
export class {{name}} {
    static readonly _table = new {{internal_table_type}}();
    static get table(): TxnTableGeneric<{{db_ts_define_type key_ttype}},{{db_ts_define_type value_ttype}}> { return this._table }

    static getAsync(key: {{db_ts_define_type key_ttype}}): Promise<{{db_ts_define_type value_ttype}}> {
        return {{name}}._table.getAsync(key);
    }

    static createIfNotExistAsync(key: {{db_ts_define_type key_ttype}}): Promise<{{db_ts_define_type value_ttype}}> {
        return {{name}}._table.createIfNotExistAsync(key);
    }

    static insertAsync(key: {{db_ts_define_type key_ttype}}, value: {{db_ts_define_type value_ttype}}): Promise<void> {
        return {{name}}._table.insertAsync(key, value);
    }

    static removeAsync(key: {{db_ts_define_type key_ttype}}): Promise<void> {
        return {{name}}._table.removeAsync(key);
    }

    static put(key: {{db_ts_define_type key_ttype}}, value: {{db_ts_define_type value_ttype}}): Promise<void> {
        return {{name}}._table.putAsync(key, value);
    }

    static selectAsync(key: {{db_ts_define_type key_ttype}}): Promise<{{db_ts_define_type value_ttype}}> {
        return {{name}}._table.selectAsync(key);
    }
}

{{x.typescript_namespace_end}}

");
            var result = template.RenderCode(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public string RenderTables(string name, string module, List<DefTable> tables)
        {
            var template = t_stubRender ??= Template.Parse(@"

export class {{name}} {
    static readonly tableList: TxnTable[] = [
    {{~ for table in tables~}}
        {{table.full_name}}.table,
    {{~end}}
    ]
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
