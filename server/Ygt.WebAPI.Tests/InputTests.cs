using Microsoft.AspNetCore.Http;
using Ygt.WebAPI.Utilities;

namespace Ygt.WebAPI.Tests;

/// <summary>Girdi doğrulama kuralları. Veritabanı gerektirmez.</summary>
public sealed class InputTests
{
    [Theory]
    [InlineData("0555 123 45 67")]
    [InlineData("+90 (555) 123-45-67")]
    [InlineData("905551234567")]
    [InlineData("5551234567")]
    public void Phone_numbers_are_stored_as_ten_digits(string input) =>
        Assert.Equal("5551234567", Input.RequiredPhone(input, "Telefon"));

    [Theory]
    [InlineData("123")]
    [InlineData("0555123")]
    [InlineData("abc5551234567")]
    [InlineData("0055512345678")]
    public void Invalid_phone_numbers_are_rejected(string input)
    {
        var error = Assert.Throws<BadHttpRequestException>(() => Input.RequiredPhone(input, "Telefon"));
        Assert.StartsWith("Telefon geçerli değil", error.Message);
    }

    [Fact]
    public void Tc_number_check_digits_are_validated()
    {
        Assert.Equal("10000000146", Input.OptionalTcNumber("10000000146"));
        Assert.Throws<BadHttpRequestException>(() => Input.OptionalTcNumber("12345678901"));
        Assert.Null(Input.OptionalTcNumber("  "));
    }

    [Fact]
    public void Email_is_trimmed_and_lowercased() =>
        Assert.Equal("ayse@ogr.ktun.edu.tr", Input.RequiredEmail("  Ayse@OGR.ktun.edu.tr ", "E-posta"));

    [Fact]
    public void Password_must_be_at_least_eight_characters()
    {
        Assert.Throws<BadHttpRequestException>(() => Input.Password("1234567", "Şifre"));
        Assert.Equal(" boşluk dahil ", Input.Password(" boşluk dahil ", "Şifre"));
    }
}
