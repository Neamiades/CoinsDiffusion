using System;
using System.Collections.Generic;
using System.Linq;

using Motif = System.Int32;
using Amount = System.UInt32;

namespace CoinsDiffusion
{
    public class City
    {
        private readonly Dictionary<Motif, Amount> _balance;

        private readonly List<Currency> _capital;

        public List<City> Neighbors;

        private readonly int _motifsCount;

        private bool _completed;

        public event EventHandler<NewDayComedEventArgs> CityCompleted;

        protected virtual void OnCityCompleted(NewDayComedEventArgs e)
        {
            CityCompleted?.Invoke(this, e);
        }

        public City(Motif motif, Amount startCapital, int motifsCount)
        {
            //Coordinates = coordinates;
            _motifsCount = motifsCount;
            _balance = new Dictionary<Motif, Amount> { {motif, startCapital} };
            _capital = new List<Currency>{ new Currency(motif, startCapital) };
        }

        public void TransactAllNeighbors(object sender, NewDayComedEventArgs e)
        {
            foreach (var neighbor in Neighbors)
            {
                foreach (var currency in _capital)
                {
                    if (currency.Amount > 1000)
                    {
                        if (neighbor._capital.Contains(currency))
                        {
                            neighbor._balance[motif] += _balance[motif] / 1000;
                        }
                        else
                        {
                            neighbor._balance.Add(motif, _balance[motif] / 1000);
                        }
                        _balance[motif] -= _balance[motif] / 1000;

                        if (!neighbor._completed && neighbor._balance.Count == _motifsCount)
                        {
                            neighbor._completed = true;
                            OnCityCompleted(e);
                        }
                    }
                }
            }
        }
    }
}
