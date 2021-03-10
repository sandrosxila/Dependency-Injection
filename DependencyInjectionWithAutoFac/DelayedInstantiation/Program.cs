using System;
using Autofac;

namespace DelayedInstantiation
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
        private Lazy<ConsoleLog> log;

        public Reporting(Lazy<ConsoleLog> log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            Console.WriteLine("Reported component created");
        }

        public void Report()
        {
            log.Value.Write("Log started");
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>();
            builder.RegisterType<Reporting>();
            using (var c = builder.Build())
            {
                c.Resolve<Reporting>().Report();
            }
        }
    }
}