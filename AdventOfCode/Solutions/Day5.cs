using AdventOfCode.Solutions.Intcode;

namespace AdventOfCode.Solutions
{
    [Aoc(5)]
    public class Day5 : IAocDay
    {
        private string _programCode;

        public Day5 (ILoader loader) => _programCode = loader.ReadAllText("Input.txt");
        
        public string SolvePart1()
        {
            var program = new IntcodeProgram(_programCode);
            var computer = new IntcodeComputer(program);
            computer.Inputs.Push(1);

            computer.RunToEnd();

            var formattedOutputs = string.Join(",", computer.Outputs.ToArray());
            return $"All outputs: [{formattedOutputs}]";
        }

        public string SolvePart2()
        {
            throw new System.NotImplementedException();
        }
    }
}