using System;
using Autofac;

namespace CircularDependencies
{
    public class ParentWithProperty
    {
        public ChildWithProperty Child { get; set; }

        public override string ToString()
        {
            return "Parent";
        }
    }

    public class ChildWithProperty
    {
        public ParentWithProperty Parent { get; set; }

        public override string ToString()
        {
            return "Child";
        }
    }

    public class ParentWithConstructor1
    {
        public ChildWithProperty1 Child { get; set; }

        public ParentWithConstructor1(ChildWithProperty1 child)
        {
            Child = child;
        }

        public override string ToString()
        {
            return "Parent with a ChildWithProperty";
        }
    }

    public class ChildWithProperty1
    {
        public ParentWithConstructor1 Parent { get; set; }

        public override string ToString()
        {
            return "Child";
        }
    }

    internal class Program
    {
        public static void Main_(string[] args)
        {
            var b = new ContainerBuilder();
            b.RegisterType<ParentWithProperty>().InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            b.RegisterType<ChildWithProperty>().InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            using (var c = b.Build())
            {
                Console.WriteLine(c.Resolve<ParentWithProperty>().Child);
            }
        }

        static void Main(string[] args)
        {
            var b = new ContainerBuilder();
            b.RegisterType<ParentWithConstructor1>().InstancePerLifetimeScope();
            b.RegisterType<ChildWithProperty1>().InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            using (var c = b.Build())
            {
                Console.WriteLine(c.Resolve<ParentWithConstructor1>().Child.Parent);
            }
        }
    }
}