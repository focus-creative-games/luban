namespace Luban.Encryption;

public static class EncryptionUtil
{
    public static byte[] Encrypt(string keySuffix, byte[] source)
    {
        string primaryKey = EnvManager.Current.GetOptionOrDefault("encryption", "primaryKey", false, "qwerty");
        string secondaryKey = EnvManager.Current.GetOptionOrDefault("encryption", "secondaryKey", false, "zxcv");
        return XXTEA.Encrypt(source, XXTEA.Encrypt(secondaryKey + keySuffix, primaryKey));
    }
}