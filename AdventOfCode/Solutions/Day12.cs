using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using Autofac;

namespace AdventOfCode.Solutions
{
    [Aoc(12)]
    public class Day12 : IAocDay
    {
        public string Input { get; set; }

        public Day12(ILoader loader) => Load(loader.ReadAllLines("Input.txt"));

        public void Load(string[] lines) => Moons = lines.Select(Moon.Parse).ToList();

        public List<Moon> Moons { get; set; }

        public struct Point3d
        {
            public int X { get; }
            public int Y { get; }
            public int Z { get; }

            public Point3d(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public int AbsoluteSum => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

            public Point3d AddPoint(Point3d other) => new Point3d(X+other.X, Y+other.Y, Z+other.Z);

            public override string ToString() => $"<x={X.ToString().PadLeft(2)}, y={Y.ToString().PadLeft(2)}, z={Z.ToString().PadLeft(2)}>";
        }
        
        public class Moon
        {
            static Regex _parseRegex = new Regex(@".*x=(-?\d*).*y=(-?\d*).*z=(-?\d*)"); 
            
            public Point3d Position { get; private set; }
            public Point3d Velocity { get; private set; } = new Point3d(0, 0, 0);

            public int PotentialEnergy => Position.AbsoluteSum * Velocity.AbsoluteSum;
            
            public static Moon Parse(string input)
            {
                var match = _parseRegex.Match(input);
                var nums = match.Groups.OfType<Group>().Skip(1).Select(x => int.Parse(x.Value)).ToArray();
                return new Moon {Position = new Point3d(nums[0], nums[1], nums[2])};
            }

            public void Step() => Position = Position.AddPoint(Velocity);

            public void AdjustGravity(Moon other)
            {
                Velocity = new Point3d(
                    Velocity.X + Adjust(Position.X, other.Position.X),
                    Velocity.Y + Adjust(Position.Y, other.Position.Y),
                    Velocity.Z + Adjust(Position.Z, other.Position.Z)
                );
                
                int Adjust(int me, int other)
                {
                    if (me < other) return 1;
                    if (me > other) return -1;
                    return 0;
                }
            }

            public override string ToString() => $"pos={Position}, vel={Velocity}";
        }

        public void Step()
        {
            foreach (var moon in Moons)
            {
                var others = Moons.Except(new[] {moon});
                foreach (var other in others)
                    moon.AdjustGravity(other);
            }

            foreach (var moon in Moons) moon.Step();
        }
        
        public string SolvePart1()
        {
            for (var i = 0; i < 1000; i++)
            {
                Step();
            }
            
            return $"total energy after 1000 steps: {Moons.Sum(x => x.PotentialEnergy)}";
        }

        public string SolvePart2()
        {
            var startDimensions = GetAllDimensions(Moons);

            var repeatResults = new Dictionary<int, int>();

            for (var i = 1; repeatResults.Count < startDimensions.Length; i++)
            {
                Step();

                var currentDimensions = GetAllDimensions(Moons);

                for (var dimensionId = 0; dimensionId < startDimensions.Length; dimensionId++)
                {
                    if (startDimensions[dimensionId].SequenceEqual(currentDimensions[dimensionId]))
                    {
                        if (!repeatResults.ContainsKey(dimensionId))
                            repeatResults[dimensionId] = i;
                    }
                }
            }

            return $"first repeating iteration at {lcm_of_array_elements(repeatResults.Values.Select(x => (long) x).ToArray())}";

            List<int>[] GetAllDimensions(IEnumerable<Moon> moons)
                => moons.Select(moon => new[]
                {
                    new[] {moon.Position.X, moon.Velocity.X},
                    new[] {moon.Position.Y, moon.Velocity.Y},
                    new[] {moon.Position.Z, moon.Velocity.Z},
                }).Aggregate(new[] {new List<int>(), new List<int>(), new List<int>()},
                             (result, next) =>
                             {
                                 result[0].AddRange(next[0]);
                                 result[1].AddRange(next[1]);
                                 result[2].AddRange(next[2]);
                                 return result;
                             });
        }
        
        // C# Program to find LCM of n elements 
        // This Code is contributed by nitin mittal 
        public static long lcm_of_array_elements(long[] element_array) 
        { 
            long lcm_of_array_elements = 1; 
            long divisor = 2; 
          
            while (true) { 
              
                int counter = 0; 
                bool divisible = false; 
                for (int i = 0; i < element_array.Length; i++) { 
  
                    // lcm_of_array_elements (n1, n2, ... 0) = 0. 
                    // For negative number we convert into 
                    // positive and calculate lcm_of_array_elements. 
                    if (element_array[i] == 0) { 
                        return 0; 
                    } 
                    else if (element_array[i] < 0) { 
                        element_array[i] = element_array[i] * (-1); 
                    } 
                    if (element_array[i] == 1) { 
                        counter++; 
                    } 
  
                    // Divide element_array by devisor if complete 
                    // division i.e. without remainder then replace 
                    // number with quotient; used for find next factor 
                    if (element_array[i] % divisor == 0) { 
                        divisible        = true; 
                        element_array[i] = element_array[i] / divisor; 
                    } 
                } 
  
                // If divisor able to completely divide any number 
                // from array multiply with lcm_of_array_elements 
                // and store into lcm_of_array_elements and continue 
                // to same divisor for next factor finding. 
                // else increment divisor 
                if (divisible) { 
                    lcm_of_array_elements = lcm_of_array_elements * divisor; 
                } 
                else { 
                    divisor++; 
                } 
  
                // Check if all element_array is 1 indicate  
                // we found all factors and terminate while loop. 
                if (counter == element_array.Length) { 
                    return lcm_of_array_elements; 
                } 
            } 
        } 
    }
}