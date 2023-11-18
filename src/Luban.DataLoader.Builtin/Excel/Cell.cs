namespace Luban.DataLoader.Builtin.Excel;

public struct Cell
{
    public Cell(int row, int column, object value)
    {
        this.Row = row;
        this.Column = column;
        this.Value = value;
    }
    public int Row { get; } // 从 1 开始

    public int Column { get; } // 从 0 开始，考虑改了它？

    public object Value { get; }


    private static string ToAlphaString(int column)
    {
        int h = column / 26;
        int n = column % 26;
        return $"{(h > 0 ? ((char)('A' + h - 1)).ToString() : "")}{(char)('A' + n)}";
    }

    public override string ToString()
    {
        return $"[{ToAlphaString(Column)}:{Row + 1}] {Value}";
    }
}
