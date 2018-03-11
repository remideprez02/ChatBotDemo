using ChatbotApp.Models.Luis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Web;
using ChatbotApp.Models.Qna;
using Newtonsoft.Json;

namespace ChatbotApp.Services.CognitiveServices
{
    public class LuisService
    {
        private readonly string _luisAppId = ConfigurationManager.AppSettings["LuisAppId"];
        private readonly string _luisSecretKey = ConfigurationManager.AppSettings["LuisSecretKey"];
        public async Task<Answer> GetAnswerFromLuis(string utterance)
        {
            try
            {
                var result = await CallLuis(utterance);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new Answer();
            }
        }

        public async Task<Answer> GetLuisResult(string utterance)
        {
            try
            {
                var result = await CallLuis(utterance);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new Answer();
            }
        }

        public async Task<Answer> CallLuis(string utterance)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _luisSecretKey);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["q"] = utterance;

            // These optional request parameters are set to their default values
            queryString["timezoneOffset"] = "0";
            //See All Intents with verbose = true
            queryString["verbose"] = "true";
            queryString["spellCheck"] = "false";
            queryString["staging"] = "false";

            var uri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/" + _luisAppId + "?" + queryString;
            var response = await client.GetAsync(uri);

            var strResponseContent = response.Content.ReadAsStringAsync().Result;
            //var json = JsonConvert.SerializeObject(strResponseContent);
            var luisResponse = JsonConvert.DeserializeObject<Answer>(strResponseContent);
            return luisResponse;
        }
    }

}
