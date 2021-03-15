using System;

namespace Quest.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Quest.Core.Grammar.QuestReader questReader = new Core.Grammar.QuestReader("{{eq:2},{eq:3},{matches:'[0-9]{1}'}}");
            var z = questReader.Eval("nima3");
            Console.WriteLine("Hello World!");
        }
    }
}
