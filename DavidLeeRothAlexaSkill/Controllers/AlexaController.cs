﻿using System;
using AlexaSkill.Data;
using DavidLeeRothAlexaSkill.Configuration;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DavidLeeRothAlexaSkill.Controllers
{
    public class DavidLeeRothController : Controller
    {
        private static string[] RothResponsePrefixes = {
            "Dave says ",
            " "
        };

        private static string[] RothResponses = {
            "bopx.mp3",
            "bosdibodiboppx.mp3",
            "r2x.mp3",
            "r3x.mp3",
            "r4x.mp3",
            "whosaidthatx.mp3"
        };

        private static Random GlobalRandom = new Random(Guid.NewGuid().GetHashCode());
        private string randomRothResponsePrefix
        {
            get
            {
                var rand = new Random(DavidLeeRothController.GlobalRandom.Next());
                return DavidLeeRothController.RothResponsePrefixes[rand.Next(0, DavidLeeRothController.RothResponsePrefixes.Length - 1)];
            }
        }

        private string randomRothResponse
        {
            get
            {
                var rand = new Random(DavidLeeRothController.GlobalRandom.Next());
                return DavidLeeRothController.RothResponses[rand.Next(0, DavidLeeRothController.RothResponses.Length - 1)];
            }
        }

        private AlexaSkillConfiguration alexaSkillConfiguration;

        public DavidLeeRothController(IOptions<AlexaSkillConfiguration> alexaSkillConfiguration)
        {
            this.alexaSkillConfiguration = alexaSkillConfiguration.Value;
        }

        [Route("api/alexa")]
        [HttpGet]
        public IActionResult Hello()
        {
            return Ok();
        }

        [Route("api/alexa")]
        [HttpPost]
        public IActionResult GiveMeABottleOfAnythingAndAGlazedDonut([FromBody] AlexaRequest request)
        {
            if(request.Session.Application.ApplicationId != this.alexaSkillConfiguration.ApplicationId)
            {
                return BadRequest();
            }

            AlexaResponse response = null;

            switch (request.Request.Type)
            {
                case "IntentRequest":
                    response = this.IntentRequestHandler(request);
                    break;
                case "LaunchRequest":
                    response = this.LaunchRequestHandler(request);
                    break;
                case "SessionEndedRequest":
                    response = this.SessionEndedRequestHandler(request);
                    break;
            }

            return Ok(response);
        }

        private AlexaResponse IntentRequestHandler(AlexaRequest request)
        {
            AlexaResponse response = null;

            switch (request.Request.Intent.Name)
            {
                case "SayIntent":
                    response = this.SayIntentHandler(request);
                    break;
                case "AMAZON.HelpIntent":
                    response = this.HelpIntentHandler(request);
                    break;
            }

            return response;
        }

        private AlexaResponse HelpIntentHandler(AlexaRequest request)
        {
            return new AlexaResponse("Ask David Lee Roth to melt your face");
        }

        private AlexaResponse SayIntentHandler(AlexaRequest request)
        {
            var baseUrl = $"https://{this.Request.Host}";
            var ssmlResponse = $"<speak> <audio src=\"{baseUrl}/Sounds/{this.randomRothResponse}\"></audio> </speak>";

            var response = new AlexaResponse();
            response.Response.OutputSpeech.Type = "SSML";
            response.Response.OutputSpeech.Ssml = ssmlResponse;
            response.Response.Card.Content = "Awwwww yeah!";

            return response;
        }

        private AlexaResponse LaunchRequestHandler(AlexaRequest request)
        {
            var response = new AlexaResponse("Welcome to Hair Band. Tell me to melt your face.");
            response.Session.MemberId = request.Session.Attributes.MemberId;
            response.Response.Card.Content = "Awwwww yeah!";
            response.Response.Reprompt.OutputSpeech.Text = "Please tell me to say something.";
            response.Response.ShouldEndSession = false;

            return response;
        }

        private AlexaResponse SessionEndedRequestHandler(AlexaRequest request)
        {
            return null;
        }
    }
}