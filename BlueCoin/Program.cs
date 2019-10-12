using BlueCoin.Classes;
using System;
using System.Linq;

namespace BlueCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            Blockchain blockchain = new Blockchain();
            blockchain.AddTransaction(0, 200, 15m);
            Console.WriteLine(blockchain.MineFirstPendingTransaction(2));
            var hash = blockchain.Chain[1].Transactions[0].Hash;
            string hashString = string.Empty;
            foreach (var currByte in hash)
            {
                hashString.Insert(hashString.Length, ((int)currByte).ToString("X4"));
            }
            Console.WriteLine(hashString);
        }
    }
}
