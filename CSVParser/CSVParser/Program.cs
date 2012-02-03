using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;

namespace CSVParser
{
    class Program
    {
        static void Main()
        {
            bool fExit = false;
            while (!fExit)
            {
                ParseAndDisplay getWeatherByCity = new ParseAndDisplay();
                Console.WriteLine("Enter a City: ");
                string sCity = Console.ReadLine();
                if (sCity == "x")
                    fExit = true;

                getWeatherByCity.getLatitudeAndLongitudeByCity(sCity);
                Console.ReadLine();
            }

        }
    }
}
