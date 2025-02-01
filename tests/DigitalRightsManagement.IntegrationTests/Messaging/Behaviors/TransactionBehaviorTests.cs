using Ardalis.Result;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Infrastructure.Messaging.Behaviors;
using DigitalRightsManagement.Infrastructure.Persistence;
using DigitalRightsManagement.IntegrationTests.Helpers.Abstractions;
using DigitalRightsManagement.IntegrationTests.Helpers.TestDoubles;
using DigitalRightsManagement.Tests.Shared.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit.Abstractions;

namespace DigitalRightsManagement.IntegrationTests.Messaging.Behaviors;

public sealed class TransactionBehaviorTests : ApiIntegrationTestsBase
{
    private readonly IServiceScope _scope;
    private readonly ManagementDbContext _transactionDbContext;
    private readonly TransactionBehavior<TestRequest, Result> _behavior;

    public TransactionBehaviorTests(ITestOutputHelper outputHelper, ApiFixture fixture) : base(outputHelper, fixture)
    {
        _scope = fixture.Services.CreateScope();
        _transactionDbContext = _scope.ServiceProvider.GetRequiredService<ManagementDbContext>();
        var logger = _scope.ServiceProvider.GetRequiredService<ILogger<TransactionBehavior<TestRequest, Result>>>();
        _behavior = new TransactionBehavior<TestRequest, Result>(_transactionDbContext, logger);
    }

    [Fact]
    public async Task Transaction_Commits_On_Success()
    {
        // Arrange
        var agent = AgentFactory.Seeded(AgentRoles.Manager);
        var product = ProductFactory.InDevelopment(manager: agent.Id);

        var request = new TestRequest();
        var productId = product.Id;

        // Act
        await _behavior.Handle(request,
            async () =>
            {
                _transactionDbContext.Products.Add(product);
                await _transactionDbContext.SaveChangesAsync();
                return Result.Success();
            },
            CancellationToken.None);

        // Assert
        var addedProduct = await DbContext.Products.FindAsync(productId);
        addedProduct.ShouldNotBeNull();
        addedProduct.Id.ShouldBe(productId);
    }

    [Fact]
    public async Task Transaction_Rolls_Back_On_Error()
    {
        // Arrange
        var agent = AgentFactory.Seeded(AgentRoles.Manager);
        var product = ProductFactory.InDevelopment(manager: agent.Id);

        var request = new TestRequest();
        var productId = product.Id;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _behavior.Handle(request,
                async () =>
                {
                    _transactionDbContext.Products.Add(product);
                    await _transactionDbContext.SaveChangesAsync();
                    throw new InvalidOperationException("Test exception");
                },
                CancellationToken.None)
                .ConfigureAwait(false);
        });

        var addedProduct = await DbContext.Products.FindAsync(productId);
        addedProduct.ShouldBeNull();
    }

    [Fact]
    public async Task Nested_Transactions_Share_Same_Transaction()
    {
        // Arrange
        var agent = AgentFactory.Seeded(AgentRoles.Manager);
        var product1 = ProductFactory.InDevelopment(manager: agent.Id);
        var product2 = ProductFactory.InDevelopment(manager: agent.Id);

        var request = new TestRequest();
        var product1Id = product1.Id;
        var product2Id = product2.Id;

        // Act
        await _behavior.Handle(request,
            async () =>
            {
                _transactionDbContext.Products.Add(product1);
                await _transactionDbContext.SaveChangesAsync();

                var currentTransaction = _transactionDbContext.Database.CurrentTransaction;
                await _behavior.Handle(request,
                    async () =>
                    {
                        var nestedTransaction = _transactionDbContext.Database.CurrentTransaction;
                        nestedTransaction.ShouldBeSameAs(currentTransaction);

                        _transactionDbContext.Products.Add(product2);
                        await _transactionDbContext.SaveChangesAsync();
                        return Result.Success();
                    },
                    CancellationToken.None);

                return Result.Success();
            },
            CancellationToken.None);

        // Assert
        var products = await DbContext.Products.Where(p => p.Id == product1Id || p.Id == product2Id).ToListAsync();
        products.Count.ShouldBe(2);
        products.ShouldContain(p => p.Id == product1Id);
        products.ShouldContain(p => p.Id == product2Id);
    }

    protected override void Dispose(bool disposing)
    {
        _scope.Dispose();
        _transactionDbContext.Dispose();
        base.Dispose(disposing);
    }
}