using DigitalRightsManagement.Domain.UserAggregate;
using System.Collections.Frozen;

namespace DigitalRightsManagement.MigrationService;

public static class SeedData
{
    public static FrozenSet<User> Users =>
    [
        // Admins
        User.Create("John Doe", "john.doe@localhost.com", UserRoles.Admin, Guid.Parse("34f87c92-4f2e-4350-8f70-f2c4c2a449a2")),
        User.Create("Jane Smith", "jane.smith@localhost.com", UserRoles.Admin, Guid.Parse("de3c7c39-6cf5-4eb0-8f97-3c3f2ed93575")),
      
        // Managers
        User.Create("Chris Taylor", "chris.taylor@localhost.com", UserRoles.Manager, Guid.Parse("bb6346b1-4924-4e69-9099-8e81f84c5123")),
        User.Create("Sarah Moore", "sarah.moore@localhost.com", UserRoles.Manager, Guid.Parse("08c7acee-0f8c-4e70-9f72-c59db53ae0be")),
        User.Create("James Anderson", "james.anderson@localhost.com", UserRoles.Manager, Guid.Parse("3d792f3d-8764-49c1-ae2b-40444ffbb2f9")),
        User.Create("Linda Thomas", "linda.thomas@localhost.com", UserRoles.Manager, Guid.Parse("b404a193-c0b8-4807-b4ef-aa442ce5acbe")),
        User.Create("Robert Jackson", "robert.jackson@localhost.com", UserRoles.Manager, Guid.Parse("245ad689-41e1-40c4-9114-2dc89d88e53b")),
        User.Create("Patricia White", "patricia.white@localhost.com", UserRoles.Manager, Guid.Parse("2965854b-70fa-41ed-a7ce-d1a83973124f")),
        User.Create("Mark Harris", "mark.harris@localhost.com", UserRoles.Manager, Guid.Parse("8f2703f7-96a6-4a75-b663-ec027e4b513e")),
        User.Create("Barbara Martin", "barbara.martin@localhost.com", UserRoles.Manager, Guid.Parse("58bf001c-bb9a-41fd-8917-c6eaf110af90")),
        User.Create("Paul Thompson", "paul.thompson@localhost.com", UserRoles.Manager, Guid.Parse("cd9f1577-06b4-4f13-8901-3c97f04ada30")),
        User.Create("Nancy Garcia", "nancy.garcia@localhost.com", UserRoles.Manager, Guid.Parse("05aebb26-5611-4eda-9cc0-73bd50c6fddf")),
    
        // Viewers
        User.Create("Daniel King", "daniel.king@localhost.com", UserRoles.Viewer, Guid.Parse("a01d3277-a55b-411d-b526-662f4df0bbe7")),
        User.Create("Jessica Wright", "jessica.wright@localhost.com", UserRoles.Viewer, Guid.Parse("ff043c9e-a1a2-4ed5-97c9-0ed1de3304f8")),
        User.Create("Matthew Lopez", "matthew.lopez@localhost.com", UserRoles.Viewer, Guid.Parse("cd86fe59-81d6-4e9a-8d02-dd71613f202b")),
        User.Create("Ashley Hill", "ashley.hill@localhost.com", UserRoles.Viewer, Guid.Parse("2ea745cc-7476-4e39-a2c1-048c9fd304f4")),
        User.Create("Joshua Scott", "joshua.scott@localhost.com", UserRoles.Viewer, Guid.Parse("14615405-f391-4f38-9de4-0c4aaf6a4341")),
        User.Create("Amanda Green", "amanda.green@localhost.com", UserRoles.Viewer, Guid.Parse("7c588df5-4b1a-4370-9f4c-33d0c351406d")),
        User.Create("Andrew Adams", "andrew.adams@localhost.com", UserRoles.Viewer, Guid.Parse("6a0e67e9-153e-4eea-a751-77f7043e32ef")),
        User.Create("Megan Baker", "megan.baker@localhost.com", UserRoles.Viewer, Guid.Parse("84282b24-8c11-44bb-9ccb-5c4c06ded57b")),
        User.Create("Justin Gonzalez", "justin.gonzalez@localhost.com", UserRoles.Viewer, Guid.Parse("d7efb26d-393c-4d5e-bfb6-12b3152f2994")),
        User.Create("Lauren Nelson", "lauren.nelson@localhost.com", UserRoles.Viewer, Guid.Parse("6cf44570-c980-4db9-b31b-c3cc77bba53a")),
        User.Create("Ethan Carter", "ethan.carter@localhost.com", UserRoles.Viewer, Guid.Parse("dd56e87c-7a54-41aa-86be-fc8137ca7b7c")),
        User.Create("Nicole Mitchell", "nicole.mitchell@localhost.com", UserRoles.Viewer, Guid.Parse("f76e0416-c5ac-4e51-bba9-a05a04734925")),
        User.Create("Ryan Perez", "ryan.perez@localhost.com", UserRoles.Viewer, Guid.Parse("8a9f2137-742e-4fb3-a296-fff2531be31c")),
        User.Create("Rachel Roberts", "rachel.roberts@localhost.com", UserRoles.Viewer, Guid.Parse("2dbe3bf2-d3da-4c30-a6d6-ebe217d09bd2")),
        User.Create("Tyler Turner", "tyler.turner@localhost.com", UserRoles.Viewer, Guid.Parse("22b2627d-6dd7-4f5b-8d20-40d70fd45293")),
        User.Create("Olivia Phillips", "olivia.phillips@localhost.com", UserRoles.Viewer, Guid.Parse("c2757be0-5f0b-4fcb-9c19-39eeaf94eb06")),
        User.Create("Brandon Campbell", "brandon.campbell@localhost.com", UserRoles.Viewer, Guid.Parse("438b03ef-47f8-4d60-813a-ce2b46957f0e")),
        User.Create("Emma Parker", "emma.parker@localhost.com", UserRoles.Viewer, Guid.Parse("f22eac28-0c2e-4d8b-ae4d-67b55af3ed52")),
        User.Create("Alexander Evans", "alexander.evans@localhost.com", UserRoles.Viewer, Guid.Parse("c55b4812-9ba4-4bd2-9874-beb676a642c5")),
        User.Create("Samantha Edwards", "samantha.edwards@localhost.com", UserRoles.Viewer, Guid.Parse("92ce36f2-8f70-4a43-ae1d-acb047ec958f")),
        User.Create("Jacob Collins", "jacob.collins@localhost.com", UserRoles.Viewer, Guid.Parse("aeb992bf-e055-4816-a9f7-f54e4d4ffc37")),
        User.Create("Sophia Stewart", "sophia.stewart@localhost.com", UserRoles.Viewer, Guid.Parse("fa9809c4-99b4-4082-818c-00c2c6f30638")),
        User.Create("Logan Sanchez", "logan.sanchez@localhost.com", UserRoles.Viewer, Guid.Parse("9d68d0eb-6d99-48c9-b4b5-b11199e95344")),
        User.Create("Hannah Morris", "hannah.morris@localhost.com", UserRoles.Viewer, Guid.Parse("049959be-cfa4-4a21-a0d4-0850a558f42d")),
        User.Create("Jackson Rogers", "jackson.rogers@localhost.com", UserRoles.Viewer, Guid.Parse("52e70c7c-bb16-4a56-b0cb-ca8779042c90")),
        User.Create("Aiden Cook", "aiden.cook@localhost.com", UserRoles.Viewer, Guid.Parse("325bd2b0-e904-4ce1-b96d-5ea42b4c9712")),
        User.Create("Isabella Barnes", "isabella.barnes@localhost.com", UserRoles.Viewer, Guid.Parse("ab580cb6-96ae-4ff1-97a5-274518ba79e8"))
    ];
}
