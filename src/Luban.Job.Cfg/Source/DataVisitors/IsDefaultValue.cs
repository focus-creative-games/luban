using Luban.Job.Cfg.Datas;
using System;
using System.Numerics;

namespace Luban.Job.Cfg.DataVisitors
{
    class IsDefaultValue : IDataFuncVisitor<bool>
    {
        public static IsDefaultValue Ins { get; } = new IsDefaultValue();

        public bool Accept(DBool type)
        {
            return type.Value == false;
        }

        public bool Accept(DByte type)
        {
            return type.Value == 0;
        }

        public bool Accept(DShort type)
        {
            return type.Value == 0;
        }

        public bool Accept(DFshort type)
        {
            return type.Value == 0;
        }

        public bool Accept(DInt type)
        {
            return type.Value == 0;
        }

        public bool Accept(DFint type)
        {
            return type.Value == 0;
        }

        public bool Accept(DLong type)
        {
            return type.Value == 0;
        }

        public bool Accept(DFlong type)
        {
            return type.Value == 0;
        }

        public bool Accept(DFloat type)
        {
            return type.Value == 0;
        }

        public bool Accept(DDouble type)
        {
            return type.Value == 0;
        }

        public bool Accept(DEnum type)
        {
            return type.Value == 0;
        }

        public bool Accept(DString type)
        {
            return string.IsNullOrEmpty(type.Value);
        }

        public bool Accept(DBytes type)
        {
            throw new NotSupportedException();
        }

        public bool Accept(DText type)
        {
            return string.IsNullOrEmpty(type.Key);
        }

        public bool Accept(DBean type)
        {
            return false;
        }

        public bool Accept(DArray type)
        {
            return type.Datas.Count == 0;
        }

        public bool Accept(DList type)
        {
            return type.Datas.Count == 0;
        }

        public bool Accept(DSet type)
        {
            return type.Datas.Count == 0;
        }

        public bool Accept(DMap type)
        {
            return type.Datas.Count == 0;
        }

        public bool Accept(DVector2 type)
        {
            return type.Value == Vector2.Zero;
        }

        public bool Accept(DVector3 type)
        {
            return type.Value == Vector3.Zero;
        }

        public bool Accept(DVector4 type)
        {
            return type.Value == Vector4.Zero;
        }

        public bool Accept(DDateTime type)
        {
            return false;
        }
    }
}
