# EasyCore.Dependencie

快速IOC依赖注入类库，基于.net8.0。

使用说明：

1.Program注册

```
  public class Program
  {
      public static void Main(string[] args)
      {
          var builder = WebApplication.CreateBuilder(args);


          builder.Services.AddControllers();
          builder.Services.AddEndpointsApiExplorer();
          builder.Services.AddSwaggerGen();

          // Use EasyCoreDependencie
          builder.Services.EasyCoreDependencie();

          var app = builder.Build();

          if (app.Environment.IsDevelopment())
          {
              app.UseSwagger();
              app.UseSwaggerUI();
          }

          app.UseAuthorization();


          app.MapControllers();

          app.Run();
      }
  }
```
2.使用EasyCore.Dependencie

2.1 EasyCore.Dependencie提供了三个接口 IScopedDependencie，ISingletonDependencie，ITransientDependencie对应Scoped，Singleton，Transient生命周期。

2.2 使用时接口继承

接口抽象
```
    public interface IScopedTest : IScopedDependencie
    {
        string Test();
    }

    public interface ISingletonTest : ISingletonDependencie
    {
        string Test();
    }

    public interface ITransientTest : ITransientDependencie
    {
        string Test();
    }
```
细节实现：

```
    public class ScopedTest : IScopedTest
    {
        public string Test()
        {
            return "This is a Scoped test";
        }
    }

    public class SingletonTest : ISingletonTest
    {
        public string Test()
        {
            return "This is a Singleton test";
        }
    }

   public class TransientTest : ITransientTest 
   {
       public string Test()
       {
           return "This is a Transient test";
       }
   }
```
就可以自动实现抽象与细节之间的依赖关系注入。

