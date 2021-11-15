using Bright.Serialization;

namespace {{namespace}}
{
   
public {{cs_class_modifier}} class {{name}} : {{if parent_def_type}} {{parent}} {{else}} ISerializable {{if is_abstract_type}}, ITypeId {{end}} {{end}}
{
    public {{name}}() {{if parent_def_type}} : base() {{end}}
    {

    }

    public {{name}}(Bright.Common.NotNullInitialization _) {{if parent_def_type}} : base(_) {{end}}
    {
        {{~for field in fields ~}}
        {{~if field.ctype.need_init~}}
        {{field.proto_cs_init_field}}
        {{~end~}}
        {{~end~}}
    }

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
        {{~for child in hierarchy_not_abstract_children~}}
            case {{child.full_name}}.__ID__: x = new {{child.full_name}}(false); break;
        {{~end~}}
            default: throw new SerializationException();
        }
        x.Deserialize(_buf);
        return x;
    }
    {{~end~}}
    {{~for field in fields ~}}
     public {{field.ctype.cs_define_type}} {{field.convention_name}};
    {{~end~}}

    {{~if !parent_def_type && is_abstract_type~}}
    public abstract int GetTypeId();
    {{~end~}}
    {{~if parent_def_type && !is_abstract_type~}}
    public const int __ID__ = {{id}};
    public override int GetTypeId()
    {
        return __ID__;
    }
    {{~end~}}

    public {{cs_method_modifer}} void Serialize(ByteBuf _buf)
    {
        {{~if parent_def_type~}}
        base.Serialize(_buf);
        {{~end~}}
        {{~for field in fields ~}}
        {{field.cs_serialize}}
        {{~end~}}
    }

    public {{cs_method_modifer}} void Deserialize(ByteBuf _buf)
    {
        {{~if parent_def_type~}}
        base.Deserialize(_buf);
        {{~end~}}
        {{~for field in fields ~}}
        {{field.cs_deserialize}}
        {{~end~}}
    }

        public override string ToString()
        {
            return "{{full_name}}{ "
        {{~for field in hierarchy_fields ~}}
            + "{{field.convention_name}}:" + {{field.proto_cs_to_string}} + ","
        {{~end~}}
            + "}";
        }
    }

}
