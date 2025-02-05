namespace DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework;

public static class AnalyzerConstants
{
    public const string EntityBaseClass = """
                                          namespace DigitalRightsManagement.Common.DDD;
                                          
                                          public abstract class Entity<TId> where TId : struct
                                          {
                                              public TId Id { get; init; }
                                              protected Entity(TId id) => Id = id;
                                              protected Entity() { }
                                          }
                                          
                                          """;
}