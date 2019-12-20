using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Autofac;
using Autofac.Core;
using Autofac.Core.Resolving;
using Autofac.Extras.AttributeMetadata;
using Autofac.Features.Metadata;

[assembly:InternalsVisibleTo("Tests")]

namespace AdventOfCode
{
    internal class Program
    {
        internal static IContainer Container { get; set; }

        internal static void Main(string[] args)
        {
            Container = CreateContainer();

            var days = Container.Resolve<IEnumerable<Meta<IAocDay, AocMetadata>>>();

            var selectedDays = int.TryParse(args.FirstOrDefault(), out var specificDay)
                ? days.Where(x => x.Metadata.Day == specificDay)
                : days;    

            var orderedDays = selectedDays.OrderBy(x => x.Metadata.Day).ToList();

            var sw = new Stopwatch();
            
            foreach (var day in orderedDays)
            {
                
                Console.Write($"Running Day {day.Metadata.Day} Part 1... ");
                sw.Restart();
                var solution1 = day.Value.SolvePart1();
                sw.Stop();
                Console.WriteLine($"[{sw.Elapsed}] Solution: {solution1}");
                
                Console.Write($"Running Day {day.Metadata.Day} Part 2... ");
                sw.Restart();
                var solution2 = day.Value.SolvePart2();
                sw.Stop();
                Console.WriteLine($"[{sw.Elapsed}] Solution: {solution2}");

                Console.WriteLine();
            }
        }

        internal static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<AttributedMetadataModule>();

            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly)
                   .Where(x => x.IsAssignableTo<IAocDay>())
                   .WithParameter(
                        new ResolvedParameter(
                            (pi, ctx) => pi.ParameterType == typeof(ILoader),
                            (pi, ctx) => new Loader((int) ((IInstanceLookup) ctx).ComponentRegistration.Metadata["Day"])
                        )
                    )
                   .AsImplementedInterfaces()
                   .AsSelf();

            builder.RegisterType<Loader>().AsImplementedInterfaces();

            return builder.Build();
        }
    }


}