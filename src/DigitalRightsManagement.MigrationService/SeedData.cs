using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using System.Collections.Frozen;

namespace DigitalRightsManagement.MigrationService;

public static class SeedData
{
    static SeedData()
    {
        var users = UsersAndPasswords.Select(x =>
        {
            var (user, _) = x;

            var productsToAdd = Products
                .Where(product => product.Manager == user.Id)
                .ExceptBy(user.Products, product => product.Id);

            user.AddProducts(productsToAdd);

            return user;
        });

        Users = [..users];

        Passwords = UsersAndPasswords.ToFrozenDictionary(x => x.User.Id, x => x.Password);
    }

    public static FrozenDictionary<Guid, string> Passwords { get;}

    public static IReadOnlyList<User> Users { get; }

    public static IReadOnlyList<(User User, string Password)> UsersAndPasswords { get; } =
    [
        // Admins
        (User.Create("admin1", "admin1@example.com", UserRoles.Admin, Guid.Parse("34f87c92-4f2e-4350-8f70-f2c4c2a449a2")).Value, "i0%5Nr8vB^2LL9T$XYfU23%EVnvY0G"),
        (User.Create("admin2", "admin2@example.com", UserRoles.Admin, Guid.Parse("de3c7c39-6cf5-4eb0-8f97-3c3f2ed93575")).Value, "XH2%%4^T27jd%%wxh5^606irij6*5T"),

        // Managers
        (User.Create("manager1", "manager1@example.com", UserRoles.Manager, Guid.Parse("bb6346b1-4924-4e69-9099-8e81f84c5123")).Value, "qCxcPqi55Ly9X$63aU#M$9HBq5jnSV"),
        (User.Create("manager2", "manager2@example.com", UserRoles.Manager, Guid.Parse("08c7acee-0f8c-4e70-9f72-c59db53ae0be")).Value, "E1qk5OhxS6#!3A1*gco*##8iF!#s&B"),
        (User.Create("manager3", "manager3@example.com", UserRoles.Manager, Guid.Parse("3d792f3d-8764-49c1-ae2b-40444ffbb2f9")).Value, "!%h2kH$%9Y*x@%D1@i0Q!@xLDg*%8E"),
        (User.Create("manager4", "manager4@example.com", UserRoles.Manager, Guid.Parse("58bf001c-bb9a-41fd-8917-c6eaf110af90")).Value, "OP41%z4&2x^B!Bkwr%*0p^CPw9m4^Z"),
        (User.Create("manager5", "manager5@example.com", UserRoles.Manager, Guid.Parse("cd9f1577-06b4-4f13-8901-3c97f04ada30")).Value, "@sK#8!n6YLuM081444&#j%4$3^oq*A"),

        // Viewers
        (User.Create("viewer1", "viewer1@example.com", UserRoles.Viewer, Guid.Parse("a01d3277-a55b-411d-b526-662f4df0bbe7")).Value, "v@2q$65NbuFJRE$N3$0u6s&p2K9*aJ"),
        (User.Create("viewer2", "viewer2@example.com", UserRoles.Viewer, Guid.Parse("ff043c9e-a1a2-4ed5-97c9-0ed1de3304f8")).Value, "zLZqvS9!7O7Q@QV6$6b@*1k&$73ogZ"),
        (User.Create("viewer3", "viewer3@example.com", UserRoles.Viewer, Guid.Parse("cd86fe59-81d6-4e9a-8d02-dd71613f202b")).Value, "19&k6d1$yu*j25rfA4On&z!5!E&VpR"),
        (User.Create("viewer4", "viewer4@example.com", UserRoles.Viewer, Guid.Parse("2ea745cc-7476-4e39-a2c1-048c9fd304f4")).Value, "#&&*^d^9a#mH4Qt@3$*H^p*3GmKdiU"),
        (User.Create("viewer5", "viewer5@example.com", UserRoles.Viewer, Guid.Parse("14615405-f391-4f38-9de4-0c4aaf6a4341")).Value, "Ugn4@r*vwc&6%7*e0&A@@Xyw^!y3dS"),
        (User.Create("viewer6", "viewer6@example.com", UserRoles.Viewer, Guid.Parse("7c588df5-4b1a-4370-9f4c-33d0c351406d")).Value, "saoYy5Q194**0O#1!E8f6iK2o9o*!V"),
        (User.Create("viewer7", "viewer7@example.com", UserRoles.Viewer, Guid.Parse("6a0e67e9-153e-4eea-a751-77f7043e32ef")).Value, "&K!c$&j@*VLR38rj4%^%iY@Aaz@@nL"),
        (User.Create("viewer8", "viewer8@example.com", UserRoles.Viewer, Guid.Parse("84282b24-8c11-44bb-9ccb-5c4c06ded57b")).Value, "!d93S!m3N1i5Shk$pgCL74*BADBU*J"),
        (User.Create("viewer9", "viewer9@example.com", UserRoles.Viewer, Guid.Parse("d7efb26d-393c-4d5e-bfb6-12b3152f2994")).Value, "*@!qa2p5S$xd@A%7Y0Tj3sqN5N34wW"),
        (User.Create("viewer10", "viewer10@example.com", UserRoles.Viewer, Guid.Parse("6cf44570-c980-4db9-b31b-c3cc77bba53a")).Value, "77#9970#HXF59Ax60&13$Nu*^sv!bB")
    ];

    public static IReadOnlyList<Product> Products { get; } =
    [
        Product.Create("Product1", "Description1", Price.Create(10, Currency.Dollar).Value, Guid.Parse("08c7acee-0f8c-4e70-9f72-c59db53ae0be")).Value,
        Product.Create("Product2", "Description2", Price.Create(20, Currency.Dollar).Value, Guid.Parse("3d792f3d-8764-49c1-ae2b-40444ffbb2f9")).Value,
        Product.Create("Product3", "Description3", Price.Create(30, Currency.Dollar).Value, Guid.Parse("58bf001c-bb9a-41fd-8917-c6eaf110af90")).Value,
        Product.Create("Product4", "Description4", Price.Create(40, Currency.Dollar).Value, Guid.Parse("58bf001c-bb9a-41fd-8917-c6eaf110af90")).Value,
        Product.Create("Product5", "Description5", Price.Create(50, Currency.Dollar).Value, Guid.Parse("cd9f1577-06b4-4f13-8901-3c97f04ada30")).Value,
        Product.Create("Product6", "Description6", Price.Create(60, Currency.Dollar).Value, Guid.Parse("cd9f1577-06b4-4f13-8901-3c97f04ada30")).Value,
        Product.Create("Product7", "Description7", Price.Create(70, Currency.Dollar).Value, Guid.Parse("cd9f1577-06b4-4f13-8901-3c97f04ada30")).Value
    ];
}
