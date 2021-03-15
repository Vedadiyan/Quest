using System;

namespace Quest.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Quest.Core.Grammar.QuestReader questReader = new Core.Grammar.QuestReader("{ and: [{gt: 8},{not: 10},{lt: 20}] }");
            var z = questReader.Eval(19);
            Console.WriteLine("Hello World!");
        }
    }
}
