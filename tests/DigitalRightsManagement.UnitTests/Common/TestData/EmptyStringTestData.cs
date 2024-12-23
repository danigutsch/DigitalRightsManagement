namespace DigitalRightsManagement.UnitTests.Common.TestData;

internal sealed class EmptyStringTestData : TheoryData<string>
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
