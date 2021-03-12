using System;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;

namespace TypeInterceptors
{
    public class CallLogger : IInterceptor
    {
        private TextWriter output;

        public CallLogger(TextWriter output)
        {
            this.output = output;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;
            output.WriteLine("Calling Method {0} with args {1}",
                methodName,
                string.Join(",",
                    invocation.Arguments.Select((a => (a ?? " ").ToString()
                        )
                    )
                )
            );
            invocation.Proceed();
            output.WriteLine("Done calling {0}, result was {1}", methodName, invocation.ReturnValue);
        }
    }


    public interface IAudit
    {
        int Start(DateTime reportDate);
    }
    
    [Intercept(typeof(CallLogger))]
    public class Audit : IAudit
    {
        public virtual int Start(DateTime reportDate)
        {
            Console.WriteLine($"Starting report on {reportDate}");
            return 42;
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            cb.Register(c => new CallLogger(Console.Out)).As<IInterceptor>().AsSelf();
            cb.RegisterType<Audit>().EnableClassInterceptors();
            using (var container = cb.Build())
            {
                var audit = container.Resolve<Audit>();
                audit.Start(DateTime.Now);
            }
        }
    }
}