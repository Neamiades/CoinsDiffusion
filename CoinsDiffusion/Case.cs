using System.Collections.Generic;

namespace CoinsDiffusion
{
    public class Case
    {
        public Country[] Countries;

        public City[,] Cities;

        public uint StartCityCapital = 1_000_000;

        public void FillCitiesMap()
        {
            foreach (var country in Countries)
            {
                country.FillCities(Cities, StartCityCapital);
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
                    Cities[x,y].Neighbors = new List<City>();
                    if (x - 1 > 0)
                    {
                        Cities[x, y].Neighbors.Add(Cities[x - 1, y]);
                    }
                    if (x + 1 < xLenght)
                    {
                        Cities[x, y].Neighbors.Add(Cities[x + 1, y]);
                    }
                    if (y - 1 > 0)
                    {
                        Cities[x, y].Neighbors.Add(Cities[x, y - 1]);
                    }
                    if (y + 1 < yLenght)
                    {
                        Cities[x, y].Neighbors.Add(Cities[x, y + 1]);
                    }
                }
            }
        }
    }
}
