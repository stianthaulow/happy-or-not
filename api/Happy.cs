using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Linq;

namespace Thaulow
{
    public static class happy
    {
        [FunctionName("happy")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            IFormFile image;
            try
            {
                var formdata = await req.ReadFormAsync();
                image = req.Form.Files[0];
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }

            IFaceClient client = GetFaceClient();

            double happiness = 0;

            using (var imageStream = new MemoryStream())
            {
                await image.CopyToAsync(imageStream);
                imageStream.Position = 0;
                var faces = await client.Face.DetectWithStreamAsync(imageStream, false, false, new FaceAttributeType[] { FaceAttributeType.Emotion });
                var firstFace = faces.FirstOrDefault();
                happiness = firstFace.FaceAttributes.Emotion.Happiness;
            }

            string message = happiness > 0.5 ? "happy" : "not happy";

            return new OkObjectResult($"You are {message}");
        }

        private static IFaceClient GetFaceClient()
        {
            var key = System.Environment.GetEnvironmentVariable("cognitive_services_key");
            var endpoint = System.Environment.GetEnvironmentVariable("cognitive_services_endpoint");

            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
    }
}