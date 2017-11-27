namespace CoinsDiffusion
{
    public struct Currency
    {
        public int Motif { get; }

        public uint Amount { get; set; }

        public Currency(int motif, uint amount)
        {
            Motif = motif;
            Amount = amount;
        }
    }
}
