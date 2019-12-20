using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Solutions.Intcode;

namespace AdventOfCode.Solutions
{
    [Aoc(11)]
    public class Day11 : IAocDay
    {
        public string Input { get; set; }

        public Day11(ILoader loader) => Input = loader.ReadAllText("Input.txt");

        public string SolvePart1()
        {
            var surface = new Surface();
            PaintSurface(surface);
            return $"Total painted cells: {surface.GetTotalPaintedCells}";
        }

        private void PaintSurface(Surface surface)
        {
            var robot = new Robot();

            var program = new IntcodeProgram(Input);
            var computer = new IntcodeComputer(program);

            while (true)
            {
                computer.Inputs.Enqueue(surface.ReadColor(robot.Location));
                computer.Run();

                if (computer.State == State.Halted)
                    break;

                var newColor = (int) computer.Outputs.Dequeue();
                var turnDirection = (TurnDirection) computer.Outputs.Dequeue();

                surface.SetColor(robot.Location, newColor);

                robot.Turn(turnDirection);
                robot.Move();
            }
        }

        public string SolvePart2()
        {
            var surface = new Surface();
            surface.SetColor((0, 0), 1);
            PaintSurface(surface);

            var sb = new StringBuilder();
            for (var y = surface.Dict.Keys.Min(x => x.y); y <= surface.Dict.Keys.Max(x => x.y); y++)
            {
                sb.AppendLine();
                for (var x = surface.Dict.Keys.Min(x => x.x); x <= surface.Dict.Keys.Max(x => x.x); x++)
                {
                    sb.Append(surface.ReadColor((x, y)) == 1 ? "##" : "  " );
                }
            }

            return sb.ToString();
        }

        public class Surface
        {
            public Dictionary<(int x, int y), int> Dict { get; } = new Dictionary<(int x, int y), int>();

            public int ReadColor((int x, int y) point) => Dict.TryGetValue(point, out var color) ? color : 0;

            public void SetColor((int x, int y) point, int color) => Dict[point] = color;

            public int GetTotalPaintedCells => Dict.Count;
        }

        public class Robot
        {
            private int _direction = int.MaxValue / 8 * 4;
            public (int x, int y) Location { get; private set; } = (0, 0);
            
            public Direction Direction => (Direction) (_direction % 4);

            public void Turn(TurnDirection td) => _direction += td switch
            {
                TurnDirection.Left => -1,
                TurnDirection.Right => +1
            };

            public void Move() => Location = Direction switch
            {
                Direction.Up => (Location.x, Location.y - 1),
                Direction.Right => (Location.x + 1, Location.y),
                Direction.Down => (Location.x, Location.y + 1),
                Direction.Left => (Location.x - 1, Location.y),
            };
        }

        public enum Direction
        {
            Up = 0, Right = 1, Down = 2, Left = 3
        }

        public enum TurnDirection
        {
            Left = 0, Right = 1
        }
    }
}