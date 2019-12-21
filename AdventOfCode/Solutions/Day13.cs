using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks.Dataflow;
using AdventOfCode.Solutions.Intcode;

namespace AdventOfCode.Solutions
{
    [Aoc(13)]
    public class Day13 : IAocDay
    {

        public Day13(ILoader loader) => Input = loader.ReadAllText("Input.txt");

        public string Input { get; set; }

        public string SolvePart1()
        {
            
            var screen = new Screen();
            var program = new IntcodeProgram(Input);
            var computer = new IntcodeComputer(program);
            computer.Run();

            while (computer.Outputs.Any())
            {
                var x = (int) computer.Outputs.Dequeue();
                var y = (int) computer.Outputs.Dequeue();
                var tileType = (Tile) computer.Outputs.Dequeue();

                screen.SetTile((x, y), tileType);
            }

            return $"Total block count on screen: {screen.Tiles.Values.Count(x => x == Tile.Block)}";
        }

        public string SolvePart2()
        {
            var screen = new Screen();
            
            var program = new IntcodeProgram(Input);
            
            // hack to play for free:
            program.Memory[0] = 2;
            
            var computer = new IntcodeComputer(program);
            computer.Run();

            int score = 0;
            
            for (; ; computer.Run())
            {
                while (computer.Outputs.Any())
                {
                    var x = (int) computer.Outputs.Dequeue();
                    var y = (int) computer.Outputs.Dequeue();
                    var tile = (Tile) computer.Outputs.Dequeue();

                    if (x == -1 && y == 0)
                        score = (int) tile;

                    screen.SetTile((x, y), tile);
                }

                if (!screen.HasBlocks)
                    break;
                
                var joystick =
                    screen.BallPosition.x == screen.PaddlePosition.x
                        ? 0
                        : screen.BallPosition.x < screen.PaddlePosition.x
                            ? -1
                            : 1;

                computer.Inputs.Enqueue(joystick);
            }

            return $"Score: {score}";
        }

        private ((int x, int y) ballPosition, (int x, int y) paddlePosition, int score, bool hasBlocks) ReadOutput(Queue<long> queue)
        {
            var ballPosition = (x:-1, y:-1);
            var paddlePosition = (x:-1, y:-1);
            var score = -1;
            var hasBlocks = false;
            
            while (queue.Any())
            {
                var x = (int) queue.Dequeue();
                var y = (int) queue.Dequeue();
                var tile = (Tile) queue.Dequeue();

                if (tile == Tile.Block)
                    hasBlocks = true;
                
                if (x == -1 && y == 0)
                    score = (int) tile;

                if (tile == Tile.Ball)
                    ballPosition = (x, y);

                if (tile == Tile.Paddle)
                    paddlePosition = (x, y);
            }

            return (ballPosition, paddlePosition, score, hasBlocks);
        }

        public class Screen
        {
            public Dictionary<(int x, int y), Tile> Tiles { get; } = new Dictionary<(int x, int y), Tile>();

            public void SetTile((int x, int y) location, Tile tile) => Tiles[location] = tile;
            public Tile GetTile((int x, int y) location) => Tiles.TryGetValue(location, out var tile) ? tile : Tile.Empty;

            public (int x, int y) BallPosition => Tiles.Single(x => x.Value == Tile.Ball).Key;
            public (int x, int y) PaddlePosition => Tiles.Single(x => x.Value == Tile.Paddle).Key;
            public bool HasBlocks => Tiles.Any(x => x.Value == Tile.Block);
        }
    }

    public enum Tile
    {
        Empty  = 0,
        Wall   = 1,
        Block  = 2,
        Paddle = 3,
        Ball   = 4
    }
}