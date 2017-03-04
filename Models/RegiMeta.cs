namespace Models
{
    /// <summary>
    ///     Contains information about a transaction id and a wallet ID to send the vote to
    /// </summary>
    public class RegiMeta
    {
        public string TxId { get; set; }

        public string RegistrarAddress { get; set; }
    }
}