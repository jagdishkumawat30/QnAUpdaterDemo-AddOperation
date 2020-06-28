using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QnAUpdaterDemo.Dialogs.Operations
{
    public class QuestionPhraseDialog : ComponentDialog
    {
        public QuestionPhraseDialog() : base(nameof(QuestionPhraseDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                QuestionStepAsync,
                ConfirmStepAsync,
                SummaryStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> QuestionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter the question phrase.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Phrase"] = (string)stepContext.Result;
            QnAData.QuestionPhrase.Add((string)stepContext.Values["Phrase"]);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Would you like to Add more phrases for your question?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var promptMessage = "What else can I do for you?";
                return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Ok."));
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
