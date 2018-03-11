using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using ChatbotApp.Models.OpenWeatherMap;
using Newtonsoft.Json;

namespace ChatbotApp.Services.WeatherServices
{
    public class OpenWeatherMapService
    {
        private const string OpenWeatherBaseurl = "http://api.openweathermap.org/data/2.5/";

        private const string ImgBaseurl = "http://openweathermap.org/img/w/";

        private const string Query = "/data/2.5/weather?q={0}&units={1}&lang={2}";

        private readonly string _openWeatherApiKey = ConfigurationManager.AppSettings["OpenWeatherAPIKey"];

        public async Task<WeatherInfo> GetWeatherAsync(OpenWeatherMapQueryModel weatherQuery)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(OpenWeatherBaseurl);
                httpClient.DefaultRequestHeaders.Add("x-api-key", _openWeatherApiKey);
                string url = string.Format(Query, weatherQuery.Location, "Metric", "fr");
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.Culture = new System.Globalization.CultureInfo("fr-FR");
                    var result = JsonConvert.DeserializeObject<WeatherInfo>(json, settings);
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        public string GetWeatherIcon(WeatherInfo weatherInfo)
        {
            string weatherIcon = "";
            weatherInfo.Weather.ForEach(w => { weatherIcon = w.Icon; });
            return ImgBaseurl + $"{weatherIcon}.png";
        }
    }
}