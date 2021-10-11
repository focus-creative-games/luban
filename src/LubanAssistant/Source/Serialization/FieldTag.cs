namespace Bright.Serialization
{

    // 把 int,long,string,bool 调整到最小
    // 这样 marshal compatible write(field_id << tag_shift | tag_id) < 2^7 能在一个字节
    // 内序列化, 优化序列化最终大小
#pragma warning disable CA1720 // 标识符包含类型名称
    public static class FieldTag
    {
        public const int
        INT = 0,
        LONG = 1,
        STRING = 2,
        BOOL = 3,

        BYTE = 4,
        SHORT = 5,
        FSHORT = 6,
        FINT = 7,
        FLONG = 8,
        FLOAT = 9,
        DOUBLE = 10,
        BYTES = 11,
        ARRAY = 12,
        LIST = 13,
        SET = 14,
        MAP = 15,
        BEAN = 16,
        TEXT = 17,
        VECTOR2 = 18,
        VECTOR3 = 19,
        VECTOR4 = 20,
        DYNAMIC_BEAN = 21,

        NOT_USE = 22;


        public const int TAG_SHIFT = 5;
        public const int TAG_MASK = (1 << TAG_SHIFT) - 1;
    }
#pragma warning restore CA1720 // 标识符包含类型名称
}
