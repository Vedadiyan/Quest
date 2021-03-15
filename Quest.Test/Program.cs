using System;

namespace Quest.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Quest.Core.Grammar.QuestReader questReader = new Core.Grammar.QuestReader("{ or: [{in: ['Hello', 'efg', '1Hello' ] },{eq: 'Hello22'}]}");
            var z = questReader.Eval<string>("Hello");
            Console.WriteLine("Hello World!");
        }
    }
}
