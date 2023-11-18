namespace Luban.Utils;

public static class ExternalTypeUtil
{

    //protected void ResolveExternalType()
    //{
    //    if (!string.IsNullOrEmpty(_externalTypeName))
    //    {
    //        if (Assembly.TryGetExternalType(_externalTypeName, out var type))
    //        {
    //            this.ExternalType = type;
    //        }
    //        else
    //        {
    //            throw new Exception($"enum:'{FullName}' 对应的 externaltype:{_externalTypeName} 不存在");
    //        }
    //    }
    //}

    // public static ExternalTypeMapper GetExternalTypeMappfer(string typeName)
    // {
    //     return GenerationContext.Ins.GetExternalTypeMapper(typeName);
    // }

    // public static string CsMapperToExternalType(DefTypeBase type)
    // {
    //     var mapper = DefAssembly.LocalAssebmly.GetExternalTypeMapper(type.FullName);
    //     return mapper != null ? mapper.TargetTypeName : type.CsFullName;
    // }
    //
    // public static string CsCloneToExternal(string typeName, string src)
    // {
    //     var mapper = DefAssembly.LocalAssebmly.GetExternalTypeMapper(typeName);
    //     if (mapper == null)
    //     {
    //         return src;
    //     }
    //     if (string.IsNullOrWhiteSpace(mapper.CreateExternalObjectFunction))
    //     {
    //         throw new Exception($"type:{typeName} externaltype:{DefAssembly.LocalAssebmly.GetExternalType(typeName)} lan:{mapper.Lan} selector:{mapper.Selector} 未定义 create_external_object_function 属性");
    //     }
    //     return $"{mapper.CreateExternalObjectFunction}({src})";
    // }
}
