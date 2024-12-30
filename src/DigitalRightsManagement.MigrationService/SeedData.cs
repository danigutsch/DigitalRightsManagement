using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.MigrationService.Factories;
using System.Collections.Immutable;

namespace DigitalRightsManagement.MigrationService;

public static class SeedData
{
    public static IReadOnlyList<(User User, Product[] Products)> Get()
    {
        var admins = AdminIds.Select(id => UserFactory.Create(role: UserRoles.Admin, id: id));

        var managers = ManagerAndProductIds.Keys.Select(id => UserFactory.Create(role: UserRoles.Manager, id: id));

        var viewers = ViewerIds.Select(id => UserFactory.Create(role: UserRoles.Viewer, id: id));

        var users = admins
            .Concat(managers)
            .Concat(viewers);

        var usersAndProducts = users.Select(user =>
        {
            if (!ManagerAndProductIds.TryGetValue(user.Id, out var productIds))
            {
                productIds = [];
            }

            Product[] products = [..productIds.Select(id => ProductFactory.InDevelopment(manager: user.Id, id: id))];

            user.AddProducts(products);

            return (user, products);
        });

        return [..usersAndProducts];
    }

    public static IReadOnlyList<Guid> AdminIds =>
    [
        Guid.Parse("34f87c92-4f2e-4350-8f70-f2c4c2a449a2"),
        Guid.Parse("de3c7c39-6cf5-4eb0-8f97-3c3f2ed93575")
    ];

    public static IImmutableDictionary<Guid, Guid[]> ManagerAndProductIds =>
        new Dictionary<Guid, Guid[]>
        {
            { Guid.Parse("bb6346b1-4924-4e69-9099-8e81f84c5123"), [] },
            { Guid.Parse("08c7acee-0f8c-4e70-9f72-c59db53ae0be"), [Guid.Parse("f3cf02bd-0818-4f25-95a1-b783076d6bba")] },
            { Guid.Parse("3d792f3d-8764-49c1-ae2b-40444ffbb2f9"), [Guid.Parse("dac03f24-88a2-4029-a001-bcf12eef003b")] },
            { Guid.Parse("b404a193-c0b8-4807-b4ef-aa442ce5acbe"), [Guid.Parse("fedc5f34-e2c4-4ced-b2f6-f1467220a071")] },
            { Guid.Parse("245ad689-41e1-40c4-9114-2dc89d88e53b"), [Guid.Parse("7af398bd-715b-4385-80e5-a4bcc4c25d62"), Guid.Parse("d002ec9a-ff0a-4900-8a66-dbe5560120cf")] },
            { Guid.Parse("2965854b-70fa-41ed-a7ce-d1a83973124f"), [Guid.Parse("5f4a6bc2-bad6-423f-90e4-54de4080c1f1"), Guid.Parse("2ff4da2d-5c68-457d-b6a8-261c5d91cf5b")] },
            { Guid.Parse("8f2703f7-96a6-4a75-b663-ec027e4b513e"), [Guid.Parse("bae91018-c7ee-4fe8-a912-11a2a0221699"), Guid.Parse("f22d8006-ea3d-4606-b2ea-3c9591d5f89e")] },
            { Guid.Parse("58bf001c-bb9a-41fd-8917-c6eaf110af90"), [Guid.Parse("51a2a074-87f4-424f-9d0a-aac9afc2e481"), Guid.Parse("b0e73268-c8a2-4f3d-b5e7-c0b7d0e19af0")] },
            { Guid.Parse("cd9f1577-06b4-4f13-8901-3c97f04ada30"), [Guid.Parse("83e41022-44e1-40f4-8f41-d9b610da9f60"), Guid.Parse("84bef7ed-d646-4bb9-98b0-beeec2f26c18"), Guid.Parse("43273dea-a9e3-4db1-8704-d267c94f31d4")] },
            { Guid.Parse("05aebb26-5611-4eda-9cc0-73bd50c6fddf"), [Guid.Parse("ade8b813-cf00-43a0-8497-b854869bd957"), Guid.Parse("091669dd-aba7-4bda-b95a-719d727a659a"), Guid.Parse("4673e0fe-c9ac-4772-a7ae-b4b3bb5ff296")] }
        }.ToImmutableDictionary();

    public static IReadOnlyList<Guid> ViewerIds =>
    [
        Guid.Parse("a01d3277-a55b-411d-b526-662f4df0bbe7"),
        Guid.Parse("ff043c9e-a1a2-4ed5-97c9-0ed1de3304f8"),
        Guid.Parse("cd86fe59-81d6-4e9a-8d02-dd71613f202b"),
        Guid.Parse("2ea745cc-7476-4e39-a2c1-048c9fd304f4"),
        Guid.Parse("14615405-f391-4f38-9de4-0c4aaf6a4341"),
        Guid.Parse("7c588df5-4b1a-4370-9f4c-33d0c351406d"),
        Guid.Parse("6a0e67e9-153e-4eea-a751-77f7043e32ef"),
        Guid.Parse("84282b24-8c11-44bb-9ccb-5c4c06ded57b"),
        Guid.Parse("d7efb26d-393c-4d5e-bfb6-12b3152f2994"),
        Guid.Parse("6cf44570-c980-4db9-b31b-c3cc77bba53a"),
        Guid.Parse("dd56e87c-7a54-41aa-86be-fc8137ca7b7c"),
        Guid.Parse("f76e0416-c5ac-4e51-bba9-a05a04734925"),
        Guid.Parse("8a9f2137-742e-4fb3-a296-fff2531be31c"),
        Guid.Parse("2dbe3bf2-d3da-4c30-a6d6-ebe217d09bd2"),
        Guid.Parse("22b2627d-6dd7-4f5b-8d20-40d70fd45293"),
        Guid.Parse("c2757be0-5f0b-4fcb-9c19-39eeaf94eb06"),
        Guid.Parse("438b03ef-47f8-4d60-813a-ce2b46957f0e"),
        Guid.Parse("f22eac28-0c2e-4d8b-ae4d-67b55af3ed52"),
        Guid.Parse("c55b4812-9ba4-4bd2-9874-beb676a642c5"),
        Guid.Parse("92ce36f2-8f70-4a43-ae1d-acb047ec958f"),
        Guid.Parse("aeb992bf-e055-4816-a9f7-f54e4d4ffc37"),
        Guid.Parse("fa9809c4-99b4-4082-818c-00c2c6f30638"),
        Guid.Parse("9d68d0eb-6d99-48c9-b4b5-b11199e95344"),
        Guid.Parse("049959be-cfa4-4a21-a0d4-0850a558f42d"),
        Guid.Parse("52e70c7c-bb16-4a56-b0cb-ca8779042c90"),
        Guid.Parse("325bd2b0-e904-4ce1-b96d-5ea42b4c9712"),
        Guid.Parse("ab580cb6-96ae-4ff1-97a5-274518ba79e8"),
    ];
}
