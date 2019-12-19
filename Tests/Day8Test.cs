﻿using AdventOfCode;
using AdventOfCode.Solutions;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class Day8Test
    {
        private readonly Day8 _solution = Program.CreateContainer().Resolve<Day8>();

        [Fact]
        public void FillLayers()
        {
            var layers = _solution.FillLayers("123456789012", 3, 2);

            layers.Should().BeEquivalentTo(new[]
            {
                new[,]
                {
                    {1, 2, 3},
                    {4, 5, 6},
                },

                new[,]
                {
                    {7, 8, 9},
                    {0, 1, 2},
                }
            });
        }
    }
}