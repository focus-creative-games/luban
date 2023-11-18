namespace Luban.Serialization;

/// <summary>
/// 非兼容binary序列化
/// </summary>
public interface ISerializable
{
    void Serialize(ByteBuf os);

    void Deserialize(ByteBuf os);
}
