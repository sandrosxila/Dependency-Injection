using System;
using System.Collections.Generic;
using Autofac;

namespace CaptiveDependencies
{
    public interface IResource
    {
        
    }

    class SingletonResource : IResource
    {
    }

    public class InstancePerDependencyResource : IResource, IDisposable
    {
        public InstancePerDependencyResource()
        {
            Console.WriteLine($"Instance Per Dependency Created");
        }

        public void Dispose()
        {
            Console.WriteLine("Instance Per Dependency is Destroyed");
        }
    }

    public class ResourceManager
    {
        public ResourceManager(IEnumerable<IResource> resources)
        {
            Resources = resources;
        }
        public IEnumerable<IResource> Resources { get; set; }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ResourceManager>().SingleInstance();
            builder.RegisterType<SingletonResource>().As<IResource>().SingleInstance();
            builder.RegisterType<InstancePerDependencyResource>().As<IResource>().InstancePerDependency();

            using (var container = builder.Build())
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    scope.Resolve<ResourceManager>();
                }
            }
        }
    }
}