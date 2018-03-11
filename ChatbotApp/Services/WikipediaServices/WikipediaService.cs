using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WikipediaNET;
using WikipediaNET.Enums;
using WikipediaNET.Objects;

namespace ChatbotApp.Services.WikipediaServices
{
    public class WikipediaService
    {
        public async Task<QueryResult> GetWikipediaInformations(string utterance)
        {
            var result = await CallWikipedia(utterance);
            return result;
        }

        public async Task<QueryResult> CallWikipedia(string utterance)
        {
            //Default language is English
            var wikipedia = new Wikipedia
            {
                UseTLS = true,
                Limit = 5,
                What = What.Text,
                Language = Language.French
            };

            var searchText = utterance;
            var results = wikipedia.Search(searchText);
            return results;
        }
    }
}