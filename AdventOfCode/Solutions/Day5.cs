using AdventOfCode.Solutions.Intcode;

namespace AdventOfCode.Solutions
{
    [Aoc(5)]
    public class Day5 : IAocDay
    {
        private string _programCode;

        public Day5 (ILoader loader) => _programCode = loader.ReadAllText("Input.txt");

        private string Solve(int input)
        {
            var program = new IntcodeProgram(_programCode);
            var computer = new IntcodeComputer(program);
            computer.Inputs.Enqueue(input);

            computer.Run();

            var formattedOutputs = string.Join(",", computer.Outputs.ToArray());
            return $"All outputs: [{formattedOutputs}]";
        }

        public string SolvePart1() => Solve(1);
        public string SolvePart2() => Solve(5);
    }
}