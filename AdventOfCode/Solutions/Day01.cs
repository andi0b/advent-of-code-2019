using System;
using System.Linq;

namespace AdventOfCode.Solutions
{
    [Aoc(1)]
    public class Day01 : IAocDay
    {
        private ILoader _loader;
        public Day01(ILoader loader) => _loader = loader;

        public string SolvePart1() => Solve(CalculateRequiredFuel);
        public string SolvePart2() => Solve(CalculateRequiredFuelIncludingFuelMass);

        private string Solve(Func<int, int> mapperfunc)
        {
            var inputs = _loader.ReadAllLines("Part1.txt").Select(int.Parse);

            var totalFuelRequirement =
                inputs.Select(mapperfunc)
                      .Sum();

            return $"Fuel Requirement: {totalFuelRequirement}";
        }

        internal int CalculateRequiredFuel(int mass) => mass / 3 - 2;
        internal int CalculateRequiredFuelIncludingFuelMass(int mass)
        {
            var requiredFuel = CalculateRequiredFuel(mass);
            if (requiredFuel <= 0) return 0;

            var additionalFuel = CalculateRequiredFuelIncludingFuelMass(requiredFuel);
            return requiredFuel + additionalFuel;
        }
    }
}