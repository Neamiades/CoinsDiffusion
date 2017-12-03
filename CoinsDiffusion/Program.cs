using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CoinsDiffusion
{
    static class Program
    {
        private static List<Case> _cases;

        private const uint StartCityCapital = 1_000_000;

        private const int MinimalCoordinate = 1;

        private const int MaximalCoordinate = 10;

        private const string ConstantRules = "The country description has the format: name xl yl xh yh," +
                                             "where name is a single word with at most 25 characters;" +
                                             "xl, yl are the lower left city coordinates of that country (most southwestward city )" +
                                             "and xh, yh are the upper right city coordinates of that country (most northeastward city)." +
                                             "The last case in the input is followed by a single zero.";

        private static string TemporaryRules = $"{MinimalCoordinate} <= xl <= xh <= {MaximalCoordinate} and " +
                                               $"{MinimalCoordinate} <= yl <= yh <= {MaximalCoordinate}.";

        static void Main()
        {
            if (ParseInputData())
            {
                SetCasesDataForModeling();
                ModelCases();
                OutPutModelingResults();
            }
        }

        private static void OutPutModelingResults()
        {
            for (var index = 0; index < _cases.Count; index++)
            {
                var caseCountries = _cases[index].Countries.OrderBy(c => c.DaysToBeComplete).ThenBy(c => c.Name);

                Console.WriteLine($"Case number {index + 1}");

                foreach (var country in caseCountries)
                {
                    Console.WriteLine($"{country.Name} {country.DaysToBeComplete}");
                }
            }
        }

        private static void ModelCases()
        {
            foreach (var diffusionCase in _cases)
            {
                if (!diffusionCase.Completed)
                {
                    diffusionCase.ChangeDay();
                    diffusionCase.CheckForCompletionPossibility();
                }
            }
            while (_cases.Any(c => !c.Completed))
            {
                foreach (var diffusionCase in _cases)
                {
                    if (!diffusionCase.Completed)
                    {
                        diffusionCase.ChangeDay();
                    }
                }
            }
        }

        private static void SetCasesDataForModeling()
        {
            for (var index = 0; index < _cases.Count; index++)
            {
                var diffusionCase = _cases[index];
                try
                {
                    diffusionCase.FillCitiesMap();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message} (Case #{index + 1})");
                    _cases[index].Completed = true;
                    continue;
                }
                diffusionCase.IdentifyCitiesNeighbors();
                diffusionCase.SubscribeAll();
            }
        }

        private static bool ParseInputData()
        {
            if (!File.Exists("input.txt"))
            {
                Console.WriteLine("Error: Input file doesn't exist");

                return false;
            }

            using (var fs = new FileStream("input.txt", FileMode.OpenOrCreate))
            {
                using (var input = new StreamReader(fs))
                {
                    _cases = new List<Case>();
                    string line;

                    for (int n = 0; (line = input.ReadLine()) != null; n++)
                    {
                        if (!UInt16.TryParse(line, out ushort countriesCount))
                        {
                            Console.WriteLine("Error: Incorrect countries number in input");
                            return false;
                        }

                        if (countriesCount == 0)
                        {
                            break;
                        }

                        _cases.Add(new Case { Countries = new Country[countriesCount], StartCityCapital = StartCityCapital });
                        int xlMin, xhMax, ylMin, yhMax;

                        xhMax = yhMax = MinimalCoordinate;
                        xlMin = ylMin = MaximalCoordinate;

                        for (int i = 0; i < countriesCount && (line = input.ReadLine()) != null; i++)
                        {
                            var countryEntry = line.Split();

                            if (countryEntry.Length != 5
                                || !Int32.TryParse(countryEntry[1], out int xl)
                                || !Int32.TryParse(countryEntry[2], out int yl)
                                || !Int32.TryParse(countryEntry[3], out int xh)
                                || !Int32.TryParse(countryEntry[4], out int yh)
                                || !ValidateCoordinateRange(xl, yl, xh, yh)
                                )
                            {
                                Console.WriteLine($"Error: Incorrect country input\n{ConstantRules}\n{TemporaryRules}");
                                return false;
                            }

                            try
                            {
                                _cases[n].Countries[i] = new Country(countryEntry[0], new Point(xl, yl), new Point(xh, yh));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                return false;
                            }
                            CitiesMapPointsDetect(ref xlMin, ref xhMax, ref ylMin, ref yhMax, xl, yl, xh, yh);
                        }
                        _cases[n].Cities = new City[xhMax - xlMin + 1, yhMax - ylMin + 1];
                        foreach (var country in _cases[n].Countries)
                        {
                            country.CorrectCoordinates(xlMin, ylMin);
                        }
                    }
                }
            }
            return true;
        }

        private static bool ValidateCoordinateRange(int xl, int yl, int xh, int yh)
        {
            return xl >= MinimalCoordinate && xl <= xh && xh <= MaximalCoordinate &&
                   yl >= MinimalCoordinate && yl <= yh && yh <= MaximalCoordinate;
        }

        private static void CitiesMapPointsDetect(ref int xlMin, ref int xhMax, ref int ylMin, ref int yhMax, int xl, int yl, int xh, int yh)
        {
            if (xl < xlMin)
            {
                xlMin = xl;
            }
            if (yl < ylMin)
            {
                ylMin = xl;
            }
            if (xh > xhMax)
            {
                xhMax = xh;
            }
            if (yh > yhMax)
            {
                yhMax = yh;
            }
        }
    }
}
