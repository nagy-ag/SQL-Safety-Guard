using System;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

[Serializable]
[SqlUserDefinedAggregate(Format.Native, IsInvariantToNulls = true, IsInvariantToDuplicates = false, IsInvariantToOrder = true)]
public struct WaveSum
{
    private int total;
    public void Init() { total = 0; }
    public void Accumulate(SqlInt32 value) { if (!value.IsNull) total += value.Value; }
    public void Merge(WaveSum other) { total += other.total; }
    public SqlInt32 Terminate() { return new SqlInt32(total); }
}

[Serializable]
[SqlUserDefinedType(Format.Native, IsByteOrdered = true)]
public struct WaveNumber : INullable
{
    private int value;
    private bool isNull;
    public bool IsNull { get { return isNull; } }
    public static WaveNumber Null { get { return new WaveNumber { isNull = true }; } }
    public static WaveNumber Parse(SqlString text)
    {
        return text.IsNull ? Null : new WaveNumber { value = int.Parse(text.Value), isNull = false };
    }
    public override string ToString() { return isNull ? "NULL" : value.ToString(); }
}
