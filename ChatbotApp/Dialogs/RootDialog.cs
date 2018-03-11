using System;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;

namespace ChatbotApp.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(BotLogic);

            return Task.CompletedTask;
        }

        private async Task BotLogic(IDialogContext context, IAwaitable<object> result)
        {
            if (await result is Activity activity && activity.Text != string.Empty)
            {
                var reply = activity.CreateReply(String.Empty);
                reply.Type = ActivityTypes.Typing;
                await context.PostAsync(reply);

                var userMessage = activity.Text;

                //Récupération du service QnaMaker
                var qnaService = new Services.CognitiveServices.QnaService();

                //Utilisation du service et récupération de la réponse
                var qnaResponse = await qnaService.GetAnswerFromQnA(userMessage);

                //Récupération du service Luis
                var luisService = new Services.CognitiveServices.LuisService();

                //Utilisation du service et récupération de la réponse
                var luisResponse = await luisService.GetAnswerFromLuis(userMessage);

                //Récupération du service TextAnalytics
                var textAnalyticsService = new Services.CognitiveServices.TextAnalyticsService();

                //Utilisation du service et récupération de la réponse
                var testAnalyticsResponse = await textAnalyticsService.GetTextAnalyticsSentiments(userMessage);

                //Test du scorable
                if (luisResponse.TopScoringIntent.Intent != "None" && qnaResponse.Score < 70 || luisResponse.TopScoringIntent.Intent == "GetOpinion" && luisResponse.TopScoringIntent.Score < 0.7 || luisResponse.TopScoringIntent.Intent == "Greetings")
                {

                    if (luisResponse.TopScoringIntent.Score > 0.6 && qnaResponse.Score < 70 || qnaResponse.Score > 100)
                    {
                        context.Call(new LuisDialog(luisResponse, testAnalyticsResponse, userMessage), BotLogic);
                    }

                    if (qnaResponse.Score > 70 && luisResponse.TopScoringIntent.Score < 0.7 ||
                        luisResponse.TopScoringIntent.Score > 1)
                    {
                        context.Call(new QnaDialog(qnaResponse, testAnalyticsResponse), BotLogic);
                    }

                    if (qnaResponse.Score > 70 && qnaResponse.Score <= 100 &&
                        luisResponse.TopScoringIntent.Score > 0.7 && luisResponse.TopScoringIntent.Score <= 1)
                    {
                        if (qnaResponse.Score > luisResponse.TopScoringIntent.Score)
                        {
                            context.Call(new QnaDialog(qnaResponse, testAnalyticsResponse), BotLogic);
                        }
                        else
                        {
                            context.Call(new LuisDialog(luisResponse, testAnalyticsResponse, userMessage), BotLogic);
                        }
                    }

                    if (qnaResponse.Score < 50 && luisResponse.TopScoringIntent.Score < 0.6 ||
                        luisResponse.TopScoringIntent.Intent == "None")
                    {
                        await context.PostAsync("Et là c'est le bug ! Peux-tu répéter ?");
                        context.Call(new TextAnalyticsDialog(testAnalyticsResponse), BotLogic);
                    }
                }
                else
                {
                    await context.PostAsync("Et là c'est le bug ! Peux-tu répéter ? Je vais te donner mon ressentiment sur ton message ");
                    context.Call(new TextAnalyticsDialog(testAnalyticsResponse), BotLogic);
                }
            }
            else
            {
                context.Wait(BotLogic);
            }
        }
    }
}