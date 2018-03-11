using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Luis;

namespace ChatbotApp.Models.Luis
{
    public class Answer
    {
        public string Query { get; set; }
        public TopScoringIntent TopScoringIntent { get; set; }
        public List<Intent> Intents { get; set; }
        public List<Entity> Entities { get; set; }
    }
    public class Entity
    {
        public string entity { get; set; }
        public string Type { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public double Score { get; set; }
        public Resolution Resolution { get; set; }
    }
    public class Intent
    {
        public string intent { get; set; }
        public double Score { get; set; }
    }
    public class Query
    {
        public string Utterance { get; set; }
    }
    public class Resolution
    {
        public List<Value> Values { get; set; }
    }
    public class TopScoringIntent
    {
        public string Intent { get; set; }
        public double Score { get; set; }

        public TopScoringIntent()
        {
            this.Score = 0.0f;
        }
    }
    public class Value
    {
        public string Timex { get; set; }
        public string Type { get; set; }
        public string value { get; set; }
    }
}