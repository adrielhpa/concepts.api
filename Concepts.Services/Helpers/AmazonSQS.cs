using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Concepts.Domain.DTOs;
using Concepts.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Concepts.Services.Helpers
{
    public class AmazonSQS
    {
        public async Task SendMessage(ResponseData response)
        {
            var accessKey = Environment.GetEnvironmentVariable("AmazonAccessKey");
            var secretKey = Environment.GetEnvironmentVariable("AmazonSecretKey");
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var client = new AmazonSQSClient(credentials, RegionEndpoint.USEast1);

            var request = new SendMessageRequest()
            {
                QueueUrl = "https://sqs.us-east-1.amazonaws.com/473698569966/conceptsQueue",
                MessageBody = JsonSerializer.Serialize(response)
            };

            await client.SendMessageAsync(request);
        }
    }
}
