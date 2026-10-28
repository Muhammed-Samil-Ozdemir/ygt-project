namespace Ygt.WebAPI.Enums;

/// <summary>Etkinlik formunun türü. Başvuruların hangi tabloya yazılacağını belirler.</summary>
public enum FormType
{
    /// <summary>Bireysel başvuru (ad, soyad, telefon, e-posta).</summary>
    Standard,

    /// <summary>2-3 kişilik takım başvurusu.</summary>
    Hackathon
}
