using Luban.Job.Cfg.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luban.Job.Cfg.DataSources.Excel
{
    class ExcelStream
    {

        private readonly List<Sheet.Cell> _datas;
        private readonly int _toIndex;
        private int _curIndex;


        /// <summary>
        /// NamedMode下 string可以用空白表达空字符串，而不必用null或""
        /// </summary>
        public bool NamedMode { get; set; }

        public ExcelStream(List<Sheet.Cell> datas, int fromIndex, int toIndex, string sep, bool namedMode)
        {
            NamedMode = namedMode;
            if (string.IsNullOrWhiteSpace(sep))
            {
                this._datas = datas;
                this._toIndex = toIndex;
                this._curIndex = fromIndex;
            }
            else
            {
                this._datas = new List<Sheet.Cell>();
                for (int i = fromIndex; i <= toIndex; i++)
                {
                    var cell = datas[i];
                    object d = cell.Value;
                    if (d is string s)
                    {
                        this._datas.AddRange(DataUtil.SplitStringByAnySepChar(s, sep).Select(x => new Sheet.Cell(cell.Row, cell.Column, x)));
                    }
                    else
                    {
                        this._datas.Add(cell);
                    }
                }
                this._curIndex = 0;
                this._toIndex = this._datas.Count - 1;
            }
        }

        public ExcelStream(Sheet.Cell cell, string sep, bool namedMode)
        {
            NamedMode = namedMode;
            if (string.IsNullOrWhiteSpace(sep))
            {
                this._datas = new List<Sheet.Cell> { cell };
                this._toIndex = 0;
                this._curIndex = 0;
            }
            else
            {
                this._datas = new List<Sheet.Cell>();
                object d = cell.Value;
                if (!IsSkip(d))
                {
                    if (d is string s)
                    {
                        this._datas.AddRange(DataUtil.SplitStringByAnySepChar(s, sep).Where(x => !IsSkip(x)).Select(x => new Sheet.Cell(cell.Row, cell.Column, x)));
                    }
                    else
                    {
                        this._datas.Add(cell);
                    }
                }
                this._curIndex = 0;
                this._toIndex = this._datas.Count - 1;
            }
        }

        public string First => _datas[_curIndex].Value?.ToString();

        public string CurrentExcelPosition => _datas[Math.Min(_curIndex, _datas.Count - 1)].ToString();

        public int IncludeNullAndEmptySize => _toIndex - _curIndex + 1;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('[');
            for (int i = _curIndex; i <= _toIndex; i++)
            {
                sb.Append(_datas[i].Value);
                sb.Append(',');
            }
            sb.Append(']');

            return sb.ToString();
        }

        public bool TryRead(out object data)
        {
            data = null;
            while (_curIndex <= _toIndex)
            {
                data = _datas[_curIndex++].Value;
                if (!IsSkip(data))
                {
                    return true;
                }
            }
            return false;
        }

        public object Read(bool notSkip = false)
        {
            //if (curIndex <= toIndex)
            //{
            //    return datas[curIndex++].Value;
            //}
            //else
            //{
            //    throw new Exception($"cell:{datas[curIndex - 1]} 无法读取到足够多的数据");
            //}
            return notSkip ? ReadMayNull() : ReadSkipNull();
        }

        private object ReadMayNull()
        {
            return _curIndex <= _toIndex ? _datas[_curIndex++].Value : null;
        }

        //public object Read(bool nullable)
        //{
        //    return nullable ? Read() : ReadSkipNull();
        //}

        public Sheet.Cell ReadCell()
        {
            while (_curIndex <= _toIndex)
            {
                var data = _datas[_curIndex++];
                if (!IsSkip(data.Value))
                {
                    return data;
                }
            }
            throw new Exception($"cell:{_datas[_curIndex - 1]} 缺少数据");
        }

        public object ReadSkipNull()
        {
            while (_curIndex <= _toIndex)
            {
                var data = _datas[_curIndex++];
                if (!IsSkip(data.Value))
                {
                    return data.Value;
                }
            }
            throw new Exception($"cell:{_datas[_curIndex - 1]} 缺少数据");
        }


        private const string END_OF_LIST = "}";


        private bool IsSkip(object x)
        {
            return x == null || (x is string s && string.IsNullOrEmpty(s));
        }

        public bool TryReadEOF()
        {
            int oldIndex = _curIndex;
            while (_curIndex <= _toIndex)
            {
                var value = _datas[_curIndex++].Value?.ToString();
                if (!IsSkip(value))
                {
                    if (value == END_OF_LIST)
                    {
                        return true;
                    }
                    else
                    {
                        _curIndex = oldIndex;
                        return false;
                    }
                }

            }
            _curIndex = oldIndex;
            return true;
        }
    }
}
