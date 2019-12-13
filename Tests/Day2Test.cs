using AdventOfCode;
using AdventOfCode.Solutions;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class Day2Test
    {
        private readonly Day2 _solution = Program.CreateContainer().Resolve<Day2>();

        private const string Part1Example = "1,9,10,3,2,3,11,0,99,30,40,50";

        [Theory,
         InlineData(Part1Example, 0, new[] {1, 9, 10, 3}),
         InlineData(Part1Example, 1, new[] {2, 3, 11, 0}),
         InlineData(Part1Example, 2, new[] {99,}),
         InlineData(Part1Example, 3, new[] {30, 40, 50}),
        ]
        public void GetStatementById(string input, int statementId, int[] expected)
            => new Day2.IntcodeProgram(input).GetStatement(statementId).Should().BeEquivalentTo(expected);

        [Theory,
         InlineData(Part1Example, 3500),
         InlineData("1,0,0,0,99", 2),
         InlineData("2,3,0,3,99", 2),
         InlineData("2,4,4,5,99,0", 2),
         InlineData("1,1,1,4,99,5,6,0,99", 30),
        ]
        public void RunToEnd(string input, int expected)
            => new Day2.IntcodeComputer(new Day2.IntcodeProgram(input)).RunToEnd().Should().Be(expected);
    }
}