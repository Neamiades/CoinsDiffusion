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

        public bool Completed { get; private set; }

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
            var xLenght = Cities.GetLength(0);
            var yLenght = Cities.GetLength(1);

            for (int x = 0; x < xLenght; x++)
            {
                for (int y = 0; y < yLenght; y++)
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
                    if (x + 1 < xLenght && Cities[x + 1, y] != null)
                    {
                        Cities[x, y].Neighbors.Add(Cities[x + 1, y]);
                    }
                    if (y - 1 >= 0 && Cities[x, y - 1] != null)
                    {
                        Cities[x, y].Neighbors.Add(Cities[x, y - 1]);
                    }
                    if (y + 1 < yLenght && Cities[x, y + 1] != null)
                    {
                        Cities[x, y].Neighbors.Add(Cities[x, y + 1]);
                    }
                }
            }
        }

        private void CheckForCompletion(object sender, EventArgs e)
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
                    country.Completed -= CheckForCompletion;
                    country.DaysToBeComplete = 0;
                    CheckForCompletion(null, null);
                }
            }
        }

        //public void OutPutCityMap(object sender, EventArgs e)
        //{
        //    Console.SetCursorPosition(0, 0);
        //    var xLenght = Cities.GetLength(0);
        //    var yLenght = Cities.GetLength(1);

        //    for (int y = 0; y < yLenght; y++)
        //    {
        //        for (int x = 0; x < xLenght; x++)
        //        {
        //            Console.ForegroundColor = Cities[x, y] == null
        //                ? ConsoleColor.Gray
        //                : Cities[x, y].Completed
        //                    ? ConsoleColor.Green
        //                    : ConsoleColor.Yellow;
        //            Console.Write("* ");
        //        }
        //        Console.WriteLine();
        //    }
        //}

        public void SubscribeAll()
        {
            foreach (var country in Countries)
            {
                foreach (var city in country.Cities)
                {
                    NewDayHasCome += city.TransactAllNeighbors;
                    city.CityCompleted += country.CheckForCompletion;
                }
                country.Completed += CheckForCompletion;
            }
        }
    }
}
