using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Bright.Common
{
    public class ParamInfo
    {
        public Type Type { get; set; }

        public bool ChildType { get; set; }
    }

    public class SignatureInfo
    {
        public Type ReturnType { get; set; }

#pragma warning disable CA2227 // 集合属性应为只读
        public List<ParamInfo> Params { get; set; }
#pragma warning restore CA2227 // 集合属性应为只读

        public Action<Delegate, Attribute> Callback { get; set; }

        public bool CanBeStatic { get; set; }
    }

    public class ReflectionException : Exception
    {
        public ReflectionException()
        {
        }

        public ReflectionException(string message) : base(message)
        {
        }

        public ReflectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReflectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public static class ReflectionUtil
    {
        public static Type GetType(string name)
        {
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = a.GetType(name);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        public static Type[] GetSubTypes(Assembly assembly, Type type)
        {
            return assembly.GetTypes().Where(t => type.IsAssignableFrom(t) && !t.IsAbstract).ToArray();
        }

        public static Type[] GetTypesByAttr(Assembly assembly, Type attr)
        {
            return assembly.GetTypes().Where(t => t.GetCustomAttribute(attr) != null).ToArray();
        }

        public static MethodInfo[] GetMethodsByAttr(Type type, Type attr, bool includeStatic = false)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            if (includeStatic)
            {
                flags |= BindingFlags.Static;
            }
            return type.GetMethods(flags).Where(m => m.GetCustomAttribute(attr) != null).ToArray();
        }

        public static Type[] GetCallingSubTypes(Type type)
        {
            return GetSubTypes(Assembly.GetCallingAssembly(), type);
        }

        public static Type[] GetCallingTypesByAttr(Type attr)
        {
            return GetTypesByAttr(Assembly.GetCallingAssembly(), attr);
        }

        public static Type MakeGenericActionFromMethod(MethodInfo method)
        {
            var argTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            return argTypes.Length switch
            {
                0 => typeof(Action),
                1 => typeof(Action<>).MakeGenericType(argTypes),
                2 => typeof(Action<,>).MakeGenericType(argTypes),
                3 => typeof(Action<,,>).MakeGenericType(argTypes),
                4 => typeof(Action<,,,>).MakeGenericType(argTypes),
                5 => typeof(Action<,,,,>).MakeGenericType(argTypes),
                _ => throw new NotImplementedException(),
            };
        }

        public static Type MakeGenericFuncFromMethod(MethodInfo method)
        {
            var returnType = method.ReturnType;
            var argTypes = method.GetParameters().Select(p => p.ParameterType).Concat(new Type[] { returnType }).ToArray();
            return argTypes.Length switch
            {
                1 => typeof(Func<>).MakeGenericType(argTypes),
                2 => typeof(Func<,>).MakeGenericType(argTypes),
                3 => typeof(Func<,,>).MakeGenericType(argTypes),
                4 => typeof(Func<,,,>).MakeGenericType(argTypes),
                5 => typeof(Func<,,,,>).MakeGenericType(argTypes),
                6 => typeof(Func<,,,,,>).MakeGenericType(argTypes),
                _ => throw new NotImplementedException(),
            };
        }



        public static void ScanHandler(Type type, object self, Type attr, params SignatureInfo[] signatures)
        {
            foreach (var method in GetMethodsByAttr(type, attr, true))
            {
                bool match = false;
                foreach (var s in signatures)
                {
                    var ps = method.GetParameters();
                    if (method.ReturnType == s.ReturnType && ps.Length == s.Params.Count)
                    {
                        bool allMatch = true;
                        for (int i = 0; i < ps.Length; i++)
                        {
                            var p = s.Params[i];
                            if ((p.ChildType ? !p.Type.IsAssignableFrom(ps[i].ParameterType) : p.Type != ps[i].ParameterType))
                            {
                                allMatch = false;
                                break;
                            }
                        }
                        if (!allMatch)
                        {
                            continue;
                        }

                        if (allMatch && method.IsStatic && !s.CanBeStatic)
                        {
                            throw new ReflectionException($"type:{type} method:{method} can't be static");
                        }
                        match = true;

                        var del = Delegate.CreateDelegate((method.ReturnType == typeof(void) ? MakeGenericActionFromMethod(method) : MakeGenericFuncFromMethod(method)),
                            method.IsStatic ? null : self, method);
                        s.Callback(del, method.GetCustomAttribute(attr));
                        break;
                    }
                }
                if (!match)
                {
                    throw new ReflectionException($"type{type} method:{method} invalid signature");
                }
            }
        }

        public static Action<TR> CastAction<TS, TR>(Delegate del) where TS : TR
        {
            var action = (Action<TS>)del;
            return (TR a) => action((TS)a);
        }


        //public static Action<TR> CastAction<TR>(Delegate del, Type sourceType)
        //{
        //    var castAction = typeof(ReflectionUtil).GetMethod(nameof(CastAction), 2, new Type[] { typeof(Delegate) });
        //    return (Action<TR>)castAction.MakeGenericMethod(sourceType, typeof(TR)).Invoke(null, new object[] { del });
        //}

        public static Action<TR1, TR2> CastAction<TS1, TR1, TS2, TR2>(Delegate del) where TS1 : TR1 where TS2 : TR2
        {
            var action = (Action<TS1, TS2>)del;
            return (TR1 a, TR2 b) => action((TS1)a, (TS2)b);
        }

        public static Action<TR1, TR2, TR3> CastAction<TS1, TR1, TS2, TR2, TS3, TR3>(Delegate del) where TS1 : TR1 where TS2 : TR2 where TS3 : TR3
        {
            var action = (Action<TS1, TS2, TS3>)del;
            return (TR1 a, TR2 b, TR3 c) => action((TS1)a, (TS2)b, (TS3)c);
        }

        //public static void RegisterProtoHandler(Assembly assembly, HandlerDispatcher dispatcher)
        //{
        //    foreach (var type in GetTypesByAttr(assembly, typeof(ProtoHandlerAttribute)))
        //    {
        //        var handler = (ProtoHandler)Activator.CreateInstance(type);
        //        s_logger.ConditionalDebug("RegisterProtoHandler {0} {1}", type, handler);
        //        dispatcher.Register(type.BaseType.GetGenericArguments()[0], handler.Process);
        //    }
        //}
    }
}
