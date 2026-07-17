# 💉 EasyCore.Dependency

> **EasyCore.Dependency** is an automatic dependency-injection library for .NET 8. Implement `IScopedDependency` / `ISingletonDependency` / `ITransientDependency`, and services are registered at startup — no hand-written `AddScoped` / `AddSingleton` / `AddTransient` boilerplate.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp)
![DI](https://img.shields.io/badge/DI-Microsoft.Extensions-blueviolet)
![Features](https://img.shields.io/badge/Features-Scoped%20%7C%20Singleton%20%7C%20Transient-blue)
![License](https://img.shields.io/badge/License-Apache--2.0-yellow)
![Version](https://img.shields.io/badge/Version-8.0.0-blue)
![NuGet](https://img.shields.io/nuget/v/EasyCore.Dependency?label=NuGet)

---

## 🌍 Language

- 🇨🇳 Chinese: [README.md](https://github.com/RockyWang0521/EasyCore.Dependency/blob/master/README.md)
- 🇺🇸 **English (this document)**

---

## 📚 Table of Contents

### 🗺️ Part I — Overview & Architecture
- [1. 🎯 Positioning](#1--positioning)
- [2. 🏗️ Architecture & Registration Flow](#2-️-architecture--registration-flow)
- [3. 📦 NuGet / Projects](#3--nuget--projects)
- [4. 📊 Capability Matrix](#4--capability-matrix)

### 🚀 Part II — Getting Started
- [5. 💻 Requirements](#5--requirements)
- [6. 📥 Installation](#6--installation)
- [7. ⚡ Quick Start (3 minutes)](#7--quick-start-3-minutes)
- [8. ⚙️ API & Options](#8-️-api--options)

### 🧩 Part III — Registration Patterns
- [9. 🔌 Interface Abstraction (Preferred)](#9--interface-abstraction-preferred)
- [10. 🧱 Concrete Self-Registration](#10--concrete-self-registration)
- [11. 🔗 Multiple Business Interfaces](#11--multiple-business-interfaces)
- [12. 🧭 Assembly Scan Strategy](#12--assembly-scan-strategy)

### 🏭 Part IV — Demo · Migration · Production
- [13. 🧪 Demo & Tests](#13--demo--tests)
- [14. 🔄 Migrating from Older Versions](#14--migrating-from-older-versions)
- [15. ✅ Checklist](#15--checklist)
- [16. ❓ FAQ](#16--faq)
- [17. 📄 License](#17--license)

---

## 1. 🎯 Positioning

EasyCore.Dependency reduces DI boilerplate in ASP.NET Core and generic hosts:

| Pain point | EasyCore.Dependency approach |
|---|---|
| Hand-written `AddScoped` / `AddSingleton` for every service | Marker interfaces + startup scan |
| Lifetime scattered across registrations | Lifetime declared on the interface or type |
| Multi-interface same-impl hard to keep as one instance | Register concrete once; forward interfaces |
| Directory `LoadFrom` pulls unrelated DLLs | Entry + already-loaded assemblies; optional prefixes |
| Re-registration overwrites existing services | `TryAdd` throughout — safe to call repeatedly |

### 1.1 ✨ Design Principles

| Principle | Meaning |
|---|---|
| **Low friction** | One call: `AddEasyCoreDependency()` |
| **Convention over config** | Markers declare lifetime and registration intent |
| **Safe defaults** | `TryAdd`; skip `System.` / `Microsoft.` assemblies |
| **Controllable scan** | Explicit assemblies or `AssemblyNamePrefixes` |
| **Multi-interface consistency** | Shared instance across business interfaces |

### 1.2 📁 Repository Layout

```text
EasyCore.Dependency/
├── src/EasyCore.Dependency/              # Core library
│   └── Dependency/
│       ├── Base/IBaseDependency.cs       # Marker base
│       ├── IScopedDependency.cs
│       ├── ISingletonDependency.cs
│       ├── ITransientDependency.cs
│       ├── DependencyRegistrationOptions.cs
│       └── ServiceCollectionExtensions.cs
├── demo/Web.Dependency/                  # Runnable sample + Swagger
├── tests/EasyCore.Dependency.Tests/      # Unit tests
└── docs/png/                             # README diagrams (PNG for NuGet)
```

---

## 2. 🏗️ Architecture & Registration Flow

### 2.1 🖼️ Component Diagram

![architecture-en](https://raw.githubusercontent.com/RockyWang0521/EasyCore.Dependency/master/docs/png/architecture-en.png)

### 2.2 🔁 Registration Flow

![registration-flow-en](https://raw.githubusercontent.com/RockyWang0521/EasyCore.Dependency/master/docs/png/registration-flow-en.png)

### 2.3 📜 Data Flow

```text
[Program.cs]
      │
      ▼
AddEasyCoreDependency(options?)
      │
      ├─ ResolveAssemblies
      │     · Explicit Assemblies, or
      │     · Entry + loaded non-framework assemblies
      │     · Optional AssemblyNamePrefixes filter
      │
      ├─ GetConcreteDependencyTypes
      │     · Non-abstract · non-open-generic · IBaseDependency
      │
      └─ RegisterType
            · Exactly one lifetime marker → Scoped / Singleton / Transient
            · 0 business interfaces → register concrete as self
            · 1 business interface → interface → implementation
            · N business interfaces → concrete + forward each interface
            · TryAdd (never overwrites existing registrations)
```

---

## 3. 📦 NuGet / Projects

| Package / Project | Role | Required |
|---|---|---|
| [`EasyCore.Dependency`](https://www.nuget.org/packages/EasyCore.Dependency) | Markers + auto-registration | ✅ |
| `demo/Web.Dependency` | Web API sample | Sample |
| `tests/EasyCore.Dependency.Tests` | Unit tests | Dev |

Repository: [github.com/RockyWang0521/EasyCore.Dependency](https://github.com/RockyWang0521/EasyCore.Dependency)

---

## 4. 📊 Capability Matrix

| Capability | Description | Surface |
|---|---|---|
| Scoped | One per scope | `IScopedDependency` |
| Singleton | One per process | `ISingletonDependency` |
| Transient | New each resolve | `ITransientDependency` |
| Interface → impl | Business interface inherits marker | `AddEasyCoreDependency` |
| Concrete self | Type implements marker directly | Same |
| Multi-iface same instance | Forward to one concrete | Automatic |
| Explicit assemblies | `params Assembly[]` | Overload |
| Prefix filter | `AssemblyNamePrefixes` | Options |
| Idempotent | `TryAdd` | Built-in |

### 4.1 🌳 Decision Tree

```text
Need auto DI?
└── AddEasyCoreDependency()

How to expose the service?
├── Preferred: business iface : IScopedDependency (etc.) → impl only business iface
├── Helpers: concrete : ISingletonDependency → inject concrete
└── Split read/write: multiple business ifaces → one impl, same instance

Scan scope?
├── Default: entry + loaded app assemblies
├── Precise: AddEasyCoreDependency(typeof(X).Assembly)
└── Filter: options.AssemblyNamePrefixes.Add("MyApp")
```

---

## 5. 💻 Requirements

| Item | Requirement |
|---|---|
| .NET | 8.0+ |
| Host | ASP.NET Core / generic host / console (`IServiceCollection`) |
| Dependency | `Microsoft.Extensions.DependencyInjection.Abstractions` (brought by this package) |

---

## 6. 📥 Installation

```bash
dotnet add package EasyCore.Dependency
```

Or in the project file:

```xml
<PackageReference Include="EasyCore.Dependency" Version="8.0.0" />
```

Project reference:

```xml
<ProjectReference Include="..\..\src\EasyCore.Dependency\EasyCore.Dependency.csproj" />
```

---

## 7. ⚡ Quick Start (3 minutes)

### 7️⃣.1️⃣ Define a service

```csharp
using EasyCore.Dependency;

public interface IUserService : IScopedDependency
{
    string GetName();
}

public class UserService : IUserService
{
    public string GetName() => "EasyCore";
}
```

### 7️⃣.2️⃣ Register and inject

```csharp
using EasyCore.Dependency;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Scan entry assembly + loaded non-framework assemblies
builder.Services.AddEasyCoreDependency();

var app = builder.Build();
app.MapControllers();
app.Run();
```

```csharp
public class HomeController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public string Get() => userService.GetName();
}
```

Open the demo Swagger UI to verify injection.

---

## 8. ⚙️ API & Options

### 8.1 Extension methods

| Method | Description |
|---|---|
| `AddEasyCoreDependency()` | Default scan strategy |
| `AddEasyCoreDependency(params Assembly[])` | Scan only these assemblies |
| `AddEasyCoreDependency(Action<DependencyRegistrationOptions>)` | Full options |

### 8.2 `DependencyRegistrationOptions`

| Option | Description |
|---|---|
| `Assemblies` | Explicit scan list; empty ⇒ entry + loaded non-framework |
| `AssemblyNamePrefixes` | Include only assemblies whose name starts with a prefix |
| `AddAssemblies(...)` | Fluent append |

```csharp
builder.Services.AddEasyCoreDependency(options =>
{
    options.AddAssemblies(typeof(Program).Assembly);
    options.AssemblyNamePrefixes.Add("MyApp");
});
```

### 8.3 Lifetime mapping

| Marker | DI lifetime |
|---|---|
| `IScopedDependency` | Scoped |
| `ISingletonDependency` | Singleton |
| `ITransientDependency` | Transient |

> ⚠️ A type may implement **only one** lifetime marker; multiple markers throw `InvalidOperationException`.

---

## 9. 🔌 Interface Abstraction (Preferred)

Business interface inherits the lifetime marker; the implementation only implements the business interface:

```csharp
public interface IUserService : IScopedDependency
{
    string GetName();
}

public class UserService : IUserService
{
    public string GetName() => "EasyCore";
}
```

Result: `IUserService` → `UserService` (Scoped).

---

## 10. 🧱 Concrete Self-Registration

When the concrete type implements the marker directly, it is registered as itself:

```csharp
public class CacheHelper : ISingletonDependency
{
    public string Get(string key) => key;
}

// Inject the concrete type
public class DemoController(CacheHelper cacheHelper) : ControllerBase
{
}
```

---

## 11. 🔗 Multiple Business Interfaces

When one implementation implements multiple business interfaces, each is registered and resolves to the **same instance**:

```csharp
public interface IOrderReader : IScopedDependency { }
public interface IOrderWriter : IScopedDependency { }

public class OrderService : IOrderReader, IOrderWriter
{
}
```

Internally: register `OrderService` once, then forward `IOrderReader` / `IOrderWriter` to `GetRequiredService<OrderService>()`.

---

## 12. 🧭 Assembly Scan Strategy

| Scenario | Usage |
|---|---|
| Default | `AddEasyCoreDependency()` |
| Explicit | `AddEasyCoreDependency(typeof(Program).Assembly)` |
| Prefix filter | `options.AssemblyNamePrefixes.Add("MyApp")` |

**Excluded by default**: `System.` / `Microsoft.` / `mscorlib` / `netstandard` and dynamic assemblies.

**No** directory-wide `Assembly.LoadFrom` — avoids loading unrelated DLLs.

---

## 13. 🧪 Demo & Tests

![demo-topology-en](https://raw.githubusercontent.com/RockyWang0521/EasyCore.Dependency/master/docs/png/demo-topology-en.png)

| Project | Role | Command |
|---|---|---|
| [`demo/Web.Dependency`](demo/Web.Dependency) | Six scenarios + Swagger | `dotnet run --project demo/Web.Dependency` |
| [`tests/EasyCore.Dependency.Tests`](tests/EasyCore.Dependency.Tests) | Lifetime / multi-iface / prefix | `dotnet test EasyCore.Dependency.sln` |

Demo endpoints (`DependencyController`):

| Endpoint | Description |
|---|---|
| `GET /api/Dependency/ScopedTest` | Interface + Scoped |
| `GET /api/Dependency/SingletonTest` | Interface + Singleton |
| `GET /api/Dependency/TransientTest` | Interface + Transient |
| `GET /api/Dependency/ScopedNotAbstractionTest` | Concrete + Scoped |
| `GET /api/Dependency/SingletonNotAbstractionTest` | Concrete + Singleton |
| `GET /api/Dependency/TransientNotAbstractionTest` | Concrete + Transient |

```bash
dotnet run --project demo/Web.Dependency
# Open Swagger and call the endpoints above

dotnet test EasyCore.Dependency.sln
```

---

## 14. 🔄 Migrating from Older Versions

From **8.0.0**, package and API spelling is corrected (Dependencie → Dependency):

| Old | New |
|---|---|
| `EasyCore.Dependencie` | `EasyCore.Dependency` |
| `IScopedDependencie` etc. | `IScopedDependency` etc. |
| `services.EasyCoreDependencie()` | `services.AddEasyCoreDependency()` |

Upgrade steps:

1. Update the NuGet package to `EasyCore.Dependency`
2. Replace namespaces and interface names globally
3. Switch the extension method to `AddEasyCoreDependency`

---

## 15. ✅ Checklist

- [ ] Prefer “business interface inherits marker + impl only implements business interface”
- [ ] Each implementation implements exactly one lifetime marker
- [ ] In large solutions, narrow the scan with `Assemblies` or `AssemblyNamePrefixes`
- [ ] Coexist with hand-written `Add*` via `TryAdd` (this library does not overwrite)
- [ ] Run `dotnet test EasyCore.Dependency.sln` in CI
- [ ] Assert `Same` for multi-interface scenarios in tests

---

## 16. ❓ FAQ

**Q: Why wasn't my service registered?**  
A: Ensure it implements a lifetime marker; ensure its assembly is loaded or included via `AddAssemblies` / prefixes; ensure it is not abstract or an open generic.

**Q: Can I implement both Scoped and Singleton?**  
A: No. That throws `InvalidOperationException` — keep only one marker.

**Q: Conflict with `services.AddScoped<IFoo, Foo>()`?**  
A: This library uses `TryAdd`. Existing registrations for the same service type are not overwritten.

**Q: Why not scan every DLL under bin?**  
A: Full `LoadFrom` easily loads unrelated or not-yet-ready assemblies. Default is entry + loaded app assemblies, with explicit/prefix control.

**Q: Do multiple business interfaces resolve to the same instance?**  
A: Yes. The concrete type is registered once; interfaces forward to that same implementation.

---

## 17. 📄 License

Apache License 2.0 — see [LICENSE](LICENSE).

Copyright 2025 Rocky Wang

---

## 🤝 Contributing

1. Fork and create a feature branch  
2. Add tests under `tests/EasyCore.Dependency.Tests`  
3. Run `dotnet test` and `dotnet build EasyCore.Dependency.sln`  
4. Open a pull request  

Issues and PRs are welcome 🚀

Repository: <https://github.com/RockyWang0521/EasyCore.Dependency>
