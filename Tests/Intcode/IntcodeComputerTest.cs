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
            computer.RunToEnd().Should().Be(expectedReturnCode);
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
            computer.RunToEnd();
            
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
            computer.Inputs.Push(input);
            
            // act
            computer.RunToEnd();
            var output = computer.Outputs.Pop();
            
            // assert
            output.Should().Be(input);
        }
    }
}