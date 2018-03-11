using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ChatbotApp.Models;
using ChatbotApp.Models.Luis;
using ChatbotApp.Models.OpenWeatherMap;
using ChatbotApp.Services.WikipediaServices;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Entity = ChatbotApp.Models.Luis.Entity;

namespace ChatbotApp.Dialogs
{

    [Serializable]
    public class LuisDialog : IDialog<object>
    {
        private readonly Answer _luisAnswer;
        private readonly TextAnalyticsDocumentSentimentModel _textAnalyticsDocumentSentimentModel;
        private readonly string _utterance;

        public LuisDialog(Answer luisAnswer, TextAnalyticsDocumentSentimentModel textAnalyticsDocumentSentimentModel, string utterance)
        {
            _luisAnswer = luisAnswer;
            _textAnalyticsDocumentSentimentModel = textAnalyticsDocumentSentimentModel;
            _utterance = utterance;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await HandleLuisResult(context);
        }

        private async Task HandleLuisResult(IDialogContext context)
        {
            if (_textAnalyticsDocumentSentimentModel != null)
            {
                var helper = new Helpers.SomeHelpers();
                var heroCard = new HeroCard
                {
                    Title = "Voici ce que j'ai trouvé",
                    Subtitle = "",
                    Text = "",
                    Images = new List<CardImage>
                            {
                                new CardImage(helper.GetImgByScore(_textAnalyticsDocumentSentimentModel.Score)),
                            },
                    Buttons = new List<CardAction>()
                };

                var message = context.MakeMessage();
                if (_textAnalyticsDocumentSentimentModel?.Score != null)
                {
                    var txtAnalyticsResult =
                        $@"Score de sentiment :  {
                            _textAnalyticsDocumentSentimentModel?.Score.Value
                        :0.0}  KeyWords : {_textAnalyticsDocumentSentimentModel.KeyWords} Langage : {
                            _textAnalyticsDocumentSentimentModel.DetectedLanguage
                        }";
                    message.Text = "Voici ce que j'ai trouvé :";

                    if (_luisAnswer.TopScoringIntent.Intent == "GetWeather")
                    {
                        var openWeatherMapService = new Services.WeatherServices.OpenWeatherMapService();
                        var city = _luisAnswer.Entities?.FirstOrDefault(x => x.Type == "City")?.entity;

                        if (city != null)
                        {
                            var weatherQuery = new OpenWeatherMapQueryModel();
                            weatherQuery.Location = city;

                            var response = await openWeatherMapService.GetWeatherAsync(weatherQuery);
                            var minTemp = 0;
                            var maxTemp = 0;
                            if (response.Main.MinTemperature != null && response.Main.MaxTemperature != null)
                            {
                                minTemp = (int) response.Main.MinTemperature;
                                maxTemp = (int) response.Main.MaxTemperature;
                            }

                            if (minTemp == maxTemp)
                            {
                                if (response.Main.MaxTemperature != null)
                                    if (_textAnalyticsDocumentSentimentModel.Score != null)
                                        heroCard.Text = "A " + response.Name + " , le temps est " +
                                                        response.Weather.FirstOrDefault()?.Description +
                                                        " avec une température de "
                                                        + response.Main.MaxTemperature.Value.ToString("00.0") +
                                                        " °";
                                heroCard.Title = "Voici la météo ";
                                message.Attachments.Add(heroCard.ToAttachment());
                                await context.PostAsync(txtAnalyticsResult);
                                await context.PostAsync(message);
                            }
                            else
                            {
                                if (response.Main.MinTemperature != null)
                                    if (response.Main.MaxTemperature != null)
                                        heroCard.Text = "A " + response.Name + " , le temps est " +
                                                        response.Weather.FirstOrDefault()?.Description +
                                                        " avec des températures comprises entre "
                                                        + response.Main.MinTemperature.Value.ToString("00.0") +
                                                        " et " +
                                                        response.Main.MaxTemperature.Value.ToString("00.0") +
                                                        " °";
                                heroCard.Title = "Voici la météo ";
                                message.Attachments.Add(heroCard.ToAttachment());
                                await context.PostAsync(txtAnalyticsResult);
                                await context.PostAsync(message);
                            }


                        }
                    }

                    //if (_luisAnswer.TopScoringIntent.Intent == "Greetings")
                        //{
                        //    if (_textAnalyticsDocumentSentimentModel.Score != null)
                        //        heroCard.Text = "Bonjour ! Enchanté :) ";
                        //    message.Text += message.Text + _textAnalyticsDocumentSentimentModel.Score.Value.ToString("0.0");
                        //    message.Attachments.Add(heroCard.ToAttachment());
                        //        await context.PostAsync(message);
                        //}

                        if (_luisAnswer.TopScoringIntent.Intent == "GetWikipediaInformations")
                        {
                            var wikipediaService = new WikipediaService();
                            var location = _luisAnswer.Entities?.FirstOrDefault(x => x.Type == "Location")?.entity;
                            var subject = _utterance.Split(new[] {"histoire"}, StringSplitOptions.None)[1];
                            var result = wikipediaService.GetWikipediaInformations(subject).Result.Search;

                            foreach (var item in result)
                            {
                                heroCard.Buttons.Add(new CardAction
                                    {
                                        Value = item.Url,
                                        Type = "openUrl",
                                        Title = item.Url.AbsoluteUri
                                    }
                                );
                            }

                            message.Text = "";
                            message.Attachments.Add(heroCard.ToAttachment());
                            await context.PostAsync(txtAnalyticsResult);
                            await context.PostAsync(message);
                        }
                    if (_luisAnswer.TopScoringIntent.Intent == "GetAppreciation")
                    {
                        heroCard.Title = "Tout le plaisir est pour moi :)";
                        message.Attachments.Add(heroCard.ToAttachment());
                        await context.PostAsync(txtAnalyticsResult);
                        await context.PostAsync(message);
                    }
                    if (_luisAnswer.TopScoringIntent.Intent == "GetOpinion")
                    {
                        heroCard.Title = "Je vais te donner mon avis sur la question :)";
                        message.Attachments.Add(heroCard.ToAttachment());
                        await context.PostAsync(txtAnalyticsResult);
                        await context.PostAsync(message);
                    }
                }
            }
            context.Done<object>(null);
        }
    }
}