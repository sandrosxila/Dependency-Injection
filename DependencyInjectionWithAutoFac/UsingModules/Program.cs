using System;
using Autofac;

namespace UsingModules
{

    public interface IVehicle
    {
        void Go();
    }

    public interface IDriver
    {
        void Drive();
    }
    
    public class CrazyDriver : IDriver
    {
        public void Drive()
        {
            Console.WriteLine("Going too fast and crashing into a tree");
        }
    }

    public class SaneDriver : IDriver
    {
        public void Drive()
        {
            Console.WriteLine($"Driving Safely to the destination");
        }
    }

    class Truck : IVehicle
    {
        private IDriver driver;

        public Truck(IDriver driver)
        {
            this.driver = driver;
        }
        public void Go()
        {
            driver.Drive();
        }
    }

    public class TransportModule : Module
    {
        public bool ObeySpeedLimit { get; set; }
        protected override void Load(ContainerBuilder builder)
        {
            if (ObeySpeedLimit)
            {
                builder.RegisterType<SaneDriver>().As<IDriver>();
            }
            else
            {
                builder.RegisterType<CrazyDriver>().As<IDriver>();
            }

            builder.RegisterType<Truck>().As<IVehicle>();
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new TransportModule(){ObeySpeedLimit = true});
            using (var c = builder.Build())
            {
                c.Resolve<IVehicle>().Go();
            }
        }
    }
}