using System;
using Autofac;

namespace Decorators
{
    public interface IReportingService
    {
        void Report();
    }

    public class ReportingService : IReportingService
    {
        public void Report()
        {
            Console.WriteLine("Here is your Report");
        }
    }

    public class ReportingServiceWithLogging : IReportingService
    {
        private IReportingService decorated;

        public ReportingServiceWithLogging(IReportingService decorated)
        {
            this.decorated = decorated;
        }

        public void Report()
        {
            Console.WriteLine("Commenting Log...");
            decorated.Report();
            Console.WriteLine("Ending Log...");
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var b = new ContainerBuilder();
            b.RegisterType<ReportingService>().Named<IReportingService>("reporting");
            b.RegisterDecorator<IReportingService>(
                (context, service) => new ReportingServiceWithLogging(service),
                "reporting"
            );

            using (var c = b.Build())
            {
                var r = c.Resolve<IReportingService>();
                r.Report();
            }
        }
    }
}