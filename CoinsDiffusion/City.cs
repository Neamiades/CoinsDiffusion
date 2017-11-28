using System;
using System.Collections.Generic;

using CountryHash = System.Int32;
using Amount = System.UInt32;

namespace CoinsDiffusion
{
    public class City
    {
        public Dictionary<int, Currency> Balance { get; }

        public List<City> Neighbors;

        private readonly int _motifsCount;

        private bool Completed { get; set; }

        public event EventHandler<NewDayComedEventArgs> CityCompleted;

        protected virtual void OnCityCompleted(NewDayComedEventArgs e)
        {
            CityCompleted?.Invoke(this, e);
        }

        public City(CountryHash countryHash, Amount startCapital, int motifsCount)
        {
            //Coordinates = coordinates;
            _motifsCount = motifsCount;
            Balance = new Dictionary<CountryHash, Currency> {{countryHash, new Currency(startCapital)}};
        }

        public void TransactAllNeighbors(object sender, NewDayComedEventArgs e)
        {
            var keys = Balance.Keys;

            foreach (var neighbor in Neighbors)
            {
                foreach (var key in keys)
                {
                    if (Balance[key].Amount > 1000)
                    {
                        var givenCoins = Balance[key].Amount / 1000;

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
