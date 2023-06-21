using StableDiffusionSdk.Infrastructure;
using StableDiffusionTools.Integrations.OpenAi;

namespace StableDiffusionSdk.HermansDialogicalSelfTheory;

public class SelfOrchestrator
{
    private readonly List<GptSelfPosition> _selfPositions;

    public SelfOrchestrator(GptApi gptApi)
    {
        _selfPositions = Directory.GetFiles(@"D:\Stable Diffusion\Projects\StableDiffusionTools\StableDiffusionSdk\HermansDialogicalSelfTheory\SelfDescriptions")
            .Select(fileName => new GptSelfPosition(gptApi, fileName, Path.GetFileNameWithoutExtension(fileName), 10))
            .ToList();
    }

    public async Task Run(string topic)
    {
        Console.WriteLine($"Topic: {topic}");
        var selfTalkMessages = new List<SelfTalkMessage>();
        var usedSelves = new List<GptSelfPosition>();

        for(var i = 0; i < 100; i++)
        {
            if (_selfPositions.Count == 0) // All selves have spoken. Reset the list.
            {
                _selfPositions.AddRange(usedSelves);
                usedSelves.Clear();
            }
            
            var curr = PickWeightedRandom(_selfPositions);
            _selfPositions.Remove(curr);  // Remove the current 'self' from the list
            usedSelves.Add(curr);  // Add the current 'self' to the usedSelves list
            
            var response = await curr.SaySomething(topic, selfTalkMessages);
            selfTalkMessages.Add(new SelfTalkMessage(curr, response));
            Console.Write($"{response} ");
        }
    }

    // Picks an item based on weight
    private GptSelfPosition PickWeightedRandom(List<GptSelfPosition> list)
    {
        int totalWeight = list.Sum(c => c.Weight); // this assumes you have a Weight property in your GptSelfPosition class
        int randomNumber = new Random().Next(0, totalWeight);

        GptSelfPosition selectedItem = null;

        foreach (var item in list)
        {
            if (randomNumber < item.Weight)
            {
                selectedItem = item;
                break;
            }

            randomNumber -= item.Weight;
        }

        return selectedItem;
    }
}

