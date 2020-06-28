using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QnAUpdaterDemo.Utilities
{
    public static class QnAClient
    {

        // <UpdateKBMethod>
        public static async Task UpdateKB(IQnAMakerClient client, string kbId, string answer)
        {

            var updateOp = await client.Knowledgebase.UpdateAsync(kbId, new UpdateKbOperationDTO
            {
                // Create JSON of changes
                Add = new UpdateKbOperationDTOAdd
                {
                    QnaList = new List<QnADTO> {
                        new QnADTO {
                            Questions = QnAData.QuestionPhrase,
                            Answer = answer,
                        }
                    },
                },
                Update = null,
                Delete = null
            }); ;

            // Loop while operation is success
            updateOp = await MonitorOperation(client, updateOp);
        }
        // </UpdateKBMethod>

        // <PublishKB>
        public static async Task PublishKb(IQnAMakerClient client, string kbId)
        {
            await client.Knowledgebase.PublishAsync(kbId);
        }
        // </PublishKB>

        // <MonitorOperation>
        private static async Task<Operation> MonitorOperation(IQnAMakerClient client, Operation operation)
        {
            // Loop while operation is success
            for (int i = 0;
                i < 20 && (operation.OperationState == OperationStateType.NotStarted || operation.OperationState == OperationStateType.Running);
                i++)
            {
                Console.WriteLine("Waiting for operation: {0} to complete.", operation.OperationId);
                await Task.Delay(5000);
                operation = await client.Operations.GetDetailsAsync(operation.OperationId);
            }

            if (operation.OperationState != OperationStateType.Succeeded)
            {
                throw new Exception($"Operation {operation.OperationId} failed to completed.");
            }
            return operation;
        }
        // </MonitorOperation>
    }
}
