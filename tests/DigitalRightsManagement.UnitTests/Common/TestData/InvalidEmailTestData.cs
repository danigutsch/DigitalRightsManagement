namespace DigitalRightsManagement.UnitTests.Common.TestData;

internal sealed class InvalidEmailTestData : TheoryData<string>
{
    public InvalidEmailTestData()
    {
        AddRange(
            "invalidemail",
            "invalidemail@",
            "invalid@.email",
            "invalid@email.",
            "invalid@ email.com",
            "invalidemail.com",
            "invalid@@example.com",
            "@example.com",
            "inv alid@example.com");
    }
}
