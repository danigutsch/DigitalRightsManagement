using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Application.AgentAggregate;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.IntegrationTests.Helpers.Abstractions;
using DigitalRightsManagement.Tests.Shared.Factories;
using Shouldly;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace DigitalRightsManagement.IntegrationTests.AgentAggregate;

public sealed class AgentTests(ITestOutputHelper outputHelper, ApiFixture fixture) : ApiIntegrationTestsBase(outputHelper, fixture)
{
    [Fact]
    public async Task Get_Current_Agent_Returns_Success()
    {
        // Arrange
        var agent = AgentFactory.Seeded();

        // Act
        var response = await GetHttpClient(agent).GetAsync("/agents/me");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var agentDto = await response.Content.ReadFromJsonAsync<AgentDto>();
        agentDto.ShouldNotBeNull();
        agentDto.Id.ShouldBe(agent.Id.Value);
        agentDto.Username.ShouldBe(agent.Username);
        agentDto.Email.ShouldBe(agent.Email.Value);
        agentDto.Role.ShouldBe(agent.Role);
    }

    [Fact]
    public async Task Change_Role_Returns_Success()
    {
        // Arrange
        var admin = AgentFactory.Seeded(AgentRoles.Admin);
        var target = AgentFactory.Seeded(AgentRoles.Worker);
        const AgentRoles desiredRole = AgentRoles.Admin;

        var changeRoleDto = new ChangeRoleDto(target.Id.Value, desiredRole);

        // Act
        var response = await GetHttpClient(admin).PostAsJsonAsync("agents/change-role", changeRoleDto);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        target = await DbContext.Agents.FindAsync(target.Id);
        target.ShouldNotBeNull();
        target.Role.ShouldBe(desiredRole);
    }

    [Fact]
    public async Task Change_Email_Returns_Success()
    {
        // Arrange
        var agent = AgentFactory.Seeded();
        var newEmail = Faker.Internet.Email();

        var changeEmailDto = new ChangeEmailDto(newEmail);

        // Act
        var response = await GetHttpClient(agent).PostAsJsonAsync("/agents/change-email", changeEmailDto);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        agent = await DbContext.Agents.FindAsync(agent.Id);
        agent.ShouldNotBeNull();
        agent.Email.Value.ShouldBe(newEmail);
    }
}
