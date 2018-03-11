using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Newtonsoft.Json;

namespace ChatbotApp.Models
{
    public class TextAnalyticsSentimentModel
    {
        public List<TextAnalyticsDocumentSentimentModel> Documents { get; set; }
    }

    public class TextAnalyticsDocumentSentimentModel
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("text")] public string Text { get; set; }
        [JsonProperty("language")] public string Language { get; set; }
        [JsonProperty("score")] public double? Score { get; set; }
        public KeyPhraseBatchResult KeyPhraseBatchResult { get; set; }
        public LanguageBatchResult LanguageBatchResult { get; set; }
        public string KeyWords { get; set; }
        public string DetectedLanguage { get; set; }
    }
}