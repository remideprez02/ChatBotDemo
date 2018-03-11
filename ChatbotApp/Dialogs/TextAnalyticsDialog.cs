using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatbotApp.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ChatbotApp.Dialogs
{
    [Serializable]
    public class TextAnalyticsDialog : IDialog<object>
    {
        private readonly TextAnalyticsDocumentSentimentModel _textAnalyticsDocumentSentimentModel;

        public TextAnalyticsDialog(TextAnalyticsDocumentSentimentModel textAnalyticsDocumentSentimentModel)
        {
            _textAnalyticsDocumentSentimentModel = textAnalyticsDocumentSentimentModel;
        }
        public async Task StartAsync(IDialogContext context)
        {
            var helper = new Helpers.SomeHelpers();

            var heroCard = new HeroCard
            {
                Title = "Voici mon ressentiment sur le message",
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
                            _textAnalyticsDocumentSentimentModel?.Score.Value:0.0}  KeyWords : {_textAnalyticsDocumentSentimentModel.KeyWords} Langage : {
                            _textAnalyticsDocumentSentimentModel.DetectedLanguage
                        }";
                await context.PostAsync(txtAnalyticsResult);
            }
            message.Attachments.Add(heroCard.ToAttachment());
            await context.PostAsync(message);

            context.Done<object>(null);
        }
    }
}