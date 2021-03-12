using System;
using Autofac;
using Autofac.Extras.AggregateService;

namespace AggregateServices
{
    
    public interface IService1{}
    public interface IService2{}
    public interface IService3{}
    public interface IService4{}
    
    public class Class1 : IService1{}
    public class Class2: IService2 {}
    public class Class3: IService3{}

    public class Class4 : IService4
    {
        private string name;

        public Class4(string name)
        {
            this.name = name;
        }
    }

    public interface IMyAggregateService
    {
        IService1 service1 { get; set; }
        IService2 service2{ get; set; }
        IService3 service3{ get; set; }
        // IService4 service4{ get; set; }
        IService4 GetFourthService(string name);
    }
    
    
    public class Consumer
    {
        public IMyAggregateService AllServices { get; set; }

        public Consumer(IMyAggregateService allServices)
        {
            AllServices = allServices;
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            cb.RegisterAggregateService<IMyAggregateService>();
            cb.RegisterAssemblyTypes(typeof(Program).Assembly).Where(t => t.Name.StartsWith("Class"))
                .AsImplementedInterfaces();
            cb.RegisterType<Consumer>();
            using (var container = cb.Build())
            {
                var consumer = container.Resolve<Consumer>();
                Console.WriteLine(consumer.AllServices.service3.GetType().Name);
                Console.WriteLine(consumer.AllServices.GetFourthService("test").GetType().Name);
            }
        }
    }
}