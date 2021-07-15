using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.Utils
{
    public static class TypescriptStringTemplate
    {
        public const string PuertsByteBufImports = @"
import {Bright} from 'csharp'
import ByteBuf = Bright.Serialization.ByteBuf";

        public const string BrightByteBufImportsFormat = "import ByteBuf from '{0}/serialization/ByteBuf'";

        public const string VectorImportsFormat = @"
import Vector2 from '{0}/math/Vector2'
import Vector3 from '{0}/math/Vector3'
import Vector4 from '{0}/math/Vector4'
";
        public const string SerializeImportsFormat = @"import BeanBase from '{0}/serialization/BeanBase'";

        public const string ProtocolImportsFormat = "import Protocol from '{0}/net/Protocol'";

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

        public const string VectorTypesJson = @"

export class Vector2 {
    static deserializeFromJson(json: any): Vector2 {
        let x = json['x']
        let y = json['y']
        if (x == null || y == null) {
            throw new Error()
        }
        return new Vector2(x, y)
    }

    x: number
    y: number
    constructor(x: number = 0, y: number = 0) {
        this.x = x
        this.y = y
    }
}

export class Vector3 {
    static deserializeFromJson(json: any): Vector3 {
        let x = json['x']
        let y = json['y']
        let z = json['z']
        if (x == null || y == null || z == null) {
            throw new Error()
        }
        return new Vector3(x, y, z)
    }

    x: number
    y: number
    z: number

    constructor(x: number = 0, y: number = 0, z: number = 0) {
        this.x = x
        this.y = y
        this.z = z
    }
}

export class Vector4 {
    static deserializeFromJson(json: any): Vector4 {
        let x = json['x']
        let y = json['y']
        let z = json['z']
        let w = json['w']
        if (x == null || y == null || z == null || w == null) {
            throw new Error()
        }
        return new Vector4(x, y, z, w)
    }
    
    x: number
    y: number
    z: number
    w: number

    constructor(x: number = 0, y: number = 0, z: number = 0, w: number = 0) {
        this.x = x
        this.y = y
        this.z = z
        this.w = w
    }
}

";

        public const string VectorTypesByteBuf = @"

export class Vector2 implements ISerializable  {

    static deserializeFrom(buf: ByteBuf): Vector2 {
        var v = new Vector2()
        v.deserialize(buf)
        return v
    }

    x: number
    y: number
    constructor(x: number = 0, y: number = 0) {
        this.x = x
        this.y = y
    }

    serialize(_buf_: ByteBuf) {
        _buf_.WriteFloat(this.x)
        _buf_.WriteFloat(this.y)
    }

    deserialize(buf: ByteBuf) {
        this.x = buf.ReadFloat()
        this.y = buf.ReadFloat()
    }
}

export class Vector3 implements ISerializable{
    static deserializeFrom(buf: ByteBuf): Vector3 {
        var v = new Vector3()
        v.deserialize(buf)
        return v
    }

    x: number
    y: number
    z: number

    constructor(x: number = 0, y: number = 0, z: number = 0) {
        this.x = x
        this.y = y
        this.z = z
    }

    serialize(_buf_: ByteBuf) {
        _buf_.WriteFloat(this.x)
        _buf_.WriteFloat(this.y)
        _buf_.WriteFloat(this.z)
    }

    deserialize(buf: ByteBuf) {
        this.x = buf.ReadFloat()
        this.y = buf.ReadFloat()
        this.z = buf.ReadFloat()
    }
}

export class Vector4 implements ISerializable {
    static deserializeFrom(buf: ByteBuf): Vector4 {
        var v = new Vector4()
        v.deserialize(buf)
        return v
    }
    
    x: number
    y: number
    z: number
    w: number

    constructor(x: number = 0, y: number = 0, z: number = 0, w: number = 0) {
        this.x = x
        this.y = y
        this.z = z
        this.w = w
    }

    serialize(_buf_: ByteBuf) {
        _buf_.WriteFloat(this.x)
        _buf_.WriteFloat(this.y)
        _buf_.WriteFloat(this.z)
        _buf_.WriteFloat(this.w)
    }

    deserialize(buf: ByteBuf) {
        this.x = buf.ReadFloat()
        this.y = buf.ReadFloat()
        this.z = buf.ReadFloat()
        this.z = buf.ReadFloat()
    }
}

";
    }
}
