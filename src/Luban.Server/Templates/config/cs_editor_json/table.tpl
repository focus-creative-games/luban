using Bright.Serialization;

namespace {{namespace}}
{
   
public sealed class {{name}} : Bright.Net.Protocol
{
    {{~for field in fields ~}}
     public {{field.ctype.cs_define_type}} {{field.convention_name}};
    {{~end~}}
    public {{name}}()
    {
    }
    public {{name}}(Bright.Common.NotNullInitialization _)
    {
        {{~for field in fields ~}}
        {{~if field.ctype.need_init~}}
        {{field.proto_cs_init_field}}
        {{~end~}}
        {{~end~}}
    }
    public const int __ID__ = {{id}};

    public override int GetTypeId()
    {
        return __ID__;
    }

    public override void Serialize(ByteBuf _buf)
    {
        {{~for field in fields ~}}
        {{field.cs_serialize}}
        {{~end~}}
    }

    public override void Deserialize(ByteBuf _buf)
    {
        {{~for field in fields ~}}
        {{field.cs_deserialize}}
        {{~end~}}
    }

    public override void Reset()
    {
        throw new System.NotImplementedException();
    }

    public override object Clone()
    {
        throw new System.NotImplementedException();
    }

    public override string ToString()
    {
        return "{{full_name}}{ "
    {{~for field in fields ~}}
        + "{{field.convention_name}}:" + {{field.proto_cs_to_string}} + ","
    {{~end~}}
        + "}";
    }
}

}
