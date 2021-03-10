using System;
using Autofac;
using Autofac.Features.Indexed;

namespace InstanceScope
{
    public interface ILog
    {
        void Write(string message);
    }

    public interface IConsole
    {
    }

    public class ConsoleLog : ILog
    {
        public ConsoleLog()
        {
            Console.WriteLine($"Creating ConsoleLog");
        }

        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class EmailLog : ILog, IConsole
    {
        private const string adminEmail = "admin@foo.com";

        public void Write(string message)
        {
            Console.WriteLine($"Email sent to {adminEmail} : {message}");
        }
    }

    class SMSLog : ILog
    {
        private string phoneNumber;

        public SMSLog(string phoneNumber)
        {
            this.phoneNumber = phoneNumber;
        }

        public void Write(string message)
        {
            Console.WriteLine($"SMS to {phoneNumber} : {message}");
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

        public Engine(ILog log, int id)
        {
            this.log = log;
            this.id = id;
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

        public Car(Engine engine)
        {
            this.engine = engine;
            this.log = new EmailLog();
        }

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

    public class Parent
    {
        public override string ToString()
        {
            return "I am your father";
        }
    }

    public class Child
    {
        public string Name { get; set; }
        public Parent Parent { get; set; }

        public void SetParent(Parent parent)
        {
            Parent = parent;
        }
    }

    public class ParentChildModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Parent>();
            builder.Register(c => new Child() {Parent = c.Resolve<Parent>()});
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            // builder.RegisterType<ConsoleLog>().InstancePerLifetimeScope();
            builder.RegisterType<ConsoleLog>().InstancePerMatchingLifetimeScope("foo");

            var container = builder.Build();

            using (var scope1 = container.BeginLifetimeScope("foo"))
            {
                for (int i = 0; i < 3; i++)
                {
                    scope1.Resolve<ConsoleLog>();
                }
                
                using (var scope2 = scope1.BeginLifetimeScope())
                {
                    for (int i = 0; i < 3; i++)
                    {
                        scope2.Resolve<ConsoleLog>();
                    }
                }
            }

            // using (var scope2 = container.BeginLifetimeScope())
            // {
            //     for (int i = 0; i < 3; i++)
            //     {
            //         scope2.Resolve<ConsoleLog>();
            //     }
            // }
        }
    }
}