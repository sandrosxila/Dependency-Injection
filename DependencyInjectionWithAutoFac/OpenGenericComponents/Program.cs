using System;
using System.Collections.Generic;
using Autofac;

namespace OpenGenericComponents
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(List<>)).As(typeof(IList<>));
            IContainer container = builder.Build();
            var myList = container.Resolve<IList<int>>();
            Console.WriteLine(myList.GetType());
        }
    }
}