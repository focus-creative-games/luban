using Luban.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.CSharp;

public static class CSharpUtil
{
    public static string GetFullNameWithGlobalQualifier(DefTypeBase type)
    {
        return $"global::{type.FullNameWithTopModule}";
    }
}
