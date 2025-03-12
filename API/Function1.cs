using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAI.Images;
using Shared;
using System.ClientModel;

namespace API
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("generate")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestData = JsonConvert.DeserializeObject<ImageRequest>(requestBody);
            if (requestData == null)
                return new BadRequestObjectResult("Invalid request payload");

            var azureOpenAiEndpoint = Environment.GetEnvironmentVariable("AzureOpenAiEndpoint");
            var azureOpenAiKey = Environment.GetEnvironmentVariable("AzureOpenAiKey");
            var azureOpenAiChatClient = "dall-e-3";

            var imageClient = new AzureOpenAIClient(new Uri(azureOpenAiEndpoint), new ApiKeyCredential(azureOpenAiKey))
                        .GetImageClient(azureOpenAiChatClient);

            var options = new ImageGenerationOptions()
            {
                Size = GeneratedImageSize.W1792xH1024,
            };

            var imageGenResponse = await imageClient.GenerateImageAsync(requestData.ImageDescription, options);

            ImageResponse response = new ImageResponse()
            {
                ImageSource = imageGenResponse.Value.ImageUri
            };
            return new OkObjectResult(response);
        }
    }
}
