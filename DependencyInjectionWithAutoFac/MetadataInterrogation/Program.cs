using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.Metadata;

namespace MetadataInterrogation
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
        private Meta<ConsoleLog, Settings> log;

        public Reporting(Meta<ConsoleLog, Settings> log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public void Report()
        {
            log.Value.Write("Starting Report");
            // if(log.Metadata["mode"] as string == "verbose")
            if (log.Metadata.LogMode == "verbose")
                log.Value.Write($"Verbose mode: Logger started on {DateTime.Now}");
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            // builder.RegisterType<ConsoleLog>().WithMetadata("mode","verbose");
            builder.RegisterType<ConsoleLog>().
                WithMetadata<Settings>(c => c.For(x => x.LogMode, "verbose"));
            builder.RegisterType<Reporting>();
            using (var c = builder.Build())
            {
                c.Resolve<Reporting>().Report();
            }
        }
    }
}