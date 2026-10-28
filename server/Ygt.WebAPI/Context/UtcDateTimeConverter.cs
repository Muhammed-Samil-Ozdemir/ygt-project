using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ygt.WebAPI.Context;

/// <summary>
/// Veritabanından okunan tarihleri UTC olarak işaretler, yazarken de yerel saatleri UTC'ye çevirir.
/// </summary>
public sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    value => value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : value,
    value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
