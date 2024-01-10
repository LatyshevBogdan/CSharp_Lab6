using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Linq;
using System.IO;

namespace Lab6
{
    class Program
    {
        static void Main(string[] args)
        {
            string Key = "e8b08bd171330bc6bde867b9a9c5b9dd";

            List<Weather> weathers = new List<Weather>();


            Random random = new Random();

            int n = 0;
            while (n < 10)
            {
                double latitude = -90 + random.NextDouble() * (90 + 90);
                double longitude = -180 + random.NextDouble() * (180 + 180);

                string url = "https://api.openweathermap.org/data/2.5/weather?lat=" + latitude.ToString() + "&" +
                              "lon=" + longitude.ToString() + "&exclude=minutely,hourly,daily,alerts&" +
                              "units=metric&appid=" + Key;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))

                {
                    response = streamReader.ReadToEnd();

                    WeatherResponce weatherResponse = JsonConvert.DeserializeObject<WeatherResponce>(response);

                    if (weatherResponse.Name != "" && weatherResponse.sys.Country != "")
                    {
                        Weather weather = new Weather(weatherResponse);
                        weathers.Add(weather);
                        weather.print();
                        n++;

                    }
                }
            }
            Console.WriteLine();
            //var linqMaxTemp = weathers.MaxBy(weather => weather.Temp).Сountry;
            var linqMaxTemp = weathers.OrderByDescending(weather => weather.Temp).First();
            Console.WriteLine($"страна с самиой высокой температурой: {linqMaxTemp.Country}");
            //var linqMinTemp = weathers.MinBy(weather => weather.Temp).Country;
            var linqMinTemp = weathers.OrderByDescending(weather => weather.Temp).Last();
            Console.WriteLine($"страна с самиой низкой температурой: {linqMinTemp.Country}");

            var linqCountry = weathers.Select(weather => weather.Country).Distinct().ToList();
            Console.WriteLine($"уникальных стран: {linqCountry.Count}");

            var linqAverTemp = weathers.Average(weather => weather.Temp);
            Console.WriteLine($"средняя температура в мире: {linqAverTemp}");

            var linqDescription = weathers.FirstOrDefault(weather => weather.Description == "clear sky" ||
                                  weather.Description == "rain" ||
                                  weather.Description == "few clouds");
            Console.WriteLine($"Первая страна с определёнными значениями Description: {linqDescription.Country}, " +
                $"{linqDescription.Name}, " +
                $"discr: {linqDescription.Description}");






        }

        public struct Weather
        {
            public string Country { get; set; }
            public string Name { get; set; }
            public double Temp { get; set; }
            public string Description { get; set; }

            public Weather(WeatherResponce weatherResponce)
            {
                this.Country = weatherResponce.sys.Country;
                this.Name = weatherResponce.Name;
                this.Temp = weatherResponce.main.Temp;
                this.Description = weatherResponce.weather[0].Description;

            }
            public void print()
            {
                Console.WriteLine($"Country:{Country}, Name: {Name}, Temp: {Temp}, Discription: {Description}");
            }
        }

        public class WeatherResponce
        {
            public string Name { get; set; }
            public TemperatureInfo main { get; set; }
            public CountryInfo sys { get; set; }
            public DescriptionInfo[] weather { get; set; }

        }

        public class TemperatureInfo
        {
            public float Temp { get; set; }

        }

        public class CountryInfo
        {

            public string Country { get; set; }
        }

        public class DescriptionInfo
        {

            public string Description { get; set; }

        }
    }
}
