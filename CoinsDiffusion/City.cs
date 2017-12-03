using System;
using System.Collections.Generic;

using CountryHash = System.Int32;
using Amount = System.UInt32;

namespace CoinsDiffusion
{
    public class City
    {
        private const int CoinsToTransactOne = 1000;

        private readonly int _motifsCount;

        private bool Completed { get; set; }

        public List<City> Neighbors;

        public event EventHandler<NewDayComedEventArgs> CityCompleted;

        public Dictionary<int, Currency> Balance { get; }

        public City(CountryHash countryHash, Amount startCapital, int motifsCount)
        {
            _motifsCount = motifsCount;
            Balance = new Dictionary<CountryHash, Currency> { { countryHash, new Currency(startCapital) } };
        }

        protected virtual void OnCityCompleted(NewDayComedEventArgs e)
        {
            CityCompleted?.Invoke(this, e);
        }

        public void TransactAllNeighbors(object sender, NewDayComedEventArgs e)
        {
            var keys = Balance.Keys;

            foreach (var neighbor in Neighbors)
            {
                foreach (var key in keys)
                {
                    if (Balance[key].Amount > CoinsToTransactOne)
                    {
                        var givenCoins = Balance[key].Amount / CoinsToTransactOne;

                        if (neighbor.Balance.ContainsKey(key))
                        {
                            neighbor.Balance[key].Amount += givenCoins;
                        }
                        else
                        {
                            neighbor.Balance.Add(key, new Currency(givenCoins));
                        }
                        Balance[key].Amount -= givenCoins;

                        if (!neighbor.Completed && neighbor.Balance.Count == _motifsCount)
                        {
                            neighbor.Completed = true;
                            neighbor.OnCityCompleted(e);
                        }
                    }
                }
            }
        }
    }
}
