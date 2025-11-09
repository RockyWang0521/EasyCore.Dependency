# EasyCore.Dependency

基于标记接口的 .NET 自动依赖注入库，面向 .NET 8。

通过实现 `IScopedDependency` / `ISingletonDependency` / `ITransientDependency`，即可在启动时自动完成服务注册，无需手写大量 `AddScoped` / `AddSingleton` / `AddTransient`。

## 特性

- 三种标准生命周期标记接口
- 支持「业务接口 → 实现类」与「具体类型自身」两种注册方式
- 同一实现类实现多个业务接口时，全部注册
- 可指定扫描程序集或按程序集名前缀过滤
- 使用 `TryAdd`，重复调用安全
- 不再全盘 `LoadFrom` 扫描目录中的 DLL

## 安装

```bash
dotnet add package EasyCore.Dependency
```

或在项目文件中引用：

```xml
<PackageReference Include="EasyCore.Dependency" Version="8.0.0" />
```

## 快速开始

在 `Program.cs` 中注册：

```csharp
using EasyCore.Dependency;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// 扫描入口程序集与已加载的非框架程序集
builder.Services.AddEasyCoreDependency();

var app = builder.Build();
app.MapControllers();
app.Run();
```

## 使用方式

### 1. 接口抽象（推荐）

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

构造函数注入：

```csharp
public class HomeController : ControllerBase
{
    private readonly IUserService _userService;

    public HomeController(IUserService userService)
    {
        _userService = userService;
    }
}
```

三种生命周期：

| 标记接口 | DI 生命周期 |
|----------|-------------|
| `IScopedDependency` | Scoped |
| `ISingletonDependency` | Singleton |
| `ITransientDependency` | Transient |

### 2. 具体类直接注册

实现类直接继承标记接口时，按具体类型注册：

```csharp
public class CacheHelper : ISingletonDependency
{
    public string Get(string key) => key;
}

// 注入
public class DemoController(CacheHelper cacheHelper) : ControllerBase
{
}
```

### 3. 多业务接口

同一实现类实现多个业务接口时，每个接口都会注册：

```csharp
public interface IOrderReader : IScopedDependency { }
public interface IOrderWriter : IScopedDependency { }

public class OrderService : IOrderReader, IOrderWriter
{
}
```

## 高级用法

### 指定程序集

```csharp
builder.Services.AddEasyCoreDependency(typeof(Program).Assembly);
```

### 选项配置

```csharp
builder.Services.AddEasyCoreDependency(options =>
{
    options.AddAssemblies(typeof(Program).Assembly);
    options.AssemblyNamePrefixes.Add("MyApp");
});
```

- `Assemblies`：显式指定要扫描的程序集；为空时使用入口程序集 + 已加载的非 `System.` / `Microsoft.` 程序集
- `AssemblyNamePrefixes`：按程序集名前缀过滤

## 破坏性变更（Dependencie → Dependency）

自 `8.0.0` 起，包名与 API 全面更正拼写：

| 旧 | 新 |
|----|----|
| `EasyCore.Dependencie` | `EasyCore.Dependency` |
| `IScopedDependencie` 等 | `IScopedDependency` 等 |
| `services.EasyCoreDependencie()` | `services.AddEasyCoreDependency()` |

升级步骤：更新 NuGet 包引用，全局替换命名空间与接口名，并将扩展方法改为 `AddEasyCoreDependency`。

## 示例与测试

- 可运行示例：`demo/Web.Dependency`
- 单元测试：`tests/EasyCore.Dependency.Tests`

```bash
dotnet test EasyCore.Dependency.sln
```

## 许可证

本项目采用 [Apache License 2.0](LICENSE) 授权。

Copyright 2025 Rocky Wang
