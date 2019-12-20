using AdventOfCode;
using AdventOfCode.Solutions;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class Day01Tests
    {
        private readonly Day01 _solution = Program.CreateContainer().Resolve<Day01>();

        [
            Theory,
            InlineData(12, 2),
            InlineData(14, 2),
            InlineData(1969, 654),
            InlineData(100756, 33583)
        ]
        public void CalculateRequiredFuel(int mass, int expected)
            => _solution.CalculateRequiredFuel(mass).Should().Be(expected);

        [
            Theory,
            InlineData(14, 2),
            InlineData(1969, 966),
            InlineData(100756, 50346)
        ]
        public void CalculateRequiredFuelIncludingFuelMass(int mass, int expected)
            => _solution.CalculateRequiredFuelIncludingFuelMass(mass).Should().Be(expected);
    }
}