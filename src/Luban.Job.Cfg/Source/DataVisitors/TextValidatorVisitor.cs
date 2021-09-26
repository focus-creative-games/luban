using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.l10n;

namespace Luban.Job.Cfg.DataVisitors
{

    /// <summary>
    /// 检查 相同key的text,原始值必须相同
    /// </summary>
    class TextValidatorVisitor : IDataActionVisitor<RawTextTable>
    {
        public static TextValidatorVisitor Ins { get; } = new TextValidatorVisitor();

        public void Accept(DBool type, RawTextTable x)
        {

        }

        public void Accept(DByte type, RawTextTable x)
        {

        }

        public void Accept(DShort type, RawTextTable x)
        {

        }

        public void Accept(DFshort type, RawTextTable x)
        {

        }

        public void Accept(DInt type, RawTextTable x)
        {

        }

        public void Accept(DFint type, RawTextTable x)
        {

        }

        public void Accept(DLong type, RawTextTable x)
        {

        }

        public void Accept(DFlong type, RawTextTable x)
        {

        }

        public void Accept(DFloat type, RawTextTable x)
        {

        }

        public void Accept(DDouble type, RawTextTable x)
        {

        }

        public void Accept(DEnum type, RawTextTable x)
        {

        }

        public void Accept(DString type, RawTextTable x)
        {

        }

        public void Accept(DBytes type, RawTextTable x)
        {

        }

        public void Accept(DText type, RawTextTable x)
        {
            x.AddText(ValidatorContext.CurrentVisitor.CurrentValidateRecord, type.Key, type.RawValue);
        }

        public void Accept(DBean type, RawTextTable x)
        {

        }

        public void Accept(DArray type, RawTextTable x)
        {

        }

        public void Accept(DList type, RawTextTable x)
        {

        }

        public void Accept(DSet type, RawTextTable x)
        {

        }

        public void Accept(DMap type, RawTextTable x)
        {

        }

        public void Accept(DVector2 type, RawTextTable x)
        {

        }

        public void Accept(DVector3 type, RawTextTable x)
        {

        }

        public void Accept(DVector4 type, RawTextTable x)
        {

        }

        public void Accept(DDateTime type, RawTextTable x)
        {

        }
    }
}
