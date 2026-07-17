# 💉 EasyCore.Dependency

> **EasyCore.Dependency** 是面向 .NET 8 的自动依赖注入库。通过实现 `IScopedDependency` / `ISingletonDependency` / `ITransientDependency`，在启动时自动完成服务注册，无需手写大量 `AddScoped` / `AddSingleton` / `AddTransient`。

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp)
![DI](https://img.shields.io/badge/DI-Microsoft.Extensions-blueviolet)
![Features](https://img.shields.io/badge/Features-Scoped%20%7C%20Singleton%20%7C%20Transient-blue)
![License](https://img.shields.io/badge/License-Apache--2.0-yellow)
![Version](https://img.shields.io/badge/Version-8.0.0-blue)
![NuGet](https://img.shields.io/nuget/v/EasyCore.Dependency?label=NuGet)

---

## 🌍 Language

- 🇨🇳 **中文（当前文档）**
- 🇺🇸 English: [README.en.md](https://github.com/RockyWang0521/EasyCore.Dependency/blob/master/README.en.md)

---

## 📚 目录

### 🗺️ 第一部分：总览与架构
- [1. 🎯 项目定位](#1--项目定位)
- [2. 🏗️ 架构与注册流程](#2-️-架构与注册流程)
- [3. 📦 NuGet / 项目清单](#3--nuget--项目清单)
- [4. 📊 能力矩阵](#4--能力矩阵)

### 🚀 第二部分：快速上手
- [5. 💻 环境要求](#5--环境要求)
- [6. 📥 安装](#6--安装)
- [7. ⚡ 三分钟快速开始](#7--三分钟快速开始)
- [8. ⚙️ API 与选项说明](#8-️-api-与选项说明)

### 🧩 第三部分：注册模式详解
- [9. 🔌 接口抽象（推荐）](#9--接口抽象推荐)
- [10. 🧱 具体类直接注册](#10--具体类直接注册)
- [11. 🔗 多业务接口](#11--多业务接口)
- [12. 🧭 程序集扫描策略](#12--程序集扫描策略)

### 🏭 第四部分：Demo · 迁移 · 生产
- [13. 🧪 Demo 与测试](#13--demo-与测试)
- [14. 🔄 从旧版迁移](#14--从旧版迁移)
- [15. ✅ 使用清单](#15--使用清单)
- [16. ❓ FAQ](#16--faq)
- [17. 📄 License](#17--license)

---

## 1. 🎯 项目定位

EasyCore.Dependency 解决「在 ASP.NET Core / 通用宿主里少写样板 DI 注册」的问题：

| 痛点 | EasyCore.Dependency 做法 |
|---|---|
| 每个服务手写 `AddScoped` / `AddSingleton` | 标记接口 + 启动扫描自动注册 |
| 业务接口与生命周期散落各处 | 生命周期落在接口或实现类上，一目了然 |
| 多接口同实现难保证同一实例 | 具体类型注册一次，接口转发到同一实例 |
| 扫盘 `LoadFrom` 易加载无关 DLL | 仅扫描入口/已加载程序集，可按前缀过滤 |
| 重复注册覆盖已有服务 | 全程 `TryAdd`，重复调用安全 |

### 1.1 ✨ 设计原则

| 原则 | 说明 |
|---|---|
| **低摩擦接入** | 一行 `EasyCoreDependency()` 即可 |
| **约定优于配置** | 标记接口即声明生命周期与注册意图 |
| **安全默认** | `TryAdd`、排除 `System.` / `Microsoft.` 程序集 |
| **可控扫描** | 显式程序集或 `AssemblyNamePrefixes` |
| **多接口一致** | 多业务接口共享同一实现实例 |

### 1.2 📁 解决方案目录

```text
EasyCore.Dependency/
├── src/EasyCore.Dependency/              # 核心库
│   └── Dependency/
│       ├── Base/IBaseDependency.cs       # 标记基接口
│       ├── IScopedDependency.cs
│       ├── ISingletonDependency.cs
│       ├── ITransientDependency.cs
│       ├── DependencyRegistrationOptions.cs
│       └── ServiceCollectionExtensions.cs
├── demo/Web.Dependency/                  # 可运行示例 + Swagger
├── tests/EasyCore.Dependency.Tests/      # 单元测试
└── docs/png/                             # README 架构图（PNG，NuGet 可显示）
```

---

## 2. 🏗️ 架构与注册流程

### 2.1 🖼️ 组件关系图

![architecture-cn](https://raw.githubusercontent.com/RockyWang0521/EasyCore.Dependency/master/docs/png/architecture-cn.png)

### 2.2 🔁 注册流程

![registration-flow-cn](https://raw.githubusercontent.com/RockyWang0521/EasyCore.Dependency/master/docs/png/registration-flow-cn.png)

### 2.3 📜 数据流（文字版）

```text
[Program.cs]
      │
      ▼
EasyCoreDependency(options?)
      │
      ├─ ResolveAssemblies
      │     · 显式 Assemblies，或
      │     · 入口程序集 + 已加载非框架程序集
      │     · 可选 AssemblyNamePrefixes 过滤
      │
      ├─ GetConcreteDependencyTypes
      │     · 非抽象类 · 非开放泛型 · 实现 IBaseDependency
      │
      └─ RegisterType
            · 唯一生命周期标记 → Scoped / Singleton / Transient
            · 0 业务接口 → 注册具体类型
            · 1 业务接口 → 接口 → 实现类
            · N 业务接口 → 具体类型 + 各接口转发
            · TryAdd（不覆盖已有注册）
```

---

## 3. 📦 NuGet / 项目清单

| 包名 / 项目 | 职责 | 是否必须 |
|---|---|---|
| [`EasyCore.Dependency`](https://www.nuget.org/packages/EasyCore.Dependency) | 标记接口 + 自动注册扩展 | ✅ |
| `demo/Web.Dependency` | Web API 示例 | 示例 |
| `tests/EasyCore.Dependency.Tests` | 单元测试 | 开发 |

仓库：[github.com/RockyWang0521/EasyCore.Dependency](https://github.com/RockyWang0521/EasyCore.Dependency)

---

## 4. 📊 能力矩阵

| 能力 | 说明 | 表面 API |
|---|---|---|
| Scoped 注册 | 作用域内单例 | `IScopedDependency` |
| Singleton 注册 | 进程内单例 | `ISingletonDependency` |
| Transient 注册 | 每次解析新建 | `ITransientDependency` |
| 接口 → 实现 | 业务接口继承标记 | `EasyCoreDependency` |
| 具体类型注册 | 实现类直接实现标记 | 同上 |
| 多接口同实例 | 转发到同一实现 | 自动 |
| 显式程序集 | `params Assembly[]` | 重载 |
| 前缀过滤 | `AssemblyNamePrefixes` | Options |
| 幂等注册 | `TryAdd` | 内置 |

### 4.1 🌳 选型决策树

```text
需要自动 DI？
└── EasyCoreDependency()

服务如何暴露？
├── 推荐：业务接口 : IScopedDependency（等）→ 实现类只实现业务接口
├── 工具类：具体类 : ISingletonDependency → 注入具体类型
└── 读写分离：多个业务接口 → 同一实现类，自动同实例

扫描范围？
├── 默认：入口 + 已加载业务程序集
├── 精确：EasyCoreDependency(typeof(X).Assembly)
└── 过滤：options.AssemblyNamePrefixes.Add("MyApp")
```

---

## 5. 💻 环境要求

| 项 | 要求 |
|---|---|
| .NET | 8.0+ |
| 宿主 | ASP.NET Core / 通用宿主 / 控制台（有 `IServiceCollection` 即可） |
| 依赖 | `Microsoft.Extensions.DependencyInjection.Abstractions`（由本包引入） |

---

## 6. 📥 安装

```bash
dotnet add package EasyCore.Dependency
```

或在项目文件中引用：

```xml
<PackageReference Include="EasyCore.Dependency" Version="8.0.0" />
```

本地源码引用：

```xml
<ProjectReference Include="..\..\src\EasyCore.Dependency\EasyCore.Dependency.csproj" />
```

---

## 7. ⚡ 三分钟快速开始

### 7️⃣.1️⃣ 定义服务

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

### 7️⃣.2️⃣ 注册并注入

```csharp
using EasyCore.Dependency;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// 扫描入口程序集与已加载的非框架程序集
builder.Services.EasyCoreDependency();

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

打开 Demo 的 Swagger，即可验证注入是否生效。

---

## 8. ⚙️ API 与选项说明

### 8.1 扩展方法

| 方法 | 说明 |
|---|---|
| `EasyCoreDependency()` | 默认扫描策略 |
| `EasyCoreDependency(params Assembly[])` | 仅扫描指定程序集 |
| `EasyCoreDependency(Action<DependencyRegistrationOptions>)` | 完整选项配置 |

### 8.2 `DependencyRegistrationOptions`

| 选项 | 说明 |
|---|---|
| `Assemblies` | 显式扫描列表；为空时用入口程序集 + 已加载非框架程序集 |
| `AssemblyNamePrefixes` | 按程序集名前缀过滤（如 `"MyApp"`） |
| `AddAssemblies(...)` | 链式追加程序集 |

```csharp
builder.Services.EasyCoreDependency(options =>
{
    options.AddAssemblies(typeof(Program).Assembly);
    options.AssemblyNamePrefixes.Add("MyApp");
});
```

### 8.3 生命周期对照

| 标记接口 | DI 生命周期 |
|---|---|
| `IScopedDependency` | Scoped |
| `ISingletonDependency` | Singleton |
| `ITransientDependency` | Transient |

> ⚠️ 同一实现类只能实现**一个**生命周期标记；多个会抛出 `InvalidOperationException`。

---

## 9. 🔌 接口抽象（推荐）

业务接口继承生命周期标记，实现类只实现业务接口：

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

注册结果：`IUserService` → `UserService`（Scoped）。

---

## 10. 🧱 具体类直接注册

实现类直接继承标记接口时，按具体类型注册：

```csharp
public class CacheHelper : ISingletonDependency
{
    public string Get(string key) => key;
}

// 注入具体类型
public class DemoController(CacheHelper cacheHelper) : ControllerBase
{
}
```

---

## 11. 🔗 多业务接口

同一实现类实现多个业务接口时，每个接口都会注册，且解析到**同一实例**：

```csharp
public interface IOrderReader : IScopedDependency { }
public interface IOrderWriter : IScopedDependency { }

public class OrderService : IOrderReader, IOrderWriter
{
}
```

内部策略：先注册 `OrderService` 自身，再将 `IOrderReader` / `IOrderWriter` 转发到 `GetRequiredService<OrderService>()`。

---

## 12. 🧭 程序集扫描策略

| 场景 | 写法 |
|---|---|
| 默认 | `EasyCoreDependency()` |
| 指定程序集 | `EasyCoreDependency(typeof(Program).Assembly)` |
| 前缀过滤 | `options.AssemblyNamePrefixes.Add("MyApp")` |

**默认排除**：`System.` / `Microsoft.` / `mscorlib` / `netstandard` 以及动态程序集。

**不再**对目录做全盘 `Assembly.LoadFrom`，避免误加载无关 DLL。

---

## 13. 🧪 Demo 与测试

![demo-topology-cn](https://raw.githubusercontent.com/RockyWang0521/EasyCore.Dependency/master/docs/png/demo-topology-cn.png)

| 项目 | 角色 | 命令 |
|---|---|---|
| [`demo/Web.Dependency`](demo/Web.Dependency) | 六种注册场景 + Swagger | `dotnet run --project demo/Web.Dependency` |
| [`tests/EasyCore.Dependency.Tests`](tests/EasyCore.Dependency.Tests) | 生命周期 / 多接口 / 前缀 | `dotnet test EasyCore.Dependency.sln` |

Demo 提供的 API（`DependencyController`）：

| 端点 | 说明 |
|---|---|
| `GET /api/Dependency/ScopedTest` | 接口 + Scoped |
| `GET /api/Dependency/SingletonTest` | 接口 + Singleton |
| `GET /api/Dependency/TransientTest` | 接口 + Transient |
| `GET /api/Dependency/ScopedNotAbstractionTest` | 具体类 + Scoped |
| `GET /api/Dependency/SingletonNotAbstractionTest` | 具体类 + Singleton |
| `GET /api/Dependency/TransientNotAbstractionTest` | 具体类 + Transient |

```bash
dotnet run --project demo/Web.Dependency
# 打开 Swagger，调用上述端点

dotnet test EasyCore.Dependency.sln
```

---

## 14. 🔄 从旧版迁移

自 **8.0.0** 起，包名与 API 全面更正拼写（Dependencie → Dependency）：

| 旧 | 新 |
|---|---|
| `EasyCore.Dependencie` | `EasyCore.Dependency` |
| `IScopedDependencie` 等 | `IScopedDependency` 等 |
| `services.EasyCoreDependencie()` | `services.EasyCoreDependency()` |
| `services.AddEasyCoreDependency()` | `services.EasyCoreDependency()` |

升级步骤：

1. 更新 NuGet 包引用为 `EasyCore.Dependency`
2. 全局替换命名空间与接口名
3. 扩展方法改为 `EasyCoreDependency`

---

## 15. ✅ 使用清单

- [ ] 业务服务优先「接口继承标记 + 实现类只实现业务接口」
- [ ] 每个实现类只实现一种生命周期标记
- [ ] 大解决方案用 `Assemblies` 或 `AssemblyNamePrefixes` 缩小扫描范围
- [ ] 需要与手写 `Add*` 共存时依赖 `TryAdd`（本库不会覆盖已有注册）
- [ ] CI 执行 `dotnet test EasyCore.Dependency.sln`
- [ ] 多接口场景用单元测试验证 `Assert.Same`

---

## 16. ❓ FAQ

**Q: 为什么服务没有被注册？**  
A: 确认类型实现了生命周期标记；确认所在程序集已被加载或已通过 `AddAssemblies` / 前缀纳入扫描；确认不是抽象类或开放泛型。

**Q: 可以同时实现 Scoped 和 Singleton 吗？**  
A: 不可以。会抛出 `InvalidOperationException`，请只保留一个标记。

**Q: 和手写的 `services.AddScoped<IFoo, Foo>()` 冲突吗？**  
A: 本库使用 `TryAdd`。若已有同服务类型注册，不会覆盖。

**Q: 为什么不扫描 bin 目录下所有 DLL？**  
A: 全盘 `LoadFrom` 容易加载无关或未就绪程序集。默认只扫入口与已加载业务程序集，并支持显式/前缀控制。

**Q: 多业务接口解析到的是同一个实例吗？**  
A: 是。具体类型注册一次，各接口通过工厂转发到同一实现。

---

## 17. 📄 License

本项目采用 [Apache License 2.0](LICENSE) 授权。

Copyright 2025 Rocky Wang

---

## 🤝 贡献

1. Fork 并创建特性分支  
2. 在 `tests/EasyCore.Dependency.Tests` 补充测试  
3. 执行 `dotnet test` 与 `dotnet build EasyCore.Dependency.sln`  
4. 提交 Pull Request  

欢迎 Issue / PR 🚀

仓库地址：<https://github.com/RockyWang0521/EasyCore.Dependency>
