using System;
using programmersdigest.MT940Parser;
using System.Linq;

namespace ParserPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            string mt940File = @"D:\work\test\MT940\MT940Parser\Resources\abnamro.txt";

            Parser p = new Parser(mt940File);

            var stmt = p.Parse().Select(r => r);

            foreach (var item in stmt)
            {
                Console.WriteLine($"AccountIdentification: {item.AccountIdentification}");
                Console.WriteLine($"OpeningBalance: {item.OpeningBalance}");
                Console.WriteLine($"ClosingAvailableBalance: {item.ClosingAvailableBalance}");
                Console.WriteLine($"ClosingBalance: {item.ClosingBalance}");
                Console.WriteLine("=========== ForwardAvailableBalances:");

                foreach (var fab in item.ForwardAvailableBalances)
                {
                    Console.WriteLine(fab.ToString());
                }
                Console.WriteLine("END ForwardAvailableBalances.");

                Console.WriteLine($"InformationToOwner: {item.InformationToOwner}");
                Console.WriteLine($"RelatedReference: {item.RelatedReference}");
                Console.WriteLine($"SequenceNumber: {item.SequenceNumber}");
                Console.WriteLine($"StatementNumber: {item.StatementNumber}");
                Console.WriteLine($"TransactionReferenceNumber: {item.TransactionReferenceNumber}");
                Console.WriteLine("=========== Lines:");
                foreach (var line in item.Lines)
                {
                    Console.WriteLine(line.ToString());
                }
                Console.WriteLine("=========== END Lines:");
            }
        }
    }
}
