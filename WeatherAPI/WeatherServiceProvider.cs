using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeatherAPI
{
    public class WeatherServiceProvider : IWeatherService
    {
        HttpClient _client;
        private const string apiKey = "d7139c55a4b84860819154757223012";
        public WeatherServiceProvider()
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri("https://api.worldweatheronline.com/premium/v1/weather.ashx")
            };
        }
        public async Task<string> GetTemperature(string city, DateTime dateTime)
        {
            string parameters = "?q=" + city.Replace(" ", "+");

            if(dateTime.AddDays(-7)>DateTime.Now)
            {
                dateTime = DateTime.Now.AddDays(7);
            }
            parameters += "&date=" + dateTime.ToString("yyyy-MM-dd");
            parameters += "&key=" + apiKey;
            var response = await _client.GetAsync(parameters);
            if (response.IsSuccessStatusCode)
            {
                string xml = await response.Content.ReadAsStringAsync();
                string content = Regex.Match(xml, @"<temp_C>(.+)<\/temp_C>").Groups[1].Value;
                return content;
            }
            return "";
        }
    }
}
