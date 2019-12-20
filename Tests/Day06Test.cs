using AdventOfCode;
using AdventOfCode.Solutions;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class Day06Test
    {
        private readonly Day06 _solution = Program.CreateContainer().Resolve<Day06>();

        [Fact]
        public void Part1Example()
        {
            var example = new[]
            {
                "COM)B",
                "B)C",
                "C)D",
                "D)E",
                "E)F",
                "B)G",
                "G)H",
                "D)I",
                "E)J",
                "J)K",
                "K)L",
            };
            
            _solution.LoadInput(example);

            _solution.SolvePart1Internal().Should().Be(42);
        }

        [Fact]
        public void Part2Example()
        {
            var example = new[]
            {
                "COM)B",
                "B)C",
                "C)D",
                "D)E",
                "E)F",
                "B)G",
                "G)H",
                "D)I",
                "E)J",
                "J)K",
                "K)L",
                "K)YOU",
                "I)SAN",
            };

            _solution.LoadInput(example);
            
            _solution.SolvePart2Internal().Should().Be(4);
        }
    }
}