using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions
{
    [Aoc(8)]
    public class Day08 : IAocDay
    {
        public string Input { get; set; }

        public Day08(ILoader loader) => Input = loader.ReadAllText("Input.txt");

        public IEnumerable<int[,]> FillLayers(string inputString, int width, int height)
        {
            var input = inputString.ToCharArray().Select(x => int.Parse(x.ToString())).ToArray();
            
            var pixelsPerLayer = width * height;
            var layerCount = (int) Math.Ceiling((double) input.Length / pixelsPerLayer);
            
            var pos = 0;
            for (var layerId = 0; layerId < layerCount; layerId++)
            {
                var layer = new int[height, width];
                for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    layer[y, x] = input[pos];
                    pos++;
                }

                yield return layer;
            }
        }

        public string SolvePart1()
        {
            var layers = FillLayers(Input, 25, 6);

            var selectedLayer = layers.OrderBy(l => GetDigitNumPerLayer(l, 0)).First();

            var result =GetDigitNumPerLayer(selectedLayer, 1) * GetDigitNumPerLayer(selectedLayer, 2);
            return result.ToString();
        }

        int GetDigitNumPerLayer(int[,] layer, int digit) => layer.Cast<int>().Count(x => x == digit);


        public int[,] AggregateLayers(List<int[,]> layers)
        {
            var height = layers[0].GetLength(0);
            var width = layers[0].GetLength(1);

            var result = new int[ height, width];
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var pixels = layers.Select(l => l[y, x]);
                result[y, x] = pixels.Reverse().Aggregate((accumulate, next) => next == 2 ? accumulate : next);
            }

            return result;
        }

        public string PrintLayer(int[,] layer)
        {
            var height = layer.GetLength(0);
            var width = layer.GetLength(1);

            var sb = new StringBuilder();
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    sb.Append(layer[y, x] == 1 ? '#' : ' ');
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string SolvePart2()
        {
            var layers = FillLayers(Input, 25, 6);
            var aggregate = AggregateLayers(layers.ToList());
            return Environment.NewLine + PrintLayer(aggregate);
        }
    }
}