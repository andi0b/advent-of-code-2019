using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solutions.Intcode;

namespace AdventOfCode.Solutions
{
    [Aoc(7)]
    public class Day07 : IAocDay
    {
        public string Input { get; set; }

        public Day07(ILoader loader) => Input = loader.ReadAllText("Input.txt");

        public string SolvePart1() => $"highest signal that can be sent to the thrusters: {SolvePart1Internal()}";
        public string SolvePart2() => $"highest signal that can be sent to the thrusters {SolvePart2Internal()}";
        public int SolvePart1Internal() => Solve(new[] {0, 1, 2, 3, 4});
        public int SolvePart2Internal() => Solve(new[] {5, 6, 7, 8, 9});
        
        public int Solve(int[] possiblePhaseSettings)
        {
            var allSettigs =
                from p1 in possiblePhaseSettings
                from p2 in possiblePhaseSettings
                from p3 in possiblePhaseSettings
                from p4 in possiblePhaseSettings
                from p5 in possiblePhaseSettings
                let set = new[] {p1, p2, p3, p4, p5}
                where set.Distinct().Count() == 5
                select set;

            var allCalculations =
                allSettigs.Select(x => AmplifChainedLooped(x))
                          .ToList();

            return allCalculations.Max();
        }

        public int AmplifChainedLooped(int[] phaseSettings, int inputSignal = 0)
        {
            var computers = (
                from ps in phaseSettings
                let program = new IntcodeProgram(Input)
                let computer = new IntcodeComputer(program, ps)
                select computer
            ).ToList();
            

            while (computers.All(x => x.State != State.Halted))
            {
                foreach (var computer in computers)
                {
                    computer.Inputs.Enqueue(inputSignal);
                    computer.Run();
                    inputSignal = (int)computer.Outputs.Dequeue();
                }
            }

            return inputSignal;
        }
    }
}