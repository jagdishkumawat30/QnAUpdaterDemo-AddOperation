// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.9.2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using QnAUpdaterDemo.Dialogs.Operations;

namespace QnAUpdaterDemo.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger Logger;
        protected readonly IConfiguration Configuration;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(ILogger<MainDialog> logger, IConfiguration configuration)
            : base(nameof(MainDialog))
        {
            Logger = logger;
            Configuration = configuration;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new AddDialog(configuration));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text("What operation you would like to perform?"), cancellationToken);

            List<string> operationList = new List<string> { "Add", "Update", "Delete" };
            // Create card
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                // Use LINQ to turn the choices into submit actions
                Actions = operationList.Select(choice => new AdaptiveSubmitAction
                {
                    Title = choice,
                    Data = choice,  // This will be a string
                }).ToList<AdaptiveAction>(),
            };
            // Prompt
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    // Convert the AdaptiveCard to a JObject
                    Content = JObject.FromObject(card),
                }),
                Choices = ChoiceFactory.ToChoices(operationList),
                // Don't render the choices outside the card
                Style = ListStyle.None,
            },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Operation"] = ((FoundChoice)stepContext.Result).Value;
            string operation = (string)stepContext.Values["Operation"];

            if (operation.Equals("Add"))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("You selected Add Operation. Please provide following details to add a new question answer pair."), cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(AddDialog), new User(), cancellationToken);
            }
            else if (operation.Equals("Update"))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("You selected Update Operation. Please provide following details to update the existing question answer pair."), cancellationToken);
                
            }
            else if (operation.Equals("Delete"))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("You selected Delete Operation. Please provide following details to delete the existing question answer pair."), cancellationToken);
                
            }
            else
            {

            }
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            QnAData.QuestionPhrase.Clear();
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
