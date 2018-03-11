using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatbotApp.Models.OpenWeatherMap
{
    public class OpenWeatherMapQueryModel
    {
        public string Location { get; set; }

        public string Datetime { get; set; }
    }
}