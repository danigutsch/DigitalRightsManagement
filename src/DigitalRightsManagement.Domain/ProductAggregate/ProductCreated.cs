﻿using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed record ProductCreated(string Name, string Description, Price Price) : DomainEvent;
