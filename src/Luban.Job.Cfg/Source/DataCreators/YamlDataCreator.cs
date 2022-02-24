using Bright.Collections;
using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Luban.Job.Cfg.DataCreators
{
    class YamlDataCreator : ITypeFuncVisitor<YamlNode, DefAssembly, DType>
    {
        public static YamlDataCreator Ins { get; } = new();

        private static string GetLowerTextValue(YamlNode node)
        {
            return ((string)node).Trim().ToLower();
        }

        private static string GetTextValue(YamlNode node)
        {
            return node.ToString();
        }

        public DType Accept(TBool type, YamlNode x, DefAssembly y)
        {
            return DBool.ValueOf(bool.Parse(GetLowerTextValue(x)));
        }

        public DType Accept(TByte type, YamlNode x, DefAssembly y)
        {
            return DByte.ValueOf(byte.Parse(GetLowerTextValue(x)));
        }

        public DType Accept(TShort type, YamlNode x, DefAssembly y)
        {
            return DShort.ValueOf(short.Parse(GetLowerTextValue(x)));
        }

        public DType Accept(TFshort type, YamlNode x, DefAssembly y)
        {
            return DFshort.ValueOf(short.Parse(GetLowerTextValue(x)));
        }

        public DType Accept(TInt type, YamlNode x, DefAssembly y)
        {
            return DInt.ValueOf(int.Parse(GetLowerTextValue(x)));
        }

        public DType Accept(TFint type, YamlNode x, DefAssembly y)
        {
            return DFint.ValueOf(int.Parse(GetLowerTextValue(x)));
        }

        public DType Accept(TLong type, YamlNode x, DefAssembly y)
        {
            return DLong.ValueOf(long.Parse(GetLowerTextValue(x)));
        }

        public DType Accept(TFlong type, YamlNode x, DefAssembly y)
        {
            return DFlong.ValueOf(long.Parse(GetLowerTextValue(x)));
        }

        public DType Accept(TFloat type, YamlNode x, DefAssembly y)
        {
            return DFloat.ValueOf(float.Parse(GetLowerTextValue(x)));
        }

        public DType Accept(TDouble type, YamlNode x, DefAssembly y)
        {
            return DDouble.ValueOf(double.Parse(GetLowerTextValue(x)));
        }

        public DType Accept(TEnum type, YamlNode x, DefAssembly y)
        {
            return new DEnum(type, GetTextValue(x));
        }

        public DType Accept(TString type, YamlNode x, DefAssembly y)
        {
            return DString.ValueOf(GetTextValue(x));
        }

        public DType Accept(TBytes type, YamlNode x, DefAssembly y)
        {
            throw new NotSupportedException();
        }

        private static readonly YamlScalarNode s_keyNodeName = new("key");
        private static readonly YamlScalarNode s_textNodeName = new("text");

        public DType Accept(TText type, YamlNode x, DefAssembly y)
        {
            var key = GetTextValue(x[s_keyNodeName]);
            var text = GetTextValue(x[s_textNodeName]);
            DataUtil.ValidateText(key, text);
            return new DText(key, text);
        }

        private static readonly YamlScalarNode s_typeNodeName = new(DefBean.JSON_TYPE_NAME_KEY);
        private static readonly YamlScalarNode s_typeNodeNameFallback = new(DefBean.FALLBACK_TYPE_NAME_KEY);

        public DType Accept(TBean type, YamlNode x, DefAssembly y)
        {
            var m = (YamlMappingNode)x;
            var bean = (DefBean)type.Bean;

            DefBean implBean;
            if (bean.IsAbstractType)
            {
                if (!m.Children.TryGetValue(s_typeNodeName, out var typeNode) && !m.Children.TryGetValue(s_typeNodeNameFallback, out typeNode))
                {
                    throw new Exception($"bean:'{bean.FullName}'是多态，需要指定{DefBean.JSON_TYPE_NAME_KEY}属性.\n xml:{x}");
                }
                string subType = (string)typeNode;
                if (string.IsNullOrWhiteSpace(subType))
                {
                    throw new Exception($"bean:'{bean.FullName}'是多态，需要指定{DefBean.JSON_TYPE_NAME_KEY}属性.\n xml:{x}");
                }
                implBean = DataUtil.GetImplTypeByNameOrAlias(bean, subType);
            }
            else
            {
                implBean = bean;
            }

            var fields = new List<DType>();
            foreach (DefField f in implBean.HierarchyFields)
            {
                if (!m.Children.TryGetValue(new YamlScalarNode(f.Name), out var fele))
                {
                    if (f.CType.IsNullable)
                    {
                        fields.Add(null);
                        continue;
                    }
                    throw new Exception($"bean:{implBean.FullName} 字段:{f.Name} 缺失");
                }
                try
                {
                    fields.Add(f.CType.Apply(this, fele, y));
                }
                catch (DataCreateException dce)
                {
                    dce.Push(implBean, f);
                    throw;
                }
                catch (Exception e)
                {
                    var dce = new DataCreateException(e, "");
                    dce.Push(bean, f);
                    throw dce;
                }

            }
            return new DBean(type, implBean, fields);
        }

        private List<DType> ReadList(TType type, YamlSequenceNode x, DefAssembly ass)
        {
            var list = new List<DType>();
            foreach (var e in x)
            {
                list.Add(type.Apply(this, e, ass));
            }
            return list;
        }

        public DType Accept(TArray type, YamlNode x, DefAssembly y)
        {
            return new DArray(type, ReadList(type.ElementType, (YamlSequenceNode)x, y));
        }

        public DType Accept(TList type, YamlNode x, DefAssembly y)
        {
            return new DList(type, ReadList(type.ElementType, (YamlSequenceNode)x, y));
        }

        public DType Accept(TSet type, YamlNode x, DefAssembly y)
        {
            return new DSet(type, ReadList(type.ElementType, (YamlSequenceNode)x, y));
        }

        public DType Accept(TMap type, YamlNode x, DefAssembly y)
        {
            var m = (YamlSequenceNode)x;
            var map = new Dictionary<DType, DType>();
            foreach (var e in m)
            {
                var kv = (YamlSequenceNode)e;
                if (kv.Count() != 2)
                {
                    throw new ArgumentException($"yaml map 类型的 成员数据项:{e} 必须是 [key,value] 形式的列表");
                }
                DType key = type.KeyType.Apply(this, kv[0], y);
                DType value = type.ValueType.Apply(this, kv[1], y);
                if (!map.TryAdd(key, value))
                {
                    throw new Exception($"map 的 key:{key} 重复");
                }
            }
            return new DMap(type, map);
        }

        private static readonly YamlScalarNode s_xNodeName = new("x");
        private static readonly YamlScalarNode s_yNodeName = new("y");
        private static readonly YamlScalarNode s_zNodeName = new("z");
        private static readonly YamlScalarNode s_wNodeName = new("w");

        private static float ParseChildFloatValue(YamlNode node, YamlScalarNode keyName)
        {
            return float.Parse(GetLowerTextValue(node[keyName]));
        }

        public DType Accept(TVector2 type, YamlNode x, DefAssembly y)
        {
            return new DVector2(new System.Numerics.Vector2(ParseChildFloatValue(x, s_xNodeName), ParseChildFloatValue(x, s_yNodeName)));
        }

        public DType Accept(TVector3 type, YamlNode x, DefAssembly y)
        {
            return new DVector3(new System.Numerics.Vector3(
                ParseChildFloatValue(x, s_xNodeName),
                ParseChildFloatValue(x, s_yNodeName),
                ParseChildFloatValue(x, s_zNodeName)));
        }

        public DType Accept(TVector4 type, YamlNode x, DefAssembly y)
        {
            return new DVector4(new System.Numerics.Vector4(
                ParseChildFloatValue(x, s_xNodeName),
                ParseChildFloatValue(x, s_yNodeName),
                ParseChildFloatValue(x, s_zNodeName),
                ParseChildFloatValue(x, s_wNodeName)));
        }

        public DType Accept(TDateTime type, YamlNode x, DefAssembly y)
        {
            return DataUtil.CreateDateTime(GetLowerTextValue(x));
        }
    }
}
