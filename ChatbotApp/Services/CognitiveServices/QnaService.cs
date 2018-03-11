using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using ChatbotApp.Models.Qna;
using Newtonsoft.Json;

namespace ChatbotApp.Services.CognitiveServices
{
    public class QnaService 
    {
        private readonly string _qnAMakerknowLedgeBaseId = ConfigurationManager.AppSettings["QnAMakerKnowledgeBaseID"];
        private readonly string _qnAMakerSubscriptionKey = ConfigurationManager.AppSettings["QnAMakerSubscriptionKey"];
        public async Task<QnaAnswerModel> GetAnswerFromQnA(string utterance)
        {
            try
            {
                var result = await Task.Run(() => CallQnA(utterance));
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new QnaAnswerModel();
            }
        }

        private static string CleanStringEncoding(string str)
        {
            var res = str;

            List<(string o, string n)> replacements = new List<(string o, string n)>
            {
                ("&#224;", "à"),
                ("&#226;", "â"),
                ("&#231;", "ç"),
                ("&#232;", "è"),
                ("&#233;", "é"),
                ("&#234;", "ê"),
                ("&#249;", "ù")
            };

            foreach (var on in replacements)
            {
                res = res.Replace(on.o, on.n);
            }
            return res;
        }

        public QnaAnswerModel CallQnA(string utterance)
        {
            var httpClient = new HttpClient();

            //string url = $"https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/{ System.Configuration.ConfigurationManager.AppSettings["QnAMakerKnowledgeBaseID"] }/generateAnswer";
            var url = $"https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/{ _qnAMakerknowLedgeBaseId }/generateAnswer";

            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _qnAMakerSubscriptionKey);

            var query = new QnaQueryModel { Question = utterance, Top = 1 };

            try
            {
                var response = httpClient.PostAsJsonAsync(new Uri(url), query).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    var qaresponse = JsonConvert.DeserializeObject<QnaResponseModel>(result);
                    var bestResult = qaresponse.Answers[0];

                    //bestResult.answer = cleanStringEncoding(bestResult.answer);

                    Debug.WriteLine($"Score : {bestResult.Score} Answer : {bestResult.Answer} Text : {utterance}");

                    bestResult.Answer = CleanStringEncoding(bestResult.Answer);

                    return bestResult;
                }
                else
                {
                    throw new Exception($"Error communicating with QnA application :\nServer error (HTTP {response.StatusCode}: {response.ReasonPhrase}).");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error in the QnA Service.", e);
            }
        }
    }
}