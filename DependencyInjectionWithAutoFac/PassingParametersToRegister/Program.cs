using System;
using System.Reflection;
using Autofac;
using Autofac.Core;

namespace PassingParametersToRegister
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
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // named parameter
//      builder.RegisterType<SMSLog>()
//        .As<ILog>()
//        .WithParameter("phoneNumber", "+12345678");

            // typed parameter
//      builder.RegisterType<SMSLog>()
//        .As<ILog>()
//        .WithParameter(new TypedParameter(typeof(string), "+12345678"));

            // resolved parameter
//      builder.RegisterType<SMSLog>()
//        .As<ILog>()
//        .WithParameter(
//          new ResolvedParameter(
//            // predicate
//            (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
//            // value accessor
//            (pi, ctx) => "+12345678"
//          )
//        );
//
//
            Random random = new Random();
            builder.Register((c, p) => new SMSLog(p.Named<string>("phoneNumber")))
                .As<ILog>();

            Console.WriteLine("About to build container...");
            var container = builder.Build();

            var log = container.Resolve<ILog>(new NamedParameter("phoneNumber", random.Next().ToString()));
            log.Write("Testing");
        }
    }
}