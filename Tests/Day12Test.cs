using AdventOfCode;
using AdventOfCode.Solutions;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class Day12Test
    {
            private readonly Day12 _solution = Program.CreateContainer().Resolve<Day12>();

            [Fact]
            public void Part2()
            {
                var input = new[]
                {
                    "<x=-8, y=-10, z=0>",
                    "<x=5, y=5, z=10>",
                    "<x=2, y=-7, z=3>",
                    "<x=9, y=-8, z=-3>"
                };

                _solution.Load(input);
                
                _solution.SolvePart2().Should().Be("first repeating iteration at 4686774924");
            }
    }
}