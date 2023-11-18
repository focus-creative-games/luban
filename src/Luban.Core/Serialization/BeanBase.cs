namespace Luban.Serialization;

public abstract class BeanBase : ITypeId, ISerializable
{
    public abstract int GetTypeId();

    public abstract void Serialize(ByteBuf os);

    public abstract void Deserialize(ByteBuf os);
}
