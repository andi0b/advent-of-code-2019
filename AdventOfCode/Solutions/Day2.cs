using System;
using System.Linq;

namespace AdventOfCode.Solutions
{
    [Aoc(2)]
    public class Day2 : IAocDay
    {
        private ILoader _loader;
        public Day2(ILoader loader) => _loader = loader;

        public string SolvePart1()
        {
            var intcodeProgram = new IntcodeProgram(_loader.ReadAllText("Part1.txt"), 12, 2);
            return new IntcodeComputer(intcodeProgram).RunToEnd().ToString();
        }

        public string SolvePart2()
        {
            var input = _loader.ReadAllText("Part1.txt");
            for(var noun = 0; noun <= 99; noun++)
            for (var verb = 0; verb <= 99; verb++)
            {
                var program = new IntcodeProgram(input, noun, verb);
                var result = new IntcodeComputer(program).RunToEnd();
                if (result == 19690720)
                    return $"noun={noun}, verb={verb}, output={100 * noun + verb}";
            }

            return null;
        }

        public class IntcodeComputer
        {
            private readonly IntcodeProgram _program;
            public IntcodeComputer(IntcodeProgram program) =>_program = program;

            public int RunToEnd()
            {
                for (var statementId = 0;; statementId++)
                {
                    var statement = _program.GetStatement(statementId);

                    if (statement[0] == 99) break;

                    _program.Memory[statement[3]] = statement[0] switch
                    {
                        1 => _program.Memory[statement[1]] + _program.Memory[statement[2]],
                        2 => _program.Memory[statement[1]] * _program.Memory[statement[2]]
                    };
                }

                return _program.Memory[0];
            }
        }
        
        public class IntcodeProgram
        {
            public int[] Memory { get; }

            public IntcodeProgram(string input, int noun = -1, int verb = -1)
            {
                Memory = input.Split(',').Select(int.Parse).ToArray();

                if (noun >= 0) Memory[1] = noun;
                if (verb >= 0) Memory[2] = verb;
            }

            public override string ToString() => string.Join(",", Memory);

            public int[] GetStatement(int statementId)
            {
                var pos = 0;
                for (var curStatementId = 0; curStatementId < Memory.Length; curStatementId++)
                {
                    var op = Memory[pos];
                    var oplen = op switch
                    {
                        99 => 1,
                        _ => 4
                    };

                    if (curStatementId == statementId)
                        return Memory[pos .. (Math.Min(pos + oplen, Memory.Length))];

                    pos += oplen;
                }
                
                return new int[0];
            }
        }
    }
}