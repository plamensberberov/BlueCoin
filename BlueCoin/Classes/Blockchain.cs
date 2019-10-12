using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace BlueCoin.Classes
{
    public class Blockchain
    {
        public Blockchain()
        {
            this.PendingTransactions = new List<Transaction>();
            this.Chain = new List<Block>();
            this.Chain.Add(this.GenerateGenesisBlock());
        }
        public List<Transaction> PendingTransactions { get; private set; }
        public decimal MiningReward => 10m;
        public List<Block> Chain { get; private set; }
        public int Difficulty => 1;

        public string AddTransaction(int sender, int receiver, decimal amount)
        {
            if (sender == receiver) return "Invalid Transaction";
            if (amount == 0m) return "Invalid Transaction";

            Transaction transaction = new Transaction(sender, receiver, amount);
            if (!transaction.IsValid()) return "Invalid Transaction";

            this.PendingTransactions.Add(transaction);
            return "Successful transaction!";
        }

        public string MineFirstPendingTransaction(int minerId)
        {
            if (this.PendingTransactions.Count < 1) return "No transactions to mine!";

            List<Transaction> minedTransactions = new List<Transaction>();
            minedTransactions.Add(PendingTransactions[0]);

            Block newBlock = new Block(DateTime.Now, minedTransactions, this.Chain.Last().Hash);
            newBlock.MineBlock(this.Difficulty);
            this.Chain.Add(newBlock);

            Transaction reward = new Transaction(0, minerId, MiningReward);
            PendingTransactions.Add(reward);

            return "Transaction Successfully mined!";
        }

        private Block GenerateGenesisBlock()
        {
            List<Transaction> genesisTransactions = new List<Transaction>();
            Transaction transaction = new Transaction(-1, 0, 100);
            genesisTransactions.Add(transaction);

            Block genesis = new Block(DateTime.Now, genesisTransactions, new byte[] { 0 });

            return genesis;
        }
    }

    public class Block
    {
        public Block(DateTime time, List<Transaction> transactions, byte[] previous)
        {
            this.Time = time;
            this.Transactions = transactions;
            this.Previous = previous;

            this.Hash = GenerateHash();
        }

        public List<Transaction> Transactions { get; private set; } 
        public DateTime Time { get; private set; }
        public byte[] Hash { get; private set; }
        public byte[] Previous { get; private set; }

        public int Nonce { get; private set; }

        private byte[] GenerateHash()
        {
            List<byte> transactionSeed = new List<byte>();
            foreach(var transaction in this.Transactions)
            {
                transactionSeed.AddRange(transaction.Hash.AsEnumerable());
            }

            byte[] hashSeed = BitConverter.GetBytes(this.Time.ToBinary())
                .Concat(this.Previous.AsEnumerable())
                .Concat(transactionSeed)
                .ToArray();

            SHA512 sha = new SHA512Managed();

            return sha.ComputeHash(hashSeed);
        }

        public void MineBlock(int difficulty)
        {
            SHA512 sha = new SHA512Managed();
            string minedCheck = string.Empty;
            for (int k = 0; k < difficulty; k++)
                minedCheck.Append('0');

            string mined = string.Empty;
            foreach(var currByte in this.Hash)
            {
                mined = mined.Insert(mined.Length ,((int)currByte).ToString("X4"));
            }

            for(int i = 1; mined != minedCheck; i++)
            {
                this.Nonce = i;

                List<byte> transactionSeed = new List<byte>();
                foreach (var transaction in this.Transactions)
                {
                    transactionSeed.AddRange(Hash.AsEnumerable());
                }

                byte[] hashSeed = BitConverter.GetBytes(this.Time.ToBinary())
                .Concat(this.Previous.AsEnumerable())
                .Concat(transactionSeed)
                .Append((byte)Nonce)
                .ToArray();

                this.Hash = sha.ComputeHash(hashSeed);

                string hashString = string.Empty;
                foreach (var currByte in this.Hash)
                {
                    hashString = hashString.Insert(hashString.Length, ((int)currByte).ToString("X4"));
                }
                //Console.WriteLine(hashString);
            }
        }

        private bool AreTransactionsValid()
        {
            foreach(var transaction in this.Transactions)
            {
                if (!transaction.IsValid()) return false;
            }
            return true;
        }
    }
}
