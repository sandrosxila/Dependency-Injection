using System;
using System.Collections.Generic;
using Autofac;

namespace Enumeration
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
        private IList<ILog> allLogs;

        public Reporting(IList<ILog> allLogs)
        {
            this.allLogs = allLogs;
        }

        public void Report()
        {
            foreach (var log in allLogs)
            {
                log.Write($"Hello this is {log.GetType().Name}");
            }
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>().As<ILog>();
            builder.Register(c => new SMSLog("+1234756")).As<ILog>();
            builder.RegisterType<Reporting>();
            using (var c = builder.Build())
            {
                c.Resolve<Reporting>().Report();
            }
        }
    }
}