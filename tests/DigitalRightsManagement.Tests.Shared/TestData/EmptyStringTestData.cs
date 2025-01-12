using Xunit;

namespace DigitalRightsManagement.Tests.Shared.TestData;

public sealed class EmptyStringTestData : TheoryData<string>
{
    public EmptyStringTestData()
    {
        Add(string.Empty);
        Add(" ");
        Add("  ");
        Add("\t");
        Add("\n");
        Add("\r");
        Add("\r\n");
    }
}
