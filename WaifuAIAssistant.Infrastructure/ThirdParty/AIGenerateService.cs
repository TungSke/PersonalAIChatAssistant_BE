using GenerativeAI;
using GenerativeAI.Types;
using Microsoft.Extensions.Configuration;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

namespace WaifuAIAssistant.Infrastructure.ThirdParty
{
    public class AIGenerateService : IAIGenerateService
    {
        private readonly IConfiguration _configuration;
        public AIGenerateService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Response(string message, string configStyle)
        {
            var apiKey = _configuration["GenerativeAI:AIAPIKey"];
            var googleAI = new GoogleAi(apiKey);


            var googleModel = googleAI.CreateGenerativeModel("models/gemini-2.0-flash",
                systemInstruction: configStyle
                );
            
            var googleResponse = await googleModel.GenerateContentAsync(message);
            Console.WriteLine("Google AI Response:");
            Console.WriteLine(googleResponse.Text());
            Console.WriteLine();
            return null;
        }
    }
}