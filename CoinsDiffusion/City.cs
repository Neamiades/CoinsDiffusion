using System.Collections.Generic;

namespace CoinsDiffusion
{
    public class City
    {
        private List<Currency> Balance;

        public List<City> Neighbors;

        private Point Coordinates;

        public City(Point coordinates, int motif, uint startCapital)
        {
            Coordinates = coordinates;
            Balance = new List<Currency> { new Currency(motif, startCapital) };
        }

        public void TransactAllNeighbors()
        {
            foreach (var neighbor in Neighbors)
            {
            }
        }
    }
}
