using System;
using System.Linq;
using AdventOfCode;
using AdventOfCode.Solutions;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class Day10Test
    {
        private readonly Day10 _solution = Program.CreateContainer().Resolve<Day10>();

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 1, 1.4142, 0.25)]
        [InlineData(0, 1, 1, 0.5)]
        [InlineData(1, 0, 1, 0)]
        [InlineData(-1, 0, 1, 1)]
        [InlineData(0, -1, 0, 1.5)]
        [InlineData(-1, -1, 1.4142, 1.25)]
        public void Check(int x, int y, double rad, double ang)
        {
            var polar = _solution.CalculatePolar((x, y));

            polar.rad.Should().BeApproximately(rad, 0.001);
            polar.ang.Should().BeApproximately(ang * Math.PI, 0.001);
        }

        [Theory]
        [InlineData(new[]
        {
            @".#..#",
            ".....",
            "#####",
            "....#",
            "...##"
        }, "selected asteroid: 3,4 visible count: 8")]
        public void Part1(string[] input, string result)
        {
            _solution.Load(input);
            _solution.SolvePart1().Should().Be(result);
        }

        [Theory]
        [InlineData(new[]
        {
            ".#....#####...#..",
            "##...##.#####..##",
            "##...#...#.#####.",
            "..#.....X...###..",
            "..#.#.....#....##"
        },"")]
        public void VaporizeInOrder(string[] input, string result)
        {
            _solution.Load(input);
            var station = (8, 3);

            var vaporizedInorder = _solution.VaporizeAstroids(station, _solution.Asteroids.Except(new[] {station}).ToList());

            vaporizedInorder.Should().BeEquivalentTo(new[]
            {
                //   1,      2,      3,       4,      5,       6,      7,       8,        9,
                (8, 1), (9, 0), (9, 1), (10, 0), (9, 2), (11, 1), (12, 1), (11, 2), (15, 1),

                //    1,       2,       3,       4,       5,       6,       7,       8,      9,
                (12, 2), (13, 2), (14, 2), (15, 2), (12, 3), (16, 4), (15, 4), (10, 4), (4, 4),

                //    1,     2,       3,     4,      5,      6,      7,      8,     9,
                (2, 4), (2, 3), (0, 2), (1, 2), (0, 1), (1, 1), (5, 2), (1, 0), (5, 1),

                (6, 1), (6, 0), (7, 0), (8, 0), (10, 1), (14, 0), (16, 1), (13, 3), (14, 3),
            });

        }

        [Fact]
        public void TurnAngleNegative()
            => _solution.TurnAngle(Math.PI / 4, -90).Should().Be(Math.PI * 2 - Math.PI / 4);

        
        [Theory]
        [InlineData(new[]
        {
            ".#..##.###...#######",
            "##.############..##.",
            ".#.######.########.#",
            ".###.#######.####.#.",
            "#####.##.#.##.###.##",
            "..#####..#.#########",
            "####################",
            "#.####....###.#.#.##",
            "##.#################",
            "#####.##.###..####..",
            "..######..##.#######",
            "####.##.####...##..#",
            ".#####..#.######.###",
            "##...#.##########...",
            "#.##########.#######",
            ".####.#.###.###.#.##",
            "....##.##.###..#####",
            ".#.#.###########.###",
            "#.#.#.#####.####.###",
            "###.##.####.##.#..##"
        }, "The 200th asteroid to be vaporized is at 8,2 (solution is 802)")]
        public void Part2(string[] input, string result)
        {
            _solution.Load(input);
            _solution.SolvePart2().Should().Be(result);
        }


    }
}