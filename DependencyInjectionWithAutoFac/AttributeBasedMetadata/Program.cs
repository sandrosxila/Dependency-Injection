using System;
using System.ComponentModel.Composition;
using Autofac;
using Autofac.Extras.AttributeMetadata;
using Autofac.Features.AttributeFilters;

namespace AttributeBasedMetadata
{
    [MetadataAttribute]
    public class AgeMetadataAttribute : Attribute
    {
        public int Age { get; set; }

        public AgeMetadataAttribute(int age)
        {
            Age = age;
        }
    }

    public interface IArtwork
    {
        void Display();
    }

    [AgeMetadata(100)]
    public class CenturyArtwork : IArtwork
    {
        public void Display()
        {
            Console.WriteLine("Displaying a century-old piece");
        }
    }
    
    [AgeMetadata(1000)]
    public class MillennialArtwork : IArtwork
    {
        public void Display()
        {
            Console.WriteLine("Displaying a millennial-old piece");
        }
    }

    public class ArtDisplay
    {
        private IArtwork artwork;

        public ArtDisplay([MetadataFilter("Age",100)]IArtwork artwork)
        {
            this.artwork = artwork;
        }

        public void Display()
        {
            artwork.Display();
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var b = new ContainerBuilder();
            b.RegisterModule<AttributedMetadataModule>();
            b.RegisterType<CenturyArtwork>().As<IArtwork>();
            b.RegisterType<MillennialArtwork>().As<IArtwork>();
            b.RegisterType<ArtDisplay>().WithAttributeFiltering();
            using (var c = b.Build())
            {
                c.Resolve<ArtDisplay>().Display(); 
            }
        }
    }
}