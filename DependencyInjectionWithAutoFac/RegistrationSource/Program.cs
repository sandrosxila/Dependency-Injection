using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using Autofac.Features.ResolveAnything;

namespace RegistrationSource
{
    public interface ICanSpeak
    {
        void Speak();
    }

    public class Person : ICanSpeak
    {
        public void Speak()
        {
            Console.WriteLine("Hello");
        }
    }

    public abstract class BaseHandler
    {
        public virtual string Handle(string message)
        {
            return $"Handled: {message}";
        }
    }


    public class HandlerA : BaseHandler
    {
        public override string Handle(string message)
        {
            return $"Handled by A: {message}";
        }
    }

    public class HandlerB : BaseHandler
    {
        public override string Handle(string message)
        {
            return $"Handled by B: {message}";
        }
    }


    public interface IHandlerFactory
    {
        T GetHandler<T>() where T : BaseHandler;
    }

    public class HandlerFactory : IHandlerFactory
    {
        public T GetHandler<T>() where T : BaseHandler
        {
            return Activator.CreateInstance<T>();
        }
    }

    public class ConsumerA
    {
        private HandlerA handlerA;

        public ConsumerA(HandlerA handlerA)
        {
            this.handlerA = handlerA ?? throw new ArgumentNullException(nameof(handlerA));
        }

        public void DoWork()
        {
            Console.WriteLine(handlerA.Handle("ConsumerA"));
        }
    }

    public class ConsumerB
    {
        private HandlerB handlerB;

        public ConsumerB(HandlerB handlerB)
        {
            this.handlerB = handlerB ?? throw new ArgumentNullException(nameof(handlerB));
        }

        public void DoWork()
        {
            Console.WriteLine(handlerB.Handle("ConsumerB"));
        }
    }

    public class HandleRegistrationSource : IRegistrationSource
    {
        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service,
            Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            var swt = service as IServiceWithType;
            if (swt == null || !swt.ServiceType.IsAssignableTo<BaseHandler>())
            {
                yield break;
            }

            yield return new ComponentRegistration(
                Guid.NewGuid(),
                new DelegateActivator(swt.ServiceType, ((c, p) =>
                        {
                            var provider = c.Resolve<IHandlerFactory>();
                            var method = provider.GetType().GetMethod("GetHandler")
                                ?.MakeGenericMethod(swt.ServiceType);
                            return method?.Invoke(provider, null);
                        }
                    )),
                new CurrentScopeLifetime(),
                InstanceSharing.None,
                InstanceOwnership.OwnedByLifetimeScope,
                new[] {service},
                new ConcurrentDictionary<string, object>()
            );
        }

        public bool IsAdapterForIndividualComponents => false;
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var cb = new ContainerBuilder();
            // cb.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            cb.RegisterType<HandlerFactory>().As<IHandlerFactory>();
            cb.RegisterSource(new HandleRegistrationSource());
            cb.RegisterType<ConsumerA>();
            cb.RegisterType<ConsumerB>();
            
            using (var c = cb.Build())
            {
                // c.Resolve<Person>().Speak();
                c.Resolve<ConsumerA>().DoWork();
                c.Resolve<ConsumerB>().DoWork();
            }
        }
    }
}