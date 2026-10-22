using System.Security.Cryptography;

namespace Global360Playwright.TestData;

internal sealed record AssetCreationTestData(
    string AssetTag,
    string ModelSelection,
    string ExpectedModelName,
    string Status,
    string SerialNumber,
    string Notes)
{
    public string SuccessMessage => $"Asset with tag {AssetTag} was created successfully.";

    public static AssetCreationTestData Create()
    {
        var uniqueId = GenerateRandomString(32);

        return new AssetCreationTestData(
            AssetTag: $"PW-{uniqueId}",
            ModelSelection: "Laptops - Macbook Pro 13",
            ExpectedModelName: "Macbook Pro 13\"",
            Status: "Ready to Deploy",
            SerialNumber: $"SN-{uniqueId}",
            Notes: "Created Playwright automation test by Sonal");
    }

    public static string GenerateRandomString(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than zero.");
        }

        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return string.Create(length, characters, static (buffer, alphabet) =>
        {
            for (var index = 0; index < buffer.Length; index++)
            {
                buffer[index] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
            }
        });
    }
}