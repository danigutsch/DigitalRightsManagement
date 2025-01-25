[![.NET](https://github.com/danigutsch/DigitalRightsManagement/actions/workflows/dotnet.yml/badge.svg)](https://github.com/danigutsch/DigitalRightsManagement/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

# Digital Rights Management

A **cloud-native Digital Rights Management** system built with **.NET 9** and **.NET Aspire**, showcasing modern C# features, **Domain-Driven Design (DDD)**, and **clean architecture** principles. This repository demonstrates how to build a **production-ready distributed** application that focuses on security, scalability, and maintainability in cloud-native environments.

---

## Table of Contents
1. [Overview](#overview)  
2. [Core Features](#core-features)  
3. [Architecture](#architecture)  
    - [Clean Architecture](#clean-architecture)  
    - [Domain-Driven Design](#domain-driven-design)  
4. [Technical Stack](#technical-stack)  
    - [Implemented](#implemented)  
    - [Planned](#planned)  
5. [Getting Started](#getting-started)  
    - [Prerequisites](#prerequisites)  
    - [Running the Application](#running-the-application)  
    - [Test Credentials](#test-credentials)  
6. [Development](#development)  
    - [Project Structure](#project-structure)  
    - [Key Design Principles](#key-design-principles)  
    - [Infrastructure Configurations](#infrastructure-configurations)  
    - [Package Distribution](#package-distribution)  
7. [Contributing](#contributing)  
8. [License](#license)

---

## Overview

The **Digital Rights Management API** provides a robust framework for handling resource ownership, permissions, and product lifecycle management. Leveraging **.NET 9** and the **.NET Aspire** ecosystem, this project employs enterprise architectural patterns to ensure high availability, scalability, and testability in cloud-native environments.

---

## Core Features

- **Product Lifecycle Management**: Control the entire lifecycle of DRM-protected assets  
- **Resource Ownership & Permissions**: Granular access control with domain-driven aggregates  
- **Distributed Caching**: Improves performance and scalability with Redis  
- **Event-Driven Updates**: Ensures consistent state and real-time changes across microservices  
- **Comprehensive Testing**: Includes unit tests, integration tests, and health checks  

---

## Architecture

### Clean Architecture
- **Domain**: Core business models and logic  
- **Application**: Use cases and interfaces  
- **Infrastructure**: Technical implementations (persistence, caching, external services)  
- **API**: HTTP endpoints and contracts  

### Domain-Driven Design
- **Bounded Contexts**: Logical separation of sub-domains  
- **Rich Domain Models**: Business rules and invariants encapsulated in entities and value objects  
- **Aggregate Roots**: Transactional consistency boundaries  
- **Domain Events**: Publish/subscribe mechanism for decoupling and event-driven workflows  
- **Value Objects**: Immutable data types for guaranteed integrity  
- **Ubiquitous Language**: Consistent language for domain experts and developers  

---

## Technical Stack

### Implemented

**Architecture & Design**
- [x] Clean Architecture  
- [x] Domain-Driven Design  
- [x] CQRS pattern  
- [x] Event-driven architecture  

**Infrastructure**
- [x] .NET Aspire cloud-native setup  
- [x] PostgreSQL persistence  
- [x] Redis distributed caching  
- [x] Basic authentication  
- [x] OpenAPI/Swagger documentation  

**Quality & Testing**
- [x] Unit testing suite  
- [x] Integration tests with containers  
- [x] Health checks  
- [x] OpenTelemetry observability  
- [x] Problem Details error handling  

### Planned

**Documentation & Architecture**
- [ ] Domain model diagrams  
- [ ] Bounded context map  
- [ ] Aggregate relationship diagrams  
- [ ] Event flow documentation  
- [ ] Architecture decision records (ADRs)  
- [ ] Domain description and glossary  

**Architecture & Patterns**
- [ ] Event Sourcing  
- [ ] Outbox pattern  
- [ ] Read/write segregation  
- [ ] Multi-tenancy  
- [ ] Binary data handling  
- [ ] Efficient media file processing  
- [ ] Document storage patterns  

**Infrastructure & Performance**
- [ ] Garnet caching integration  
- [ ] Hybrid caching system  
- [ ] SQLite support  
- [ ] Full-text search  
- [ ] AOT compilation  
- [ ] Response compression  
- [ ] Memory cache management  
- [ ] Query optimization  
- [ ] Lazy loading implementation  
- [ ] Batch processing system  

**Communication & Integration**
- [ ] Message broker integration  
- [ ] gRPC services  
- [ ] GraphQL endpoint  
- [ ] Real-time updates (SignalR)  
- [ ] Email notifications  
- [ ] API versioning  
- [ ] Front-end frameworks integration  

**Storage & Data**
- [ ] S3 storage  
- [ ] Document DB support  
- [ ] Data streaming (Kafka)  
- [ ] Storage tenant system  
- [ ] Data archival/retention  
- [ ] Background job processing  

**Authentication & Security**
- [ ] Bearer token authentication  
- [ ] OpenID Connect/OAuth2  
- [ ] Keycloak integration  
- [ ] Duende Identity Server  
- [ ] Rate limiting  
- [ ] Request idempotency  
- [ ] Secrets management  
- [ ] API key management  
- [ ] Audit logging  
- [ ] Data encryption  
- [ ] GDPR compliance  
- [ ] Role-based file access  

**Testing & Quality**
- [ ] Gherkin behavior tests  
- [ ] Feature flags & toggles  
- [ ] Snapshot testing  
- [ ] Multi-configuration testing  
- [ ] AI integration  
- [ ] Performance benchmarks  
- [ ] Distributed tracing  

**DevOps & Deployment**
- [ ] Cloud deployment templates  
- [ ] Blue-green deployments  
- [ ] Infrastructure as Code  
- [ ] Monitoring dashboards  
- [ ] Backup strategies  
- [ ] Database maintenance  
- [ ] Production migration practices  
- [ ] Auto semantic versioning in CI  
- [ ] NuGet package distribution  
- [ ] Project templates  
- [ ] Development containers  

---

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/)  
- [Docker Desktop](https://www.docker.com/products/docker-desktop)  
- Entity Framework Core tools:
  ```bash
  dotnet tool install --global dotnet-ef
