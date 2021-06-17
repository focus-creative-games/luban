using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Luban.Job.Cfg.TypeVisitors
{
    public class JsonDataCreator : ITypeFuncVisitor<JsonElement, DefAssembly, DType>
    {
        public static JsonDataCreator Ins { get; } = new JsonDataCreator();

        public DType Accept(TBool type, JsonElement x, DefAssembly ass)
        {
            return new DBool(x.GetBoolean());
        }

        public DType Accept(TByte type, JsonElement x, DefAssembly ass)
        {
            return new DByte(x.GetByte());
        }

        public DType Accept(TShort type, JsonElement x, DefAssembly ass)
        {
            return new DShort(x.GetInt16());
        }

        public DType Accept(TFshort type, JsonElement x, DefAssembly ass)
        {
            return new DFshort(x.GetInt16());
        }

        public DType Accept(TInt type, JsonElement x, DefAssembly ass)
        {
            return new DInt(x.GetInt32());
        }

        public DType Accept(TFint type, JsonElement x, DefAssembly ass)
        {
            return new DFint(x.GetInt32());
        }

        public DType Accept(TLong type, JsonElement x, DefAssembly ass)
        {
            return new DLong(x.GetInt64());
        }

        public DType Accept(TFlong type, JsonElement x, DefAssembly ass)
        {
            return new DFlong(x.GetInt64());
        }

        public DType Accept(TFloat type, JsonElement x, DefAssembly ass)
        {
            return new DFloat(x.GetSingle());
        }

        public DType Accept(TDouble type, JsonElement x, DefAssembly ass)
        {
            return new DDouble(x.GetDouble());
        }

        public DType Accept(TEnum type, JsonElement x, DefAssembly ass)
        {
            return new DEnum(type, x.GetString());
        }

        public DType Accept(TString type, JsonElement x, DefAssembly ass)
        {
            return new DString(x.GetString());
        }

        public DType Accept(TBytes type, JsonElement x, DefAssembly ass)
        {
            throw new NotImplementedException();
        }

        public DType Accept(TText type, JsonElement x, DefAssembly ass)
        {
            if (!x.TryGetProperty("key", out var keyEle))
            {
                throw new Exception("text 缺失 key属性");
            }
            if (!x.TryGetProperty("text", out var textEle))
            {
                throw new Exception("text 缺失 text属性");
            }
            string key = keyEle.GetString();
            if (key == null)
            {
                throw new Exception("text key不是字符串");
            }
            string text = textEle.GetString();
            if (text == null)
            {
                throw new Exception("text text不是字符串");
            }

            ass.AddText(key, text);
            return new DText(key, text);
        }

        public DType Accept(TBean type, JsonElement x, DefAssembly ass)
        {
            var bean = (DefBean)type.Bean;

            DefBean implBean;
            if (bean.IsAbstractType)
            {
                if (!x.TryGetProperty(DefBean.TYPE_NAME_KEY, out var typeNameProp))
                {
                    throw new Exception($"结构:{bean.FullName} 是多态类型，必须用 {DefBean.TYPE_NAME_KEY} 字段指定 子类名");
                }
                string subType = typeNameProp.GetString();
                var fullName = TypeUtil.MakeFullName(bean.Namespace, subType);
                var defType = (DefBean)bean.GetNotAbstractChildType(subType);
                //if (defType.IsAbstractType)
                //{
                //    throw new Exception($"type:{fullName} 是抽象类. 不能创建实例");
                //}
                implBean = defType ?? throw new Exception($"type:{fullName} 不是合法类型");
            }
            else
            {
                implBean = bean;
            }

            var fields = new List<DType>();
            foreach (var field in implBean.HierarchyFields)
            {
                if (x.TryGetProperty(field.Name, out var ele))
                {
                    try
                    {
                        fields.Add(field.CType.Apply(this, ele, ass));
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"结构:{implBean.FullName} 字段:{field.Name} 读取失败 => {e.Message}", e);
                    }
                }
                else
                {
                    throw new Exception($"结构:{implBean.FullName} 字段:{field.Name} 缺失");
                }
            }
            return new DBean(bean, implBean, fields);
        }

        private List<DType> ReadList(TType type, JsonElement e, DefAssembly ass)
        {
            var list = new List<DType>();
            foreach (var c in e.EnumerateArray())
            {
                list.Add(type.Apply(this, c, ass));
            }
            return list;
        }

        public DType Accept(TArray type, JsonElement x, DefAssembly ass)
        {
            return new DArray(type, ReadList(type.ElementType, x, ass));
        }

        public DType Accept(TList type, JsonElement x, DefAssembly ass)
        {
            return new DList(type, ReadList(type.ElementType, x, ass));
        }

        public DType Accept(TSet type, JsonElement x, DefAssembly ass)
        {
            return new DSet(type, ReadList(type.ElementType, x, ass));
        }

        public DType Accept(TMap type, JsonElement x, DefAssembly ass)
        {
            var map = new Dictionary<DType, DType>();
            foreach (var e in x.EnumerateArray())
            {
                if (e.GetArrayLength() != 2)
                {
                    throw new ArgumentException($"json map 类型的 成员数据项:{e} 必须是 [key,value] 形式的列表");
                }
                DType key = type.KeyType.Apply(this, e[0], ass);
                DType value = type.ValueType.Apply(this, e[1], ass);
                if (!map.TryAdd(key, value))
                {
                    throw new Exception($"map 的 key:{key} 重复");
                }
            }
            return new DMap(type, map);
        }

        public DType Accept(TDateTime type, JsonElement x, DefAssembly ass)
        {
            return DataUtil.CreateDateTime(x.GetString(), ass.TimeZone);
        }

        public DType Accept(TVector2 type, JsonElement x, DefAssembly ass)
        {
            return new DVector2(new System.Numerics.Vector2(x.GetProperty("x").GetSingle(), x.GetProperty("y").GetSingle()));
        }

        public DType Accept(TVector3 type, JsonElement x, DefAssembly ass)
        {
            return new DVector3(new System.Numerics.Vector3(x.GetProperty("x").GetSingle(), x.GetProperty("y").GetSingle(), x.GetProperty("z").GetSingle()));
        }

        public DType Accept(TVector4 type, JsonElement x, DefAssembly ass)
        {
            return new DVector4(new System.Numerics.Vector4(x.GetProperty("x").GetSingle(), x.GetProperty("y").GetSingle(), x.GetProperty("z").GetSingle(), x.GetProperty("w").GetSingle()));
        }
    }
}
