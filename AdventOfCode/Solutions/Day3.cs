using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions
{
    [Aoc(3)]
    public class Day3 : IAocDay
    {
        private ILoader _loader;
        private Wire    _wire1;
        private Wire    _wire2;

        public Day3(ILoader loader)
        {
            _loader = loader;

            var input = _loader.ReadAllLines("Part1.txt");
            
            _wire1 = new Wire(input[0]);
            _wire2 = new Wire(input[1]);
        }

        public string SolvePart1()
        {
            var crossingsByDistance =
                from crossing in CalculateWireCrossings(_wire1, _wire2)
                let distanceFromHub = ManhattanDistance(crossing, (0, 0))
                orderby distanceFromHub
                select (crossing, distanceFromHub);

            return $"closest manhattan distance to a crossing: {crossingsByDistance.FirstOrDefault().distanceFromHub}";
        }

        public string SolvePart2()
        {
            var crossingsByLength =
                from crossing in CalculateWireCrossings(_wire1, _wire2)
                let wire1Position = Array.FindIndex(_wire1.AllPoints, x => x == crossing) + 1
                let wire2Position = Array.FindIndex(_wire2.AllPoints, x => x == crossing) + 1
                let totalLength = wire1Position + wire2Position
                orderby totalLength
                select (crossing, totalLength);
            
            return $"closest total wire length to a crossing: {crossingsByLength.FirstOrDefault().totalLength}";
        }
        
        
        public IEnumerable<(int x, int y)> CalculateWireCrossings(Wire wire1, Wire wire2)
            => wire1.AllPoints.Intersect(wire2.AllPoints);

        public static int ManhattanDistance((int x, int y) point1, (int x, int y) point2)
            => Math.Abs(point1.x - point2.x) + Math.Abs(point1.y - point2.y);

        public class Wire
        {
            public string[] Directions { get; }

            public (int x, int y)[] AllPoints { get; set; }

            public Wire(string input)
            {
                Directions = input.Split(",");
                AllPoints = CalculatePoints().ToArray();
            }

            private IEnumerable<(int x, int y)> CalculatePoints()
            {
                var curPoint = (x: 0, y: 0);
                foreach (var direction in Directions)
                {
                    (int x, int y) vector = direction[0] switch
                    {
                        'R' => (1, 0),
                        'D' => (0, 1),
                        'L' => (-1, 0),
                        'U' => (0, -1)
                    };

                    var amount = int.Parse(direction.Substring(1));

                    for (var i = 0; i < amount; i++)
                    {
                        curPoint = (curPoint.x + vector.x, curPoint.y + vector.y);
                        yield return curPoint;
                    }
                }
            }
        }
    }
}