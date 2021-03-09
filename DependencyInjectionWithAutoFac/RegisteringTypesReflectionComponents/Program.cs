using System;
using Autofac;

namespace RegisteringTypesReflectionComponents
{
    public interface ILog
    {
        void Write(string message);
    }
    
    public class ConsoleLog : ILog
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class Engine
    {
        private ILog log;
        private int id;
        
        public Engine(ILog log)
        {
            this.log = log;
            id = new Random().Next();
        }

        public void Ahead(int power)
        {
            log.Write($"Engine [{id}] ahead {power}");
        }
    }

    public class Car
    {
        private ILog log;
        private Engine engine;

        public Car(Engine engine, ILog log)
        {
            this.log = log;
            this.engine = engine;
        }

        public void Go()
        {
            engine.Ahead(100);
            log.Write($"Car is going forward");
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            // instead of this
            // var log = new ConsoleLog();
            // var engine = new Engine(log);
            // var car = new Car(engine, log);
            // we will do that
            var builder = new ContainerBuilder();
            // adding AsSelf at the end in order to get the ability of creation of ConsoleLog instance
            // not only the ILog interface instance
            builder.RegisterType<ConsoleLog>().As<ILog>().AsSelf();
            builder.RegisterType<Engine>();
            builder.RegisterType<Car>();

            IContainer container = builder.Build();

            // although we don't need this instance
            // we just created it to show that using AsSelf
            // give us ability to create ConsoleLog instance
            var log = container.Resolve<ConsoleLog>();

            // we don't need to create all dependency objects and
            // passing ConsoleLog object to Car and Engine instances is not necessary
            
            var car = container.Resolve<Car>(); 
            car.Go();
        }
    }
}