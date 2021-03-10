using System;
using Autofac;
using Autofac.Features.Indexed;
using Autofac.Features.Metadata;

namespace KeyedServiceLookup
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

    public class Settings
    {
        public string LogMode { get; set; }
    }

    public class Reporting
    {
        private IIndex<string, ILog> logs;

        public Reporting(IIndex<string, ILog> logs)
        {
            this.logs = logs ?? throw new ArgumentNullException(nameof(logs));
        }

        public void Report()
        {
            logs["sms"].Write("Starting Report Output");
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleLog>().Keyed<ILog>("cmd");
            builder.Register(c => new SMSLog("+123456")).Keyed<ILog>("sms");
            builder.RegisterType<Reporting>();
            using (var c = builder.Build())
            {
                c.Resolve<Reporting>().Report();
            }
        }
    }
}