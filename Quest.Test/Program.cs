using System;

namespace Quest.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Quest.Core.Grammar.QuestReader questReader = new Core.Grammar.QuestReader("{ in: ['abc', 'efg', 1, 2, {gt: 5}, not: null, {gt: 2, lt: 7, not: 6, eq:1}, in:[1,2,3]], matches: '[0-9]{2}' }");
            var z = questReader.Parse();
            Console.WriteLine("Hello World!");
        }
    }
}
