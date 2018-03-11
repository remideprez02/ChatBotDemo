using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatbotApp.Models;
using ChatbotApp.Models.Qna;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ChatbotApp.Dialogs
{
    [Serializable]
    public class QnaDialog : IDialog<object>
    {
        private readonly QnaAnswerModel _qnaAnswerModel;
        private readonly TextAnalyticsDocumentSentimentModel _textAnalyticsDocumentSentimentModel;

        public QnaDialog(QnaAnswerModel qnaAnswerModel, TextAnalyticsDocumentSentimentModel textAnalyticsDocumentSentimentModel)
        {
            _qnaAnswerModel = qnaAnswerModel;
            _textAnalyticsDocumentSentimentModel = textAnalyticsDocumentSentimentModel;
        }

        public async Task StartAsync(IDialogContext context)
        {
            var helper = new Helpers.SomeHelpers();
            if (_textAnalyticsDocumentSentimentModel?.Score != null)
            {
                var txtAnalyticsResult =
                    $@"Score de sentiment :  {_textAnalyticsDocumentSentimentModel?.Score.Value:0.0}  KeyWords : {
                            _textAnalyticsDocumentSentimentModel.KeyWords
                        } Langage : {_textAnalyticsDocumentSentimentModel.DetectedLanguage}";


                var heroCard = new HeroCard
                {
                    Title = "",
                    Subtitle = "",
                    Text = "",
                    Images = new List<CardImage>
                    {
                        new CardImage(helper.GetImgByScore(_textAnalyticsDocumentSentimentModel.Score)),
                    },
                    Buttons = new List<CardAction>()
                };
                heroCard.Text = txtAnalyticsResult;
                var message = context.MakeMessage();
                message.Attachments.Add(heroCard.ToAttachment());

                await context.PostAsync(_qnaAnswerModel.Answer);
                if (_textAnalyticsDocumentSentimentModel.Score != null)
                    await context.PostAsync(message);
            }

            context.Done<object>(null);
        }
    }
}