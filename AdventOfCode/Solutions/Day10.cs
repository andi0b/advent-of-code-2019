using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using AdventOfCode.Solutions.Intcode;

namespace AdventOfCode.Solutions
{
    [Aoc(10)]
    public class Day10 : IAocDay
    {
        public List<(int x, int y)> Asteroids { get; private set; }

        public Day10(ILoader loader) => Load(loader.ReadAllLines("Input.txt"));

        public void Load(string[] input)
        {
            Asteroids =
                input.Select((line, lineNum) => line.ToCharArray()
                                                    .Select((chr, chrNum) => (x: chrNum, y: lineNum, isAsterioid: chr == '#')))
                     .SelectMany(x => x)
                     .Where(x => x.isAsterioid)
                     .Select(asteroid => (asteroid.x, asteroid.y))
                     .ToList();
        }

        public int CalculateVisibleAsteroidCount((int x, int y) reference, IEnumerable<(int x, int y)> otherAsteroids)
        {
            var polarCoordinates =
                from cartesian in otherAsteroids
                let polar = CalculatePolar(cartesian, reference)
                select (cartesian, polar);

            var groupedByAngle = polarCoordinates.GroupBy(x => x.polar.ang);

            return groupedByAngle.Count();
        }
        
        public ((int x, int y) asteroid, int visibleOthers) SolvePart1Internal()
        {
            var visibleAstroids =
                from asteroid in Asteroids
                let otherAsteroids = Asteroids.Except(new[] {asteroid})
                let visibleOthers = CalculateVisibleAsteroidCount(asteroid, otherAsteroids)
                orderby visibleOthers descending 
                select (asteroid, visibleOthers);

            return visibleAstroids.First();
        }

        public string SolvePart1()
        {
            var bestAsteroid = SolvePart1Internal();
            return $"selected asteroid: {bestAsteroid.asteroid.x},{bestAsteroid.asteroid.y} visible count: {bestAsteroid.visibleOthers}";
        }

        public IEnumerable<(int x, int y)> VaporizeAstroids((int x, int y) reference, List<(int x, int y)> asteroids)
        {
            var vaporizedAstroids = new List<(int x, int y)>();
            var existingAsteroids = asteroids;
            
            do
            {
                existingAsteroids = asteroids.Except(vaporizedAstroids).ToList();

                var nextAsteroids =
                    from asteroid in existingAsteroids
                    let polarCoordinates = CalculatePolar(asteroid, reference)
                    let distance = polarCoordinates.rad
                    let correctedAngle = TurnAngle(polarCoordinates.ang, 90)
                    group (asteroid, distance) by correctedAngle
                    into groupedByAngle
                    orderby groupedByAngle.Key
                    select (from x in groupedByAngle
                            orderby x.distance
                            select x).First();

                vaporizedAstroids.AddRange(nextAsteroids.Select(x => x.asteroid));

            } while (existingAsteroids.Any());

            return vaporizedAstroids;
        }

        public double TurnAngle(double polarAngle, double degreeOffset)
        {
            var angleOffset = Math.PI / 180 * degreeOffset;
            var turnedAngle = polarAngle + angleOffset;

            if (turnedAngle < 0)
                return Math.PI * 2 - turnedAngle;

            if (turnedAngle >= Math.PI * 2)
                return turnedAngle - Math.PI * 2;

            return turnedAngle;
        }

        public string SolvePart2()
        {
            var reference = SolvePart1Internal().asteroid;
            var others = Asteroids.Except(new[] {reference}).ToList();
            var varporized = VaporizeAstroids(reference, others);

            var vaporized200 = varporized.Skip(199).First();
            return
                $"The 200th asteroid to be vaporized is at {vaporized200.x},{vaporized200.y} (solution is {vaporized200.x * 100 + vaporized200.y})";
        }

        public (double rad, double ang) CalculatePolar((int x, int y) point, (int x, int y) referencePoint = default)
        {
            var x = point.x - referencePoint.x;
            var y = point.y - referencePoint.y;

            var ang = Math.Atan2(y, x);

            ang = ang > 0 ? ang : Math.PI * 2 + ang;
            
            return (
                Math.Sqrt(x * x + y * y),
                ang
            );
        }
    }
}