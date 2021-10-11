using System.Collections.Generic;
using System.Numerics;
using Bright.Serialization;

namespace Bright.Common
{
    public static class SerializationUtil
    {
        public static void Serialize<T>(ByteBuf os, List<T> list) where T : ISerializable
        {
            os.WriteSize(list.Count);
            foreach (var e in list)
            {
                e.Serialize(os);
            }
        }

        public static void Deserialize<T>(ByteBuf os, List<T> list) where T : ISerializable, new()
        {
            int n = os.ReadSize();
            for (int i = 0; i < n; i++)
            {
                T e = new T();
                e.Deserialize(os);
                list.Add(e);
            }
        }

        public static void Serialize(ByteBuf os, List<string> list)
        {
            os.WriteSize(list.Count);
            foreach (var e in list)
            {
                os.WriteString(e);
            }
        }

        public static void Deserialize(ByteBuf os, List<string> list)
        {
            int n = os.ReadSize();
            for (int i = 0; i < n; i++)
            {
                list.Add(os.ReadString());
            }
        }

        public static unsafe int FloatToIntBits(float f)
        {
            return *((int*)&f);
        }

        public static void SerializeBool(ByteBuf buf, bool x)
        {
            buf.WriteBool(x);
        }

        public static bool DeserializeBool(ByteBuf buf)
        {
            return buf.ReadBool();
        }

        public static void SerializeByte(ByteBuf buf, byte x)
        {
            buf.WriteByte(x);
        }

        public static byte DeserializeByte(ByteBuf buf)
        {
            return buf.ReadByte();
        }

        public static void SerializeShort(ByteBuf buf, short x)
        {
            buf.WriteShort(x);
        }

        public static short DeserializeShort(ByteBuf buf)
        {
            return buf.ReadShort();
        }

        public static void SerializeFshort(ByteBuf buf, short x)
        {
            buf.WriteFshort(x);
        }

        public static short DeserializeFshort(ByteBuf buf)
        {
            return buf.ReadFshort();
        }

        public static void SerializeInt(ByteBuf buf, int x)
        {
            buf.WriteInt(x);
        }

        public static int DeserializeInt(ByteBuf buf)
        {
            return buf.ReadInt();
        }

        public static void SerializeFint(ByteBuf buf, int x)
        {
            buf.WriteFint(x);
        }

        public static int DeserializeFint(ByteBuf buf)
        {
            return buf.ReadFint();
        }

        public static void SerializeLong(ByteBuf buf, long x)
        {
            buf.WriteLong(x);
        }

        public static long DeserializeLong(ByteBuf buf)
        {
            return buf.ReadLong();
        }

        public static void SerializeFlong(ByteBuf buf, long x)
        {
            buf.WriteFlong(x);
        }

        public static long DeserializeFlong(ByteBuf buf)
        {
            return buf.ReadFlong();
        }

        public static void SerializeFloat(ByteBuf buf, float x)
        {
            buf.WriteFloat(x);
        }

        public static float DeserializeFloat(ByteBuf buf)
        {
            return buf.ReadFloat();
        }

        public static void SerializeDouble(ByteBuf buf, double x)
        {
            buf.WriteDouble(x);
        }

        public static double DeserializeDouble(ByteBuf buf)
        {
            return buf.ReadDouble();
        }

        public static void SerializeString(ByteBuf buf, string x)
        {
            buf.WriteString(x);
        }

        public static string DeserializeString(ByteBuf buf)
        {
            return buf.ReadString();
        }

        public static void SerializeBytes(ByteBuf buf, byte[] x)
        {
            buf.WriteBytes(x);
        }

        public static byte[] DeserializeBytes(ByteBuf buf)
        {
            return buf.ReadBytes();
        }

        public static void SerializeVector2(ByteBuf buf, Vector2 x)
        {
            buf.WriteVector2(x);
        }

        public static Vector2 DeserializeVector2(ByteBuf buf)
        {
            return buf.ReadVector2();
        }

        public static void SerializeVector3(ByteBuf buf, Vector3 x)
        {
            buf.WriteVector3(x);
        }

        public static Vector3 DeserializeVector3(ByteBuf buf)
        {
            return buf.ReadVector3();
        }

        public static void SerializeVector4(ByteBuf buf, Vector4 x)
        {
            buf.WriteVector4(x);
        }

        public static Vector4 DeserializeVector4(ByteBuf buf)
        {
            return buf.ReadVector4();
        }

        public static void SerializeBean<T>(ByteBuf buf, T x) where T : BeanBase
        {
            x.Serialize(buf);
        }

        public static T DeserializeBean<T>(ByteBuf buf) where T : BeanBase, new()
        {
            var x = new T();
            x.Deserialize(buf);
            return x;
        }
    }
}
