namespace Luban.Job.Common.Utils
{
    public static class TypescriptStringTemplate
    {
        public const string PuertsByteBufImports = @"
import {Bright} from 'csharp'
import ByteBuf = Bright.Serialization.ByteBuf";

        public static string GetByteBufImports(string path, string package)
        {
            return string.IsNullOrEmpty(package) ? string.Format(BrightByteBufPathImportsFormat, path) : string.Format(BrightByteBufPackageImportsFormat, package);
        }

        public const string BrightByteBufPathImportsFormat = "import ByteBuf from '{0}/serialization/ByteBuf'";
        public const string BrightByteBufPackageImportsFormat = "import {{ByteBuf}} from '{0}'";

        public static string GetVectorImports(string path, string package)
        {
            return string.IsNullOrEmpty(package) ? string.Format(VectorPathImportsFormat, path) : string.Format(VectorPackageImportsFormat, package);
        }

        public const string VectorPathImportsFormat = @"
import Vector2 from '{0}/math/Vector2'
import Vector3 from '{0}/math/Vector3'
import Vector4 from '{0}/math/Vector4'
";
        public const string VectorPackageImportsFormat = @"
import {{Vector2}} from '{0}'
import {{Vector3}} from '{0}'
import {{Vector4}} from '{0}'
";
        public static string GetSerializeImports(string path, string package)
        {
            return string.IsNullOrEmpty(package) ? string.Format(SerializePathImportsFormat, path) : string.Format(SerializePackageImportsFormat, package);
        }

        public const string SerializePathImportsFormat = @"import BeanBase from '{0}/serialization/BeanBase'";
        public const string SerializePackageImportsFormat = @"import {{BeanBase}} from '{0}'";

        public static string GetProtocolImports(string path, string package)
        {
            return string.IsNullOrEmpty(package) ? string.Format(ProtocolPathImportsFormat, path) : string.Format(ProtocolPackageImportsFormat, package);
        }
        public const string ProtocolPathImportsFormat = "import Protocol from '{0}/net/Protocol'";
        public const string ProtocolPackageImportsFormat = "import {{Protocol}} from '{0}'";

        public const string SerializeTypes = @"
export interface ISerializable {
    serialize(buf: ByteBuf): void
    deserialize(buf: ByteBuf): void
}

export abstract class BeanBase implements ISerializable {
    abstract getTypeId(): number
    abstract serialize(buf: Bright.Serialization.ByteBuf): void
    abstract deserialize(buf: Bright.Serialization.ByteBuf): void
}
";

        public const string ProtoTypes = @"
export abstract class Protocol implements ISerializable {
    abstract getTypeId(): number
    abstract serialize(buf: ByteBuf): void
    abstract deserialize(buf: ByteBuf): void
}
";
    }
}
