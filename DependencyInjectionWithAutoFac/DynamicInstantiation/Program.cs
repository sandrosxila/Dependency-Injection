using System;
using Autofac;
using Autofac.Features.OwnedInstances;

namespace DynamicInstantiation
{
    public interface ILog : IDisposable
    {
        void Write(string message);
    }

    public class ConsoleLog : ILog
    {
        public void Dispose()
        {
            Console.WriteLine("Console logger no longer required");
        }

        public ConsoleLog()
        {
            Console.WriteLine($"Console log created {DateTime.Now.Ticks}");
        }

        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
    public class SMSLog : ILog
    {
        private readonly string phoneNumber;

        public SMSLog(string phoneNumber)
        {
            this.phoneNumber = phoneNumber;
        }
        public void Dispose()
        {
            
        }

        public void Write(string message)
        {
            Console.WriteLine($"Sms to {phoneNumber}: {message}");
        }
    }

    public class Reporting
    {
        private Func<ConsoleLog> consoleLog;

        public Reporting(Func<ConsoleLog> consoleLog)
        {
            this.consoleLog = consoleLog ?? throw new ArgumentNullException(nameof(consoleLog));
        }

        public void Report()
        {
            consoleLog().Write("Reporting to Console");
            consoleLog().Write("And Again");
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>();
            builder.RegisterType<SMSLog>();
            builder.RegisterType<Reporting>();
            using (var c = builder.Build())
            {
                c.Resolve<Reporting>().Report();
                Console.WriteLine("Done Reporting");
            }
        }
    }
}