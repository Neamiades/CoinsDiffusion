using System;
using System.IO;

namespace CoinsDiffusion
{
    public class Country
    {
        private Point _leftBottomEdge;

        private Point _rightTopEdge;

        private int _completedCities;

        public City[,] Cities;

        public string Name { get; }

        public event EventHandler<EventArgs> Completed;

        protected virtual void OnCountryCompleted(EventArgs e)
        {
            Completed?.Invoke(this, e);
        }

        private bool _isCompleted;

        public bool IsCompleted
        {
            get => _isCompleted;
            private set
            {
                if (value)
                {
                    _isCompleted = true;
                    OnCountryCompleted(EventArgs.Empty);
                }
            }
        }

        public uint DaysToBeComplete { get; private set; }

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
            _leftBottomEdge = leftBottomEdge;
            _rightTopEdge = rightTopEdge;
        }

        public void FillCities(City[,] cities, uint startCapital, int motifsCount)
        {
            Cities = new City[_rightTopEdge.X - _leftBottomEdge.X + 1, _rightTopEdge.Y - _leftBottomEdge.Y + 1];

            for (int x = _leftBottomEdge.X, i = 0; x <= _rightTopEdge.X; x++, i++)
            {
                for (int y = _leftBottomEdge.Y, j = 0; y <= _rightTopEdge.Y; y++, j++)
                {
                    if (cities[x, y] != null)
                    {
                        throw new InvalidDataException("Incorrect input data: countries territories borders conflict");
                    }
                    cities[x, y] = Cities[i, j] = new City(Name.GetHashCode(), startCapital, motifsCount);
                }
            }
        }

        public void CheckForCompletion(object sender, NewDayComedEventArgs e)
        {
            _completedCities++;
            if (Cities.Length == _completedCities)
            {
                IsCompleted = true;
                DaysToBeComplete = e.CurrentDay;
            }
        }

        public void CorrectCoordinates(int xAmendment, int yAmendment)
        {
            _leftBottomEdge.X -= xAmendment;
            _leftBottomEdge.Y -= yAmendment;

            _rightTopEdge.X -= xAmendment;
            _rightTopEdge.Y -= yAmendment;
        }
    }
}
