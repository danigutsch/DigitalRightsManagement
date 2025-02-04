namespace DigitalRightsManagement.UnitTests.Analyzers.Infrastructure.TestFramework;

public static class AnalyzerConstants
{
    public const string EntityBaseClass = """
                                          namespace DigitalRightsManagement.Common.DDD
                                          {
                                              using System;

                                              public abstract class Entity
                                              {
                                                  public Guid Id { get; init; }
                                                  protected Entity(Guid id) => Id = id;
                                                  protected Entity() { }
                                              }
                                          }
                                          """;
}