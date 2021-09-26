{{
    name = x.name
    full_name = x.full_name
    parent = x.parent
    fields = x.fields
}}
using Bright.Serialization;

namespace {{x.namespace_with_top_module}}
{
{{~if x.comment != '' ~}}
    /// <summary>
    /// {{x.comment}}
    /// </summary>
{{~end~}}
    public sealed class {{name}} : Bright.Net.Codecs.Protocol
    {
        {{~ for field in fields ~}}
{{~if field.comment != '' ~}}
        /// <summary>
        /// {{field.comment}}
        /// </summary>
{{~end~}}
         public {{cs_define_type field.ctype}} {{field.cs_style_name}};

        {{~end~}}

        public {{name}}()
        {
        }

        public {{name}}(Bright.Common.NotNullInitialization _)
        {
            {{~ for field in fields ~}}
                {{~if cs_need_init field.ctype~}}
            {{cs_init_field_ctor_value field.cs_style_name field.ctype}}
                {{~end~}}
            {{~end~}}
        }
        public const int ID = {{x.id}};

        public override int GetTypeId()
        {
            return ID;
        }

        public override void Serialize(ByteBuf _buf)
        {
            {{~ for field in fields ~}}
            {{cs_serialize '_buf' field.cs_style_name field.ctype}}
            {{~end~}}
        }

        public override void Deserialize(ByteBuf _buf)
        {
            {{~ for field in fields ~}}
            {{cs_deserialize '_buf' field.cs_style_name field.ctype}}
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
        {{~ for field in fields ~}}
            + "{{field.cs_style_name}}:" + {{cs_to_string field.cs_style_name field.ctype}} + ","
        {{~end~}}
            + "}";
        }
    }

}
