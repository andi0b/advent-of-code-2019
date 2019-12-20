using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solutions.Intcode;

namespace AdventOfCode.Solutions
{
    [Aoc(6)]
    public class Day06 : IAocDay
    {
        private Dictionary<string, string> _outerToInnerMap;
        private IEnumerable<string> _allObjects;

        public Day06(ILoader loader) => LoadInput(loader.ReadAllLines("Input.txt"));

        public void LoadInput(string[] input)
        {
            var pairs = (
                from line in input
                let splitted = line.Split(')')
                select (inner: splitted[0], outer: splitted[1])
            ).ToList();

            _outerToInnerMap = pairs.ToDictionary(x => x.outer, x => x.inner);

            _allObjects = from pair in pairs
                          from obj in new[] {pair.inner, pair.outer}
                          select obj;
        }
        
        public int SolvePart1Internal()
        {
            var distinctObjects = _allObjects.Distinct().ToList();

            return distinctObjects.Select(CalculateOuterCount).Sum();
            
            int CalculateOuterCount(string obj)
            {
                for (int i = 0;; i++)
                {
                    if (!_outerToInnerMap.TryGetValue(obj, out obj))
                        return i;
                }
            }
        }

        public int SolvePart2Internal()
        {
            var pathFromSanta = PathToRoot("SAN").ToList();
            var pathFromMe = PathToRoot("YOU").ToList();

            IEnumerable<string> PathToRoot(string obj)
            {
                while (_outerToInnerMap.TryGetValue(obj, out obj))
                    yield return obj;
            }
            
            var intersectionPathLength = pathFromMe.Intersect(pathFromSanta).Count();

            return pathFromMe.Count + pathFromSanta.Count - intersectionPathLength * 2;
        }


        public string SolvePart1() => $"total number of direct and indirect orbits  {SolvePart1Internal()}";

        public string SolvePart2() => $"minimum number of orbital transfers {SolvePart2Internal()}";
    }
}