using System.Text;
using Luban.DataLoader.Builtin.Utils;

namespace Luban.DataLoader.Builtin.Excel;

class ExcelStream
{

    private readonly List<Cell> _datas;
    private readonly int _toIndex;
    private int _curIndex;

    private readonly string _overrideDefault;

    public ExcelStream(List<Cell> datas, int fromIndex, int toIndex, string sep, string overrideDefault)
    {
        _overrideDefault = overrideDefault;
        if (string.IsNullOrWhiteSpace(sep))
        {
            if (string.IsNullOrEmpty(overrideDefault))
            {
                this._datas = datas;
                this._toIndex = toIndex;
                this._curIndex = fromIndex;
            }
            else
            {
                this._datas = new List<Cell>();
                for (int i = fromIndex; i <= toIndex; i++)
                {
                    var cell = datas[i];
                    object d = cell.Value;
                    if (!IsSkip(d))
                    {
                        this._datas.Add(cell);
                    }
                    else
                    {
                        this._datas.Add(new Cell(cell.Row, cell.Column, _overrideDefault));
                    }
                }
                this._curIndex = 0;
                this._toIndex = this._datas.Count - 1;
            }

        }
        else
        {
            this._datas = new List<Cell>();
            for (int i = fromIndex; i <= toIndex; i++)
            {
                var cell = datas[i];
                object d = cell.Value;
                if (!IsSkip(d))
                {
                    if (d is string s)
                    {
                        this._datas.AddRange(LoadDataUtil.SplitStringByAnySepChar(s, sep).Select(x => new Cell(cell.Row, cell.Column, x)));
                    }
                    else
                    {
                        this._datas.Add(cell);
                    }
                }
                else if (!string.IsNullOrEmpty(_overrideDefault))
                {
                    this._datas.Add(new Cell(cell.Row, cell.Column, _overrideDefault));
                }
            }
            this._curIndex = 0;
            this._toIndex = this._datas.Count - 1;
        }
    }

    public ExcelStream(Cell cell, string sep)
    {
        if (string.IsNullOrWhiteSpace(sep))
        {
            this._datas = new List<Cell> { cell };
            this._toIndex = 0;
            this._curIndex = 0;
        }
        else
        {
            this._datas = new List<Cell>();
            object d = cell.Value;
            if (!IsSkip(d))
            {
                if (d is string s)
                {
                    this._datas.AddRange(LoadDataUtil.SplitStringByAnySepChar(s, sep).Where(x => !IsSkip(x)).Select(x => new Cell(cell.Row, cell.Column, x)));
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

    public ExcelStream(List<List<Cell>> rows, int fromIndex, int toIndex, string sep, string overrideDefault)
    {
        _overrideDefault = overrideDefault;
        this._datas = new List<Cell>();
        if (string.IsNullOrWhiteSpace(sep))
        {
            if (string.IsNullOrEmpty(overrideDefault))
            {
                foreach (var row in rows)
                {
                    for (int i = fromIndex; i <= toIndex; i++)
                    {
                        this._datas.Add(row[i]);
                    }
                }
            }
            else
            {
                throw new NotSupportedException("concated multi rows don't support 'default' ");
            }

        }
        else
        {
            foreach (var row in rows)
            {
                for (int i = fromIndex; i <= toIndex; i++)
                {
                    var cell = row[i];
                    object d = cell.Value;
                    if (!IsSkip(d))
                    {
                        if (d is string s)
                        {
                            this._datas.AddRange(LoadDataUtil.SplitStringByAnySepChar(s, sep).Select(x => new Cell(cell.Row, cell.Column, x)));
                        }
                        else
                        {
                            this._datas.Add(cell);
                        }
                    }
                    else if (!string.IsNullOrEmpty(_overrideDefault))
                    {
                        this._datas.Add(new Cell(cell.Row, cell.Column, _overrideDefault));
                    }
                }
            }
        }
        this._curIndex = 0;
        this._toIndex = this._datas.Count - 1;
    }

    public string First => _datas[_curIndex].Value?.ToString();

    public string LastReadDataInfo => _datas[Math.Min(LastReadIndex, _datas.Count - 1)].ToString();

    private int LastReadIndex { get; set; }

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
                LastReadIndex = _curIndex - 1;
                return true;
            }
        }
        LastReadIndex = _curIndex - 1;
        return false;
    }

    public bool TryPeed(out object data)
    {
        int oldCurIndex = _curIndex;
        if (TryRead(out data))
        {
            _curIndex = oldCurIndex;
            return true;
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
        return _curIndex <= _toIndex ? _datas[LastReadIndex = _curIndex++].Value : null;
    }

    //public object Read(bool nullable)
    //{
    //    return nullable ? Read() : ReadSkipNull();
    //}

    public Cell ReadCell()
    {
        while (_curIndex <= _toIndex)
        {
            var data = _datas[_curIndex++];
            if (!IsSkip(data.Value))
            {
                LastReadIndex = _curIndex - 1;
                return data;
            }
        }
        LastReadIndex = _curIndex - 1;
        throw new Exception($"cell:{_datas[_curIndex - 1]} 缺少数据");
    }

    public object ReadSkipNull()
    {
        while (_curIndex <= _toIndex)
        {
            var data = _datas[_curIndex++];
            if (!IsSkip(data.Value))
            {
                LastReadIndex = _curIndex - 1;
                return data.Value;
            }
        }
        LastReadIndex = _curIndex - 1;
        throw new Exception($"cell:{_datas[_curIndex - 1]} 缺少数据");
    }


    public const string END_OF_LIST = "}";


    private bool IsSkip(object x)
    {
        return x == null || (x is string s && string.IsNullOrEmpty(s));
    }

    public bool TryReadEOF()
    {
        int oldIndex = _curIndex;
        while (_curIndex <= _toIndex)
        {
            var value = _datas[_curIndex++].Value;
            if (!IsSkip(value))
            {
                if (value is string s && s == END_OF_LIST)
                {
                    LastReadIndex = _curIndex - 1;
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

    internal ExcelStream CreateAutoSepStream(string simpleContainerSep)
    {
        int startIndex = _curIndex;
        while (_curIndex <= _toIndex)
        {
            var value = _datas[_curIndex++].Value;
            if (!IsSkip(value))
            {
                if (value is string s && s == END_OF_LIST)
                {
                    break;
                }
            }
        }
        LastReadIndex = _curIndex - 1;
        return new ExcelStream(_datas, startIndex, LastReadIndex, simpleContainerSep, "");
    }
}
