using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ChatbotApp.Models;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

namespace ChatbotApp.Services.CognitiveServices
{
    public class TextAnalyticsService
    {
        private readonly string _textAnalyticsSubscriptionKey = ConfigurationManager.AppSettings["TextAnalyticsSubscriptionKey"];
        public async Task<TextAnalyticsDocumentSentimentModel> GetTextAnalyticsSentiments(string utterance)
        {
            var result = await CallTextAnalytics(utterance);
            return result;
        }

        public async Task<TextAnalyticsDocumentSentimentModel> CallTextAnalytics(string utterance)
        {
            // Create a client.
            ITextAnalyticsAPI client = new TextAnalyticsAPI();
            client.AzureRegion = AzureRegions.Westeurope;
            client.SubscriptionKey = _textAnalyticsSubscriptionKey;

            var keysList = "";

            var language = client.DetectLanguage(
                new BatchInput(
                    new List<Input>()
                    {
                        new Input("0", utterance)
                    }));

            foreach (var document in language.Documents)
            {
                Console.WriteLine("Document ID: {0} , Language: {1}", document.Id, document.DetectedLanguages[0].Name);
            }

            var lang = language.Documents.FirstOrDefault()?.DetectedLanguages.FirstOrDefault()?.Iso6391Name;

            var keys = client.KeyPhrases(
                new MultiLanguageBatchInput(
                    new List<MultiLanguageInput>()
                    {
                        new MultiLanguageInput(lang, "0", utterance),
                    }));

            var sentiment = client.Sentiment(new MultiLanguageBatchInput(new List<MultiLanguageInput>()
            {
                new MultiLanguageInput(lang, "0", utterance)
            }));

            //Si les sentiments sont nulls, on renvoie un objet vide
            if (sentiment.Documents == null) return new TextAnalyticsDocumentSentimentModel();
            {
                var document = new TextAnalyticsDocumentSentimentModel
                {
                    Text = utterance,
                    Score = sentiment.Documents.FirstOrDefault(x => x.Id == "0")?.Score,
                    Id = sentiment.Documents.FirstOrDefault()?.Id,
                    Language = lang
                };

                if (keys.Documents != null)
                {
                    foreach (var item in keys.Documents.SelectMany(x => x.KeyPhrases).ToList())
                    {
                        document.KeyWords += item;
                    }
                }

                if (language.Documents == null) return document;
                {
                    foreach (var item in language.Documents.SelectMany(x => x.DetectedLanguages).ToList())
                    {
                        document.DetectedLanguage += item.Name;
                    }
                }

                return document;
            }
        }
    }
}