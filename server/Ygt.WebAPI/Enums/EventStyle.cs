namespace Ygt.WebAPI.Enums;

/// <summary>Etkinliğin ana sayfada hangi bölümde ve nasıl gösterileceği.</summary>
public enum EventStyle
{
    /// <summary>"Etkinliklerimiz" panosunda polaroid: kapak fotoğrafı ve başlık.</summary>
    Wall,

    /// <summary>Öne çıkan etkinlik: kendi bloğunda büyük başlık, açıklama ve ilk üç fotoğraf.</summary>
    Featured,

    /// <summary>"Gelenekselleşmiş etkinliklerimiz" bölümünde: kapak fotoğrafı, başlık ve el yazısıyla açıklama.</summary>
    Recurring
}
