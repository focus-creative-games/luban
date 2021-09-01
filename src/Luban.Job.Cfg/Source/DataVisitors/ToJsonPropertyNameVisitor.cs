using Luban.Job.Cfg.Datas;
using System;

namespace Luban.Job.Cfg.DataVisitors
{
    class ToJsonPropertyNameVisitor : IDataFuncVisitor<string>
    {
        public static ToJsonPropertyNameVisitor Ins { get; } = new();

        public string Accept(DBool type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DByte type)
        {
            return type.Value.ToString();
        }

        public string Accept(DShort type)
        {
            return type.Value.ToString();
        }

        public string Accept(DFshort type)
        {
            return type.Value.ToString();
        }

        public string Accept(DInt type)
        {
            return type.Value.ToString();
        }

        public string Accept(DFint type)
        {
            return type.Value.ToString();
        }

        public string Accept(DLong type)
        {
            return type.Value.ToString();
        }

        public string Accept(DFlong type)
        {
            return type.Value.ToString();
        }

        public string Accept(DFloat type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DDouble type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DEnum type)
        {
            return type.Value.ToString();
        }

        public string Accept(DString type)
        {
            return type.Value.ToString();
        }

        public string Accept(DBytes type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DText type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DBean type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DArray type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DList type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DSet type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DMap type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DVector2 type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DVector3 type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DVector4 type)
        {
            throw new NotSupportedException();
        }

        public string Accept(DDateTime type)
        {
            throw new NotSupportedException();
        }
    }
}
