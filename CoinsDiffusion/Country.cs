using System;
using System.Drawing;
using System.IO;

namespace CoinsDiffusion
{
    public sealed class Country
    {
        private Point _leftBottomEdge;

        private Point _rightTopEdge;

        private bool _isCompleted;

        private int _completedCities;

        private bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (value)
                {
                    _isCompleted = true;
                    OnCountryCompleted(EventArgs.Empty);
                }
            }
        }

        public City[,] Cities;

        public string Name { get; }

        public uint DaysToBeComplete { get; set; }

        public event EventHandler<EventArgs> Completed;

        public Country(string name, Point leftBottomEdge, Point rightTopEdge)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 25)
                throw new ArgumentException("Country name can't be null and must have at most 25 characters");

            Name = name;
            _leftBottomEdge = leftBottomEdge;
            _rightTopEdge = rightTopEdge;
        }

        private void OnCountryCompleted(EventArgs e)
        {
            Completed?.Invoke(this, e);
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

        public void CheckForCompletionPossibility(object sender, NewDayComedEventArgs e)
        {
            var completionIsPossible = false;

            foreach (var city in Cities)
            {
                if (city.Balance.Count > 1)
                {
                    completionIsPossible = true;
                    break;
                }
            }
            if (!completionIsPossible)
            {
                foreach (var city in Cities)
                {
                    city.CityCompleted -= CheckForCompletion;
                    city.CityCompleted -= CheckForCompletion;
                }
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
