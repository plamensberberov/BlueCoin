using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlueCoin.Classes
{
    public class Transaction
    {
        public Transaction(int sender, int receiver, decimal amount)
        {
            this.Sender = sender;
            this.Receiver = receiver;
            this.Amount = amount;
            this.Time = DateTime.Now;
            this.Hash = GenerateHash();
        }

        public int Sender { get; private set; }
        public int Receiver { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Time { get; private set; }
        public byte[] Hash { get; private set; }

        public byte[] GenerateHash()
        {
            byte[] hashSeed = BitConverter.GetBytes(this.Sender)
                .Concat(BitConverter.GetBytes(this.Receiver))
                .Concat(BitConverter.GetBytes(this.Time.ToBinary()))
                .Concat(BitConverter.GetBytes(Amount.GetHashCode()))
                .ToArray();

            SHA512 sha = new SHA512Managed();

            return sha.ComputeHash(hashSeed);
        }

        public bool IsValid()
        {
            if (Sender == Receiver) return false;
            if (Amount == 0m) return false;
            return true;
        }
    }
}
