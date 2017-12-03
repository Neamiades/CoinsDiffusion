using System;
using System.Collections.Generic;

namespace CoinsDiffusion
{
    public class Case
    {
        public uint StartCityCapital = 1_000_000;

        private uint _dayCount;

        private int _completedCountries;

        public Country[] Countries;

        public City[,] Cities;

        public bool Completed { get; set; }

        public event EventHandler<NewDayComedEventArgs> NewDayHasCome;

        protected virtual void OnNewDayComming(NewDayComedEventArgs currDay)
        {
            NewDayHasCome?.Invoke(this, currDay);
        }

        public void ChangeDay()
        {
            _dayCount++;
            OnNewDayComming(new NewDayComedEventArgs(_dayCount));
        }

        public void FillCitiesMap()
        {
            foreach (var country in Countries)
            {
                country.FillCities(Cities, StartCityCapital, Countries.Length);
            }
        }

        public void IdentifyCitiesNeighbors()
        {
            var xLength = Cities.GetLength(0);
            var yLength = Cities.GetLength(1);

            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    if (Cities[x, y] == null)
                    {
                        continue;
                    }
                    Cities[x,y].Neighbors = new List<City>();
                    if (x - 1 >= 0 && Cities[x - 1, y] != null)
                    {
                        Cities[x, y].Neighbors.Add(Cities[x - 1, y]);
                    }
                    if (x + 1 < xLength && Cities[x + 1, y] != null)
                    {
                        Cities[x, y].Neighbors.Add(Cities[x + 1, y]);
                    }
                    if (y - 1 >= 0 && Cities[x, y - 1] != null)
                    {
                        Cities[x, y].Neighbors.Add(Cities[x, y - 1]);
                    }
                    if (y + 1 < yLength && Cities[x, y + 1] != null)
                    {
                        Cities[x, y].Neighbors.Add(Cities[x, y + 1]);
                    }
                }
            }
        }

        private void CompleteIfAllDone(object sender, EventArgs e)
        {
            _completedCountries++;

            if (Countries.Length == _completedCountries)
            {
                Completed = true;
            }
        }

        public void CheckForCompletionPossibility()
        {
            foreach (var country in Countries)
            {
                var completionIsPossible = false;

                foreach (var city in country.Cities)
                {
                    if (city.Balance.Count > 1)
                    {
                        completionIsPossible = true;
                        break;
                    }
                }

                if (!completionIsPossible)
                {
                    foreach (var city in country.Cities)
                    {
                        NewDayHasCome -= city.TransactAllNeighbors;
                        city.CityCompleted -= country.CheckForCompletion;
                    }
                    country.Completed -= CompleteIfAllDone;
                    country.DaysToBeComplete = 0;
                    CompleteIfAllDone(null, null);
                }
            }
        }

        public void SubscribeAll()
        {
            foreach (var country in Countries)
            {
                foreach (var city in country.Cities)
                {
                    NewDayHasCome += city.TransactAllNeighbors;
                    city.CityCompleted += country.CheckForCompletion;
                }
                country.Completed += CompleteIfAllDone;
            }
        }
    }
}
