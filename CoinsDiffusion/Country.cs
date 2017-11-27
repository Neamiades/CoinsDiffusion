using System;
using System.IO;

namespace CoinsDiffusion
{
    public class Country
    {
        private Point LeftBottomEdge { get; }

        private Point RightTopEdge { get; }

        private City[,] _cities;

        public string Name { get; }

        public bool Completed { get; } = false;

        public uint DaysToBeComplete { get; } = 0;


        public Country(string name, Point leftBottomEdge, Point rightTopEdge)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 25)
                throw new ArgumentException("Country name can't be null and must have at most 25 characters");

            if (leftBottomEdge.X < 1 || leftBottomEdge.X > rightTopEdge.X || rightTopEdge.X > 10 ||
                leftBottomEdge.Y < 1 || leftBottomEdge.Y > rightTopEdge.Y || rightTopEdge.Y > 10)
            {
                throw new ArgumentException("Countries edge cities coordinates must be at least bigger than 0 and less than 11, " +
                                            "also most southwestward city can't be northeastwardest than most northeastward city.\n" +
                                            "In simple: 1 <= xl <= xh <= 10 and 1 <= yl <= yh <= 10");
            }

            Name = name;
            LeftBottomEdge = leftBottomEdge;
            RightTopEdge = rightTopEdge;
        }

        public void FillCities(City[,] cities, uint startCapital)
        {
            _cities = new City[RightTopEdge.X - LeftBottomEdge.X + 1, RightTopEdge.Y - LeftBottomEdge.Y + 1];

            for (int x = LeftBottomEdge.X, i = 0; x <= RightTopEdge.X; x++, i++)
            {
                for (int y = LeftBottomEdge.Y, j = 0; y < RightTopEdge.Y; y++, j++)
                {
                    if (cities[x, y] != null)
                    {
                        throw new InvalidDataException("Incorrect input data: countries territories borders conflict");
                    }
                    cities[x, y] = _cities[i, j] = new City(new Point(x, y), Name.GetHashCode(), startCapital);
                }
            }
        }
    }
}
