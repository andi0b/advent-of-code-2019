using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AdventOfCode.Solutions.Intcode
{
    public class IntcodeComputer
    {
        private readonly IntcodeProgram _program;

        public IntcodeComputer(IntcodeProgram program, params int[] parameters)
        {
            _program = program;
            Inputs = new Queue<long>(parameters.Select(x => (long) x));
        }

        public State State { get; private set; } = State.NotStarted;
        
        public Queue<long> Inputs { get; }
        public Queue<long> Outputs { get; } = new Queue<long>();

        public int ProgramPosition { get; private set; } = 0;

        public long RelativeOffset { get; private set; } = 0;
        
        public long Run()
        {
            while (true)
            {
                var statementPositions = Enumerable.Range(ProgramPosition, 4);
                var statementRaw = statementPositions
                                  .Select(pos => _program.Memory.TryGetValue(pos, out var val) ? (long?) val : null)
                                  .OfType<long>()
                                  .ToArray();
                
                var statement = Statement.Parse(statementRaw);
                ProgramPosition += statement.Length;

                switch (statement.Operation.Type)
                {
                    case OperationType.Add:
                    case OperationType.Multiply:
                        WriteTo(statement.Parameter3, statement.Operation.Type switch
                        {
                            OperationType.Add => ReadParameter(statement.Parameter1) + ReadParameter(statement.Parameter2),
                            OperationType.Multiply => ReadParameter(statement.Parameter1) * ReadParameter(statement.Parameter2),
                        });
                        break;

                    case OperationType.Output:
                        Outputs.Enqueue(ReadParameter(statement.Parameter1));
                        break;

                    case OperationType.Input:
                        if (Inputs.TryDequeue(out var input))
                            WriteTo(statement.Parameter1, input);
                        else
                        {
                            ProgramPosition -= statement.Length;
                            State = State.WaitingForInput;
                            return 0;
                        }

                        break;

                    case OperationType.JumpIfTrue:
                        if (ReadParameter(statement.Parameter1) != 0)
                            ProgramPosition = (int) ReadParameter(statement.Parameter2);
                        break;

                    case OperationType.JumpIfFalse:
                        if (ReadParameter(statement.Parameter1) == 0)
                            ProgramPosition = (int) ReadParameter(statement.Parameter2);
                        break;

                    case OperationType.LessThan:
                        WriteTo(statement.Parameter3, ReadParameter(statement.Parameter1) < ReadParameter(statement.Parameter2) ? 1 : 0);
                        break;

                    case OperationType.Equals:
                        WriteTo(statement.Parameter3, ReadParameter(statement.Parameter1) == ReadParameter(statement.Parameter2) ? 1 : 0);
                        break;
                    
                    case OperationType.RelativeBaseOffset:
                         RelativeOffset += ReadParameter(statement.Parameter1);
                        break;

                    case OperationType.Halt:
                        State = State.Halted;
                        return _program.Memory[0];
                }
            }
        }

        public void WriteTo(Parameter p, long value)
        {
            switch (p.Mode)
            {
                case ParameterMode.Position:
                    _program.Memory[p.Value] = value;
                    break;
                
                case ParameterMode.Relative:
                    _program.Memory[p.Value+  RelativeOffset] = value;
                    break;
                
                case ParameterMode.Immediate:
                default:
                    throw new NotSupportedException();
            }
        }
        
        public long ReadParameter(Parameter p) => p.Mode switch
        {
            ParameterMode.Immediate => p.Value,
            ParameterMode.Position => ReadValueAt(p.Value),
            ParameterMode.Relative => ReadValueAt(p.Value +  RelativeOffset)
        };

        public long ReadValueAt(long position) =>
            _program.Memory.TryGetValue(position, out var val) ? val : 0;
    }
        
    public class IntcodeProgram
    {
        public Dictionary<long, long> Memory { get; }

        public IntcodeProgram(string input)
        {
            Memory = input.Split(',').Select(long.Parse).ToArray()
                          .Select((x, i) => (x, i))
                          .ToDictionary(t => (long)t.i, t => (long) t.x);
        }

        public override string ToString() => string.Join(",", Memory);
    }

    public class Statement
    {
        public Operation Operation { get; private set; }
        public Parameter Parameter1 { get; private set;}
        public Parameter Parameter2 { get; private set;}
        public Parameter Parameter3 { get; private set;}

        public int Length => Operation.ParameterCount + 1;

        public static Statement Parse(long[] input)
        {
            var operationType = (OperationType) (ReadDigit(0, (int)input[0]) + ReadDigit(1, (int)input[0]) * 10);
            var operation = new Operation(operationType);
            
            var param1Mode = (ParameterMode) ReadDigit(2, (int)input[0]);
            var param2Mode = (ParameterMode) ReadDigit(3, (int)input[0]);
            var param3Mode = (ParameterMode) ReadDigit(4, (int)input[0]);
            
            return new Statement
            {
                Operation = operation,
                Parameter1 = operation.ParameterCount >= 1 ? new Parameter(param1Mode, input[1]) : null,
                Parameter2 = operation.ParameterCount >= 2 ? new Parameter(param2Mode, input[2]) : null,
                Parameter3 = operation.ParameterCount >= 3 ? new Parameter(param3Mode, input[3]) : null,
            };
        }

        private static int ReadDigit(int pos, int num) => num / (int) Math.Pow(10, pos) % 10;

        public override string ToString() => $"{Operation}, P1: {Parameter1}, P2: {Parameter2}, P3: {Parameter3}";
    }

    public class Parameter
    {
        public Parameter(ParameterMode mode, long value)
        {
            Mode = mode;
            Value = value;
        }

        public ParameterMode Mode { get; }
        public long Value { get; }

        public override string ToString() => $"[{Mode}] {Value}";
    }

    public class Operation
    {
        public OperationType Type { get; }
        public Operation(OperationType operationType) => Type = operationType;
        public int ParameterCount => Type switch
        {
            OperationType.Add => 3,
            OperationType.Multiply => 3,
            OperationType.Input => 1,
            OperationType.Output => 1,
            OperationType.JumpIfTrue => 2,
            OperationType.JumpIfFalse => 2,
            OperationType.LessThan => 3,
            OperationType.Equals => 3,
            OperationType.RelativeBaseOffset => 1,
            OperationType.Halt => 0
        };

        public override string ToString() => $"{Type}";
    }

    public enum OperationType
    {
        Add                = 1,
        Multiply           = 2,
        Input              = 3,
        Output             = 4,
        JumpIfTrue         = 5,
        JumpIfFalse        = 6,
        LessThan           = 7,
        Equals             = 8,
        RelativeBaseOffset = 9,
        Halt               = 99
    }

    public enum ParameterMode
    {
        Position  = 0,
        Immediate = 1,
        Relative  = 2
    }

    public enum State
    {
        NotStarted,
        WaitingForInput,
        Halted
    }
}