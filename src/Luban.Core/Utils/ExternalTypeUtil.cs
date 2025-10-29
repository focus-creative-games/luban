// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

﻿namespace Luban.Utils;

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
