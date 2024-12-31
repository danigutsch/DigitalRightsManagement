using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DigitalRightsManagement.Infrastructure.Persistence.Converters;

internal sealed class ListOfGuidConverter(ConverterMappingHints? mappingHints = null)
    : ValueConverter<List<Guid>, string>(
        guids => ToDbValue(guids),
        value => FromDbValue(value),
        mappingHints)
{
    private static string ToDbValue(List<Guid> value) => string.Join(',', value);

    private static List<Guid> FromDbValue(string value) =>
        [..value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(Guid.Parse)];
}
