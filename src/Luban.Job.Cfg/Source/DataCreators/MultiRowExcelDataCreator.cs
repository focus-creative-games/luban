//using Luban.Job.Cfg.Datas;
//using Luban.Job.Cfg.DataSources.Excel;
//using Luban.Job.Cfg.Defs;
//using Luban.Job.Common.Types;
//using Luban.Job.Common.TypeVisitors;
//using System;
//using System.Collections.Generic;

//namespace Luban.Job.Cfg.DataCreators
//{
//    class MultiRowExcelDataCreator : ITypeFuncVisitor<IEnumerable<ExcelStream>, bool, DefAssembly, DType>
//    {
//        public static MultiRowExcelDataCreator Ins { get; } = new MultiRowExcelDataCreator();

//        public DType Accept(TBool type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TByte type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TShort type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TFshort type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TInt type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TFint type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TLong type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TFlong type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TFloat type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TDouble type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TEnum type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TString type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TBytes type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TText type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TBean type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        private List<DType> ReadMultiRow(TType type, IEnumerable<ExcelStream> rows, DefAssembly ass)
//        {
//            var list = new List<DType>();
//            foreach (var stream in rows)
//            {
//                try
//                {
//                    list.Add(type.Apply(ExcelStreamDataCreator.Ins, null, stream, ass));
//                }
//                catch (Exception e)
//                {
//                    var dce = new DataCreateException(e, stream.LastReadDataInfo);
//                    throw dce;
//                }
//            }
//            return list;
//        }

//        public DType Accept(TArray type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            return new DArray(type, ReadMultiRow(type.ElementType, x, ass));
//        }

//        public DType Accept(TList type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            return new DList(type, ReadMultiRow(type.ElementType, x, ass));
//        }

//        public DType Accept(TSet type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            return new DSet(type, ReadMultiRow(type.ElementType, x, ass));
//        }

//        public DType Accept(TMap type, IEnumerable<ExcelStream> rows, bool y, DefAssembly ass)
//        {
//            var map = new Dictionary<DType, DType>();
//            foreach (var stream in rows)
//            {
//                try
//                {
//                    DType key = type.KeyType.Apply(ExcelStreamDataCreator.Ins, null, stream, ass);
//                    DType value = type.ValueType.Apply(ExcelStreamDataCreator.Ins, null, stream, ass);
//                    map.Add(key, value);
//                }
//                catch (Exception e)
//                {
//                    var dce = new DataCreateException(e, stream.LastReadDataInfo);
//                    throw dce;
//                }
//            }
//            return new DMap(type, map);
//        }

//        public DType Accept(TVector2 type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TVector3 type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TVector4 type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }

//        public DType Accept(TDateTime type, IEnumerable<ExcelStream> x, bool y, DefAssembly ass)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
