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
        
        public IntcodeComputer(IntcodeProgram program) =>_program = program;

        public Stack<int> Inputs { get; } = new Stack<int>();
        public Stack<int> Outputs { get; } = new Stack<int>();

        public int RunToEnd()
        {
            var programPosition = 0;
            while (true)
            {
                var statementRaw = _program.Memory[programPosition .. Math.Min(programPosition + 4, _program.Memory.Length)];
                var statement = Statement.Parse(statementRaw);
                programPosition += statement.Length;
                
                switch (statement.Operation.Type)
                {
                    case OperationType.Add:
                    case OperationType.Multiply:
                        _program.Memory[statement.Parameter3.Value] = statement.Operation.Type switch
                        {
                            OperationType.Add => ReadParameter(statement.Parameter1) + ReadParameter(statement.Parameter2),
                            OperationType.Multiply => ReadParameter(statement.Parameter1) * ReadParameter(statement.Parameter2),
                        };
                        break;

                    case OperationType.Output:
                        Outputs.Push(ReadParameter(statement.Parameter1));
                        break;

                    case OperationType.Input:
                        _program.Memory[statement.Parameter1.Value] = Inputs.Pop();
                        break;

                    case OperationType.JumpIfTrue:
                        if (ReadParameter(statement.Parameter1) != 0)
                            programPosition = ReadParameter(statement.Parameter2);
                        break;
                    
                    case OperationType.JumpIfFalse:
                        if (ReadParameter(statement.Parameter1) == 0)
                            programPosition = ReadParameter(statement.Parameter2);
                        break;
                    
                    case OperationType.LessThan:
                        _program.Memory[statement.Parameter3.Value] =
                            ReadParameter(statement.Parameter1) < ReadParameter(statement.Parameter2) ? 1 : 0; 
                        break;
                    
                    case OperationType.Equals:
                        _program.Memory[statement.Parameter3.Value] =
                            ReadParameter(statement.Parameter1) == ReadParameter(statement.Parameter2) ? 1 : 0; 
                        break;
                    
                    case OperationType.Halt:
                        return _program.Memory[0];
                }
            }
        }

        public int ReadParameter(Parameter p) => p.Mode switch
        {
            ParameterMode.Immediate => p.Value,
            ParameterMode.Position => _program.Memory[p.Value]
        };
    }
        
    public class IntcodeProgram
    {
        public int[] Memory { get; }

        public IntcodeProgram(string input)
        {
            Memory = input.Split(',').Select(int.Parse).ToArray();
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

        public static Statement Parse(int[] input)
        {
            var operationType = (OperationType) (ReadDigit(0, input[0]) + ReadDigit(1, input[0]) * 10);
            var operation = new Operation(operationType);
            
            var param1Mode = (ParameterMode) ReadDigit(2, input[0]);
            var param2Mode = (ParameterMode) ReadDigit(3, input[0]);
            var param3Mode = (ParameterMode) ReadDigit(4, input[0]);
            
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
        public Parameter(ParameterMode mode, int value)
        {
            Mode = mode;
            Value = value;
        }

        public ParameterMode Mode { get; }
        public int Value { get; }

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
            OperationType.Halt => 0
        };

        public override string ToString() => $"{Type}";
    }

    public enum OperationType
    {
        Add         = 1,
        Multiply    = 2,
        Input       = 3,
        Output      = 4,
        JumpIfTrue  = 5,
        JumpIfFalse = 6,
        LessThan    = 7,
        Equals      = 8,
        Halt        = 99
    }

    public enum ParameterMode
    {
        Position  = 0,
        Immediate = 1
    }
}