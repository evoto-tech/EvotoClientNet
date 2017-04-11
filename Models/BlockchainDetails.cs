using System;

namespace Models
{
    public class BlockchainDetails
    {
        public string Name { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ChainString { get; set; }
        public string Info { get; set; }
        public int Port { get; set; }
        public string WalletId { get; set; }
        public int Blocks { get; set; }
        public string EncryptKey { get; set; }

        public bool ShouldEncryptResults => !string.IsNullOrWhiteSpace(EncryptKey);

        public override string ToString()
        {
            return Name;
        }
    }
}