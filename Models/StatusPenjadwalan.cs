using System.Runtime.Serialization;

public enum StatusPenjadwalan
{
    [EnumMember(Value = "diproses")]
    Diproses,

    [EnumMember(Value = "disetujui")]
    Disetujui,

    [EnumMember(Value = "ditolak")]
    Ditolak
}