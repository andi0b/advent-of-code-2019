﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Solutions.Intcode;

namespace AdventOfCode.Solutions
{
    [Aoc(10)]
    public class Day10 : IAocDay
    {
        public string Input { get; set; }

        public Day10(ILoader loader) => Input = loader.ReadAllText("Input.txt");


        public string SolvePart1()
        {
            var program = new IntcodeProgram(Input);
            var computer = new IntcodeComputer(program, 1);
            
            computer.Run();

            return computer.Outputs.Single().ToString();
        }

        public string SolvePart2()
        {
            var program = new IntcodeProgram(Input);
            var computer = new IntcodeComputer(program, 2);

            computer.Run();

            return computer.Outputs.Single().ToString();
        }
    }
}