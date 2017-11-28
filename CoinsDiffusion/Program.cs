using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace CoinsDiffusion
{
    static class Program
    {
        private static List<Case> _cases;

        private const uint StartCityCapital = 1_000_000;

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
                Console.WriteLine($"Case number {index + 1}");

                foreach (var country in _cases[index].Countries.OrderBy(c => c.DaysToBeComplete).ThenBy(c => c.Name))
                {
                    Console.WriteLine($"{country.Name} {country.DaysToBeComplete}");
                }
            }
        }

        private static void ModelCases()
        {
            foreach (var diffusionCase in _cases)
            {
                diffusionCase.ChangeDay();
                Thread.Sleep(100);
                diffusionCase.CheckForCompletionPossibility();
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
            foreach (var diffusionCase in _cases)
            {
                diffusionCase.FillCitiesMap();
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
                        //xlMin = xhMax = yhMax = ylMin = 0;

                        xhMax = yhMax = 0;
                        xlMin = ylMin = 100;

                        for (int i = 0; i < countriesCount && (line = input.ReadLine()) != null; i++)
                        {
                            var countryEntry = line.Split();

                            if (countryEntry.Length != 5
                                || !Int32.TryParse(countryEntry[1], out int xl)
                                || !Int32.TryParse(countryEntry[2], out int yl)
                                || !Int32.TryParse(countryEntry[3], out int xh)
                                || !Int32.TryParse(countryEntry[4], out int yh))
                            {
                                Console.WriteLine("Error: Incorrect country input");
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
