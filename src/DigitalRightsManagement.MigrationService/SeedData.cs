using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using System.Collections.Frozen;

namespace DigitalRightsManagement.MigrationService;

public static class SeedData
{
    static SeedData()
    {
        var agents = AgentsAndPasswords.Select(t =>
        {
            var (agent, _) = t;

            var productsToAdd = Products
                .Where(product => agent.Role switch
                {
                    AgentRoles.Manager => product.AgentId == agent.Id,
                    AgentRoles.Worker => product.AssignedWorkers.Contains(agent.Id),
                    _ => false
                })
                .Select(product => product.Id);

            agent.AddProducts(productsToAdd);

            return agent;
        });

        Agents = [.. agents];

        Passwords = AgentsAndPasswords.ToFrozenDictionary(x => x.Agent.Id, x => x.Password);
    }

    public static FrozenDictionary<AgentId, string> Passwords { get; }

    public static IReadOnlyList<Agent> Agents { get; }

    public static IReadOnlyList<(Agent Agent, string Password)> AgentsAndPasswords { get; } =
    [
        // Admins
        (Agent.Create("admin1", EmailAddress.From("admin1@example.com"), AgentRoles.Admin, AgentId.From(Guid.Parse("34f87c92-4f2e-4350-8f70-f2c4c2a449a2"))).Value, "i0%5Nr8vB^2LL9T$XYfU23%EVnvY0G"),
        (Agent.Create("admin2", EmailAddress.From("admin2@example.com"), AgentRoles.Admin, AgentId.From(Guid.Parse("de3c7c39-6cf5-4eb0-8f97-3c3f2ed93575"))).Value, "XH2%%4^T27jd%%wxh5^606irij6*5T"),

        // Managers
        (Agent.Create("manager1", EmailAddress.From("manager1@example.com"), AgentRoles.Manager, AgentId.From(Guid.Parse("bb6346b1-4924-4e69-9099-8e81f84c5123"))).Value, "qCxcPqi55Ly9X$63aU#M$9HBq5jnSV"),
        (Agent.Create("manager2", EmailAddress.From("manager2@example.com"), AgentRoles.Manager, AgentId.From(Guid.Parse("08c7acee-0f8c-4e70-9f72-c59db53ae0be"))).Value, "E1qk5OhxS6#!3A1*gco*##8iF!#s&B"),
        (Agent.Create("manager3", EmailAddress.From("manager3@example.com"), AgentRoles.Manager, AgentId.From(Guid.Parse("3d792f3d-8764-49c1-ae2b-40444ffbb2f9"))).Value, "!%h2kH$%9Y*x@%D1@i0Q!@xLDg*%8E"),
        (Agent.Create("manager4", EmailAddress.From("manager4@example.com"), AgentRoles.Manager, AgentId.From(Guid.Parse("58bf001c-bb9a-41fd-8917-c6eaf110af90"))).Value, "OP41%z4&2x^B!Bkwr%*0p^CPw9m4^Z"),
        (Agent.Create("manager5", EmailAddress.From("manager5@example.com"), AgentRoles.Manager, AgentId.From(Guid.Parse("cd9f1577-06b4-4f13-8901-3c97f04ada30"))).Value, "@sK#8!n6YLuM081444&#j%4$3^oq*A"),

        // Workers
        (Agent.Create("worker1", EmailAddress.From("worker1@example.com"), AgentRoles.Worker, AgentId.From(Guid.Parse("a01d3277-a55b-411d-b526-662f4df0bbe7"))).Value, "v@2q$65NbuFJRE$N3$0u6s&p2K9*aJ"),
        (Agent.Create("worker2", EmailAddress.From("worker2@example.com"), AgentRoles.Worker, AgentId.From(Guid.Parse("ff043c9e-a1a2-4ed5-97c9-0ed1de3304f8"))).Value, "zLZqvS9!7O7Q@QV6$6b@*1k&$73ogZ"),
        (Agent.Create("worker3", EmailAddress.From("worker3@example.com"), AgentRoles.Worker, AgentId.From(Guid.Parse("cd86fe59-81d6-4e9a-8d02-dd71613f202b"))).Value, "19&k6d1$yu*j25rfA4On&z!5!E&VpR"),
        (Agent.Create("worker4", EmailAddress.From("worker4@example.com"), AgentRoles.Worker, AgentId.From(Guid.Parse("2ea745cc-7476-4e39-a2c1-048c9fd304f4"))).Value, "#&&*^d^9a#mH4Qt@3$*H^p*3GmKdiU"),
        (Agent.Create("worker5", EmailAddress.From("worker5@example.com"), AgentRoles.Worker, AgentId.From(Guid.Parse("14615405-f391-4f38-9de4-0c4aaf6a4341"))).Value, "Ugn4@r*vwc&6%7*e0&A@@Xyw^!y3dS"),
        (Agent.Create("worker6", EmailAddress.From("worker6@example.com"), AgentRoles.Worker, AgentId.From(Guid.Parse("7c588df5-4b1a-4370-9f4c-33d0c351406d"))).Value, "saoYy5Q194**0O#1!E8f6iK2o9o*!V"),
        (Agent.Create("worker7", EmailAddress.From("worker7@example.com"), AgentRoles.Worker, AgentId.From(Guid.Parse("6a0e67e9-153e-4eea-a751-77f7043e32ef"))).Value, "&K!c$&j@*VLR38rj4%^%iY@Aaz@@nL"),
        (Agent.Create("worker8", EmailAddress.From("worker8@example.com"), AgentRoles.Worker, AgentId.From(Guid.Parse("84282b24-8c11-44bb-9ccb-5c4c06ded57b"))).Value, "!d93S!m3N1i5Shk$pgCL74*BADBU*J"),
        (Agent.Create("worker9", EmailAddress.From("worker9@example.com"), AgentRoles.Worker, AgentId.From(Guid.Parse("d7efb26d-393c-4d5e-bfb6-12b3152f2994"))).Value, "*@!qa2p5S$xd@A%7Y0Tj3sqN5N34wW"),
        (Agent.Create("worker10", EmailAddress.From("worker10@example.com"), AgentRoles.Worker, AgentId.From(Guid.Parse("6cf44570-c980-4db9-b31b-c3cc77bba53a"))).Value, "77#9970#HXF59Ax60&13$Nu*^sv!bB")
    ];

