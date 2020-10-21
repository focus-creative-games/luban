using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.DataVisitors
{
    class ResourceExportor : IDataActionVisitor<DefField, List<ResourceInfo>>
    {
        public static ResourceExportor Ins { get; } = new ResourceExportor();

        public void Accept(DBool type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DByte type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DShort type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DFshort type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DInt type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DFint type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DLong type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DFlong type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DFloat type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DDouble type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DEnum type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DString type, DefField x, List<ResourceInfo> y)
        {
            if (!string.IsNullOrEmpty(type.Value))
            {
                y.Add(new ResourceInfo() { Resource = type.Value, Tag = x.ResourceTag });
            }
        }

        public void Accept(DBytes type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DText type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DBean type, DefField _, List<ResourceInfo> y)
        {
            var def = type.ImplType;
            if (def == null)
            {
                return;
            }
            int index = 0;
            foreach (DType fieldData in type.Fields)
            {
                var fieldDef = (DefField)def.HierarchyFields[index++];
                if (fieldDef.IsResource)
                {
                    fieldData.Apply(this, fieldDef, y);
                }
            }
        }

        private void Accept(DefField def, List<DType> datas, TType elementType, List<ResourceInfo> ress)
        {
            if (def.IsResource || (elementType is TBean))
            {
                foreach (var e in datas)
                {
                    e.Apply(this, def, ress);
                }
            }
        }

        public void Accept(DArray type, DefField x, List<ResourceInfo> y)
        {
            Accept(x, type.Datas, type.Type.ElementType, y);
        }

        public void Accept(DList type, DefField x, List<ResourceInfo> y)
        {
            Accept(x, type.Datas, type.Type.ElementType, y);
        }

        public void Accept(DSet type, DefField x, List<ResourceInfo> y)
        {
            Accept(x, type.Datas, type.Type.ElementType, y);
        }

        public void Accept(DMap type, DefField x, List<ResourceInfo> y)
        {
            if (x.IsResource || (type.Type.ValueType is TBean))
            {
                foreach (var e in type.Datas.Values)
                {
                    e.Apply(this, x, y);
                }
            }
        }

        public void Accept(DVector2 type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DVector3 type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DVector4 type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }

        public void Accept(DDateTime type, DefField x, List<ResourceInfo> y)
        {
            throw new NotImplementedException();
        }
    }
}
