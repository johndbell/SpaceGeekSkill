using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SpaceGeek
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            var response = new SkillResponse();
            response.Response = new ResponseBody();
            response.Response.ShouldEndSession = false;
            PlainTextOutputSpeech innerResponse = null;
            var log = context.Logger;

            var allResources = GetResources();
            var resource = allResources.FirstOrDefault();

            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                log.LogLine("Default LaunchRequest made: Alexa, open Science Facts");
                innerResponse = new PlainTextOutputSpeech();
                innerResponse.Text = emitNewFact(resource, true);
            }
            else if (input.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = input.Request as IntentRequest;
                innerResponse = new PlainTextOutputSpeech();
                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                    case "AMAZON.StopIntent":
                        log.LogLine($"{intentRequest.Intent.Name}: send StopMessage");
                        innerResponse.Text = resource.StopMessage;
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        log.LogLine("AMAZON.HelpIntent: send HelpMessage");
                        innerResponse.Text = resource.HelpMessage;
                        break;
                    case "GetFactIntent":
                    case "GetNewFactIntent":
                        log.LogLine($"{intentRequest.Intent.Name}: send new fact");
                        innerResponse.Text = emitNewFact(resource, false);
                        break;
                    default:
                        log.LogLine($"Unknown Intent: {intentRequest.Intent.Name}");
                        innerResponse.Text = resource.HelpReprompt;
                        break;

                }
            }

            response.Response.OutputSpeech = innerResponse as IOutputSpeech;
            response.Version = "1.0";
            return response;
        }

        public List<FactResource> GetResources()
        {
            List<FactResource> resources = new List<FactResource>();
            FactResource enUSResource = new FactResource("en-US");
            enUSResource.SkillName = "American Space Facts";
            enUSResource.GetFactMessage = "Here's your fact: ";
            enUSResource.HelpMessage = "You can say tell me a space fact, or, you can say exit... What can I help you with?";
            enUSResource.HelpReprompt = "What can I help you with?";
            enUSResource.StopMessage = "Goodbye!";
            enUSResource.Facts.Add("A year on Mercury is just 88 days long.");
            enUSResource.Facts.Add("Despite being farther from the Sun, Venus experiences higher temperatures than Mercury.");
            enUSResource.Facts.Add("Venus rotates counter-clockwise, possibly because of a collision in the past with an asteroid.");
            enUSResource.Facts.Add("On Mars, the Sun appears about half the size as it does on Earth.");
            enUSResource.Facts.Add("Earth is the only planet not named after a god.");
            enUSResource.Facts.Add("Jupiter has the shortest day of all the planets.");
            enUSResource.Facts.Add("The Milky Way galaxy will collide with the Andromeda Galaxy in about 5 billion years.");
            enUSResource.Facts.Add("The Sun contains 99.86% of the mass in the Solar System.");
            enUSResource.Facts.Add("The Sun is an almost perfect sphere.");
            enUSResource.Facts.Add("A total solar eclipse can happen once every 1 to 2 years. This makes them a rare event.");
            enUSResource.Facts.Add("Saturn radiates two and a half times more energy into space than it receives from the sun.");
            enUSResource.Facts.Add("The temperature inside the Sun can reach 15 million degrees Celsius.");
            enUSResource.Facts.Add("The Moon is moving approximately 3.8 cm away from our planet every year.");

            resources.Add(enUSResource);
            return resources;
        }

        public string emitNewFact(FactResource resource, bool withPreface)
        {
            Random r = new Random();
            if (withPreface)
                return resource.GetFactMessage +
                       resource.Facts[r.Next(resource.Facts.Count)];
            return resource.Facts[r.Next(resource.Facts.Count)];
        }
    }
}
