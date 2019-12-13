using System.IO;

namespace AdventOfCode
{
    public interface ILoader
    {
        string ReadAllText(string name);
        string[] ReadAllLines(string name);
    }

    public class Loader : ILoader
    {
        private readonly int _day;

        public Loader(int day)
        {
            _day = day;
        }

        private string GetPath(string name) => Path.Combine("inputs", $"Day{_day}_{name}");

        public string ReadAllText(string name) => File.ReadAllText(GetPath(name));
        public string[] ReadAllLines(string name) => File.ReadAllLines(GetPath(name));
    }
}