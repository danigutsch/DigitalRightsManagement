using System.Collections.Frozen;
using DigitalRightsManagement.Api;
using DigitalRightsManagement.AppHost;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.MigrationService;
using FluentAssertions;
using System.Net.Http.Json;

namespace DigitalRightsManagement.IntegrationTests;

public sealed class UserTests
{
    [Fact]
    public async Task ChangeRole_With_Valid_Data()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.DigitalRightsManagement_AppHost>();

        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();

        await app.StartAsync();

        // Act
        using var httpClient = app.CreateHttpClient(ResourceNames.Api);
        await resourceNotificationService.WaitForResourceAsync(ResourceNames.Api, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));

        var admin = SeedData.Users.First(u => u.Role == UserRoles.Admin);
        var target = SeedData.Users.First(u => u.Role == UserRoles.Viewer);
        const UserRoles desiredRole = UserRoles.Admin;

        var changeRoleDto = new ChangeUserDto(admin.Id, target.Id, desiredRole);
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        var response = await httpClient.PostAsJsonAsync("/users/change-role", changeRoleDto, cts.Token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

public static class SeedData
{
    public static FrozenSet<User> Users =>
    [
        // Admins
        User.Create(Guid.Parse("68f44f2c07254a98a2afa133efd8ed21"), "john.doe", "john.doe@localhost.com", UserRoles.Admin),
        User.Create(Guid.Parse("a712b5a56d67455286814f6fd17380fe"), "jane.smith", "jane.smith@localhost.com", UserRoles.Admin),
        User.Create(Guid.Parse("87aef33a679942ef9b4d5fe768d0be76"), "michael.brown", "michael.brown@localhost.com", UserRoles.Admin),
        User.Create(Guid.Parse("b1aef33a679942ef9b4d5fe768d0be76"), "emily.jones", "emily.jones@localhost.com", UserRoles.Admin),
        User.Create(Guid.Parse("c2aef33a679942ef9b4d5fe768d0be76"), "david.wilson", "david.wilson@localhost.com", UserRoles.Admin),

        // Managers
        User.Create(Guid.Parse("d3aef33a679942ef9b4d5fe768d0be76"), "chris.taylor", "chris.taylor@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("e4aef33a679942ef9b4d5fe768d0be76"), "sarah.moore", "sarah.moore@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("f5aef33a679942ef9b4d5fe768d0be76"), "james.anderson", "james.anderson@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("06aef33a679942ef9b4d5fe768d0be76"), "linda.thomas", "linda.thomas@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("17aef33a679942ef9b4d5fe768d0be76"), "robert.jackson", "robert.jackson@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("28aef33a679942ef9b4d5fe768d0be76"), "patricia.white", "patricia.white@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("39aef33a679942ef9b4d5fe768d0be76"), "mark.harris", "mark.harris@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("4aaef33a679942ef9b4d5fe768d0be76"), "barbara.martin", "barbara.martin@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("5baef33a679942ef9b4d5fe768d0be76"), "paul.thompson", "paul.thompson@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("6caef33a679942ef9b4d5fe768d0be76"), "nancy.garcia", "nancy.garcia@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("7daef33a679942ef9b4d5fe768d0be76"), "kevin.martinez", "kevin.martinez@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("8eaef33a679942ef9b4d5fe768d0be76"), "karen.robinson", "karen.robinson@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("9faef33a679942ef9b4d5fe768d0be76"), "steven.clark", "steven.clark@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("0ga ef33a679942ef9b4d5fe768d0be76"), "betty.rodriguez", "betty.rodriguez@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("1haef33a679942ef9b4d5fe768d0be76"), "brian.lewis", "brian.lewis@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("2iaef33a679942ef9b4d5fe768d0be76"), "lisa.lee", "lisa.lee@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("3jaef33a679942ef9b4d5fe768d0be76"), "ronald.walker", "ronald.walker@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("4kaef33a679942ef9b4d5fe768d0be76"), "mary.hall", "mary.hall@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("5laef33a679942ef9b4d5fe768d0be76"), "jason.allen", "jason.allen@localhost.com", UserRoles.Manager),
        User.Create(Guid.Parse("6maef33a679942ef9b4d5fe768d0be76"), "susan.young", "susan.young@localhost.com", UserRoles.Manager),

        // Viewers
        User.Create(Guid.Parse("7naef33a679942ef9b4d5fe768d0be76"), "daniel.king", "daniel.king@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("8oaef33a679942ef9b4d5fe768d0be76"), "jessica.wright", "jessica.wright@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("9paef33a679942ef9b4d5fe768d0be76"), "matthew.lopez", "matthew.lopez@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("0qaef33a679942ef9b4d5fe768d0be76"), "ashley.hill", "ashley.hill@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("1raef33a679942ef9b4d5fe768d0be76"), "joshua.scott", "joshua.scott@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("2saef33a679942ef9b4d5fe768d0be76"), "amanda.green", "amanda.green@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("3taef33a679942ef9b4d5fe768d0be76"), "andrew.adams", "andrew.adams@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("4uaef33a679942ef9b4d5fe768d0be76"), "megan.baker", "megan.baker@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("5vaef33a679942ef9b4d5fe768d0be76"), "justin.gonzalez", "justin.gonzalez@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("6waef33a679942ef9b4d5fe768d0be76"), "lauren.nelson", "lauren.nelson@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("7xaef33a679942ef9b4d5fe768d0be76"), "ethan.carter", "ethan.carter@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("8yaef33a679942ef9b4d5fe768d0be76"), "nicole.mitchell", "nicole.mitchell@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("9zaef33a679942ef9b4d5fe768d0be76"), "ryan.perez", "ryan.perez@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("0aaef33a679942ef9b4d5fe768d0be76"), "rachel.roberts", "rachel.roberts@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("1baef33a679942ef9b4d5fe768d0be76"), "tyler.turner", "tyler.turner@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("2caef33a679942ef9b4d5fe768d0be76"), "olivia.phillips", "olivia.phillips@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("3daef33a679942ef9b4d5fe768d0be76"), "brandon.campbell", "brandon.campbell@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("4eaef33a679942ef9b4d5fe768d0be76"), "emma.parker", "emma.parker@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("5faef33a679942ef9b4d5fe768d0be76"), "alexander.evans", "alexander.evans@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("6gaef33a679942ef9b4d5fe768d0be76"), "samantha.edwards", "samantha.edwards@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("7haef33a679942ef9b4d5fe768d0be76"), "jacob.collins", "jacob.collins@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("8iaef33a679942ef9b4d5fe768d0be76"), "sophia.stewart", "sophia.stewart@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("9jaef33a679942ef9b4d5fe768d0be76"), "logan.sanchez", "logan.sanchez@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("0kaef33a679942ef9b4d5fe768d0be76"), "hannah.morris", "hannah.morris@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("1laef33a679942ef9b4d5fe768d0be76"), "jackson.rogers", "jackson.rogers@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("2maef33a679942ef9b4d5fe768d0be76"), "abigail.reed", "abigail.reed@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("3naef33a679942ef9b4d5fe768d0be76"), "aiden.cook", "aiden.cook@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("4oaef33a679942ef9b4d5fe768d0be76"), "madison.morgan", "madison.morgan@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("5paef33a679942ef9b4d5fe768d0be76"), "lucas.bell", "lucas.bell@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("6qaef33a679942ef9b4d5fe768d0be76"), "ella.murphy", "ella.murphy@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("7raef33a679942ef9b4d5fe768d0be76"), "dylan.bailey", "dylan.bailey@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("8saef33a679942ef9b4d5fe768d0be76"), "zoe.rivera", "zoe.rivera@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("9taef33a679942ef9b4d5fe768d0be76"), "nathan.cooper", "nathan.cooper@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("0uaef33a679942ef9b4d5fe768d0be76"), "chloe.richardson", "chloe.richardson@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("1vaef33a679942ef9b4d5fe768d0be76"), "isaac.cox", "isaac.cox@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("2waef33a679942ef9b4d5fe768d0be76"), "grace.howard", "grace.howard@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("3xaef33a679942ef9b4d5fe768d0be76"), "gabriel.ward", "gabriel.ward@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("4yaef33a679942ef9b4d5fe768d0be76"), "lily.torres", "lily.torres@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("5zaef33a679942ef9b4d5fe768d0be76"), "henry.peterson", "henry.peterson@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("6aaef33a679942ef9b4d5fe768d0be76"), "ella.gray", "ella.gray@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("7baef33a679942ef9b4d5fe768d0be76"), "jackson.ramirez", "jackson.ramirez@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("8caef33a679942ef9b4d5fe768d0be76"), "scarlett.james", "scarlett.james@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("9daef33a679942ef9b4d5fe768d0be76"), "liam.watson", "liam.watson@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("0eaef33a679942ef9b4d5fe768d0be76"), "sophia.brooks", "sophia.brooks@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("1faef33a679942ef9b4d5fe768d0be76"), "logan.kelly", "logan.kelly@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("2gaef33a679942ef9b4d5fe768d0be76"), "mia.sanders", "mia.sanders@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("3haef33a679942ef9b4d5fe768d0be76"), "jackson.price", "jackson.price@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("4iaef33a679942ef9b4d5fe768d0be76"), "ava.bennett", "ava.bennett@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("5jaef33a679942ef9b4d5fe768d0be76"), "noah.wood", "noah.wood@localhost.com", UserRoles.Viewer),
        User.Create(Guid.Parse("6kaef33a679942ef9b4d5fe768d0be76"), "isabella.barnes", "isabella.barnes@localhost.com", UserRoles.Viewer)
    ];
}
