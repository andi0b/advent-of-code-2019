﻿using System.Linq;
using System.Net.NetworkInformation;
using AdventOfCode.Solutions;
using AdventOfCode.Solutions.Intcode;
using FluentAssertions;
using Xunit;

namespace Tests.Intcode
{
    public class IntcodeComputerTest
    {
        /// <summary>
        /// Test cases from Day 2
        /// </summary>
        [Theory]
        [InlineData("1,9,10,3,2,3,11,0,99,30,40,50", 3500)]
        [InlineData("1,1,1,4,99,5,6,0,99", 30)]
        public void SimpleRunToEnd(string input, int expectedReturnCode)
        {
            // arrange
            var program = new IntcodeProgram(input);
            var computer = new IntcodeComputer(program);
            
            // act & assert
            computer.Run().Should().Be(expectedReturnCode);
        }

        /// <summary>
        /// Test case from Day 5 Part 1
        /// </summary>
        [Fact]
        public void PositonModeTest()
        {
            // arrange
            var program = new IntcodeProgram("1002,4,3,4,33");
            var computer = new IntcodeComputer(program);

            // act
            computer.Run();
            
            // assert
            program.Memory[4].Should().Be(99);
        }

        /// <summary>
        /// IO Test from Day 5 Part 1
        /// </summary>
        [Theory]
        [InlineData(5)]
        [InlineData(-15)]
        [InlineData(9999)]
        public void InputOutputTest(int input)
        {
            // arrange
            var program = new IntcodeProgram("3,0,4,0,99");
            var computer = new IntcodeComputer(program);
            computer.Inputs.Enqueue(input);
            
            // act
            computer.Run();
            var output = computer.Outputs.Dequeue();
            
            // assert
            output.Should().Be(input);
        }

        [Theory]
        [InlineData(-100,999)]
        [InlineData(0,999)]
        [InlineData(7,999)]
        [InlineData(8,1000)]
        [InlineData(9,1001)]
        [InlineData(9999,1001)]
        public void ExtendedJumpTests(int input, int expectedOutput)
        {
            // arrange
            var program = new IntcodeProgram("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,"+
                                             "1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,"+
                                             "999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99");
            var computer = new IntcodeComputer(program);
            computer.Inputs.Enqueue(input);
            
            // act
            computer.Run();
            var output = computer.Outputs.Dequeue();
            
            // assert
            output.Should().Be(expectedOutput);
        }

        [Fact]
        public void LongExt_Itself()
        {
            var program = new IntcodeProgram("109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99");
            var computer = new IntcodeComputer(program);

            computer.Run();

            computer.Outputs.Should().BeEquivalentTo(new long[] {109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99});

        }
        
        [Fact]
        public void LongExt_SpecificNumber()
        {
            var program = new IntcodeProgram("104,1125899906842624,99");
            var computer = new IntcodeComputer(program);

            computer.Run();

            computer.Outputs.Single().Should().Be(1125899906842624);
        }

        [Fact]
        public void a()
        {
            var program = new IntcodeProgram("1102,34915192,34915192,7,4,7,99,0");
            var computer = new IntcodeComputer(program);

            computer.Run();

            computer.Outputs.Single().ToString().Length.Should().Be(16);
        }
    }
}