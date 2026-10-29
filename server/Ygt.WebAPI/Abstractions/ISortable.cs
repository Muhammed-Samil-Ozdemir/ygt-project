namespace Ygt.WebAPI.Abstractions;

/// <summary>Panelden sırası değiştirilebilen liste içeriği (ekip, hakkımızda kartları...). Küçük sıra önce gelir.</summary>
public interface ISortable
{
    Guid Id { get; }
    int SortOrder { get; set; }
}