    public static IReadOnlyList<Product> Products { get; } =
    [
        CreateProductWithWorkers(
            "Product1",
            "Description1",
            10,
            Currency.Dollar,
            "08c7acee-0f8c-4e70-9f72-c59db53ae0be",
            "2a1b2bc7-339a-47aa-9ba3-a2cf2e6aaf2c",
            [
                "cd86fe59-81d6-4e9a-8d02-dd71613f202b",
                "2ea745cc-7476-4e39-a2c1-048c9fd304f4"
            ]),
        CreateProductWithWorkers(
            "Product2",
            "Description2",
            20,
            Currency.Dollar,
            "3d792f3d-8764-49c1-ae2b-40444ffbb2f9",
            "474c01ab-eeff-48ff-8199-9eb4b6595033",
            [
                "14615405-f391-4f38-9de4-0c4aaf6a4341",
                "7c588df5-4b1a-4370-9f4c-33d0c351406d",
                "6a0e67e9-153e-4eea-a751-77f7043e32ef"
            ]),
        CreateProductWithWorkers(
            "Product3",
            "Description3",
            30,
            Currency.Dollar,
            "58bf001c-bb9a-41fd-8917-c6eaf110af90",
            "6517c639-7083-46e8-a0a7-65308f0c0f6a",
            [
                "84282b24-8c11-44bb-9ccb-5c4c06ded57b",
                "d7efb26d-393c-4d5e-bfb6-12b3152f2994"
            ]),
        CreateProductWithWorkers(
            "Product4",
            "Description4",
            40,
            Currency.Dollar,
            "cd9f1577-06b4-4f13-8901-3c97f04ada30",
            "7b6b5c25-99f2-497e-bde1-c06d8ebfd542",
            [
                "a01d3277-a55b-411d-b526-662f4df0bbe7",
                "ff043c9e-a1a2-4ed5-97c9-0ed1de3304f8",
                "6cf44570-c980-4db9-b31b-c3cc77bba53a"
            ]),
        CreateProductWithWorkers(
            "Product5",
            "Description5",
            50,
            Currency.Dollar,
            "cd9f1577-06b4-4f13-8901-3c97f04ada30",
            "2683e1c1-c872-497a-adfd-29d6d07c3efa",
            [
                "cd86fe59-81d6-4e9a-8d02-dd71613f202b",
                "2ea745cc-7476-4e39-a2c1-048c9fd304f4",
                "14615405-f391-4f38-9de4-0c4aaf6a4341"
            ])
    ];
    private static Product CreateProductWithWorkers(
        string name,
        string description,
        decimal price,
        Currency currency,
        string createdBy,
        string id,
        string[] workerIds)
    {
        var product = Product.Create(
            ProductName.From(name).Value,
            Description.From(description).Value,
            Price.From(price, currency).Value,
            AgentId.From(createdBy).Value,
            ProductId.From(id).Value
            ).Value;

        foreach (var workerId in workerIds.Select(i => AgentId.From(i).Value))
        {
            product.AssignWorker(product.AgentId, workerId);
        }

        return product;
    }
}
