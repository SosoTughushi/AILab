using System.Text;
using StableDiffusionTools.Integrations.OpenAi;

namespace StableDiffusionSdk.HermansDialogicalSelfTheory
{
    public class GptSelfPosition
    {
        private readonly GptApi _gptApi;
        private readonly string _description;

        public GptSelfPosition(GptApi gptApi, string selfDescriptionLocation, string name, int weight)
        {
            _gptApi = gptApi;
            this._description = File.ReadAllText(selfDescriptionLocation);
            this.Name = name;
            this.Weight = weight;
        }

        public string Name { get; }
        public int Weight { get;  }


        public async Task<string> SaySomething(string topic, List<SelfTalkMessage> messages)
        {
            StringBuilder prompt = new StringBuilder("You are embodying the role of '" + this.Name + "'. The topic of discussion is '" + topic + "'. You're based on the following description: \n" + this._description + "\n");

            prompt.Append("Your train of thought so far has been:\n");
            foreach (var message in messages)
            {
                prompt.Append(message.message+ " ");
            }

            prompt.Append("Now, continue this train of thought, integrating the viewpoints of '" + this.Name + "' seamlessly into the discussion. Remember to vary between questioning, stating, and hypothesizing. Try to use transitional phrases to connect ideas, and always speak from a first-person perspective. Compose a concise response of no more than two sentences.");

            string response = await _gptApi.GenerateTextAsync(prompt.ToString());

            return response;
        }

    }

    public record SelfTalkMessage(GptSelfPosition Author, string message);
}