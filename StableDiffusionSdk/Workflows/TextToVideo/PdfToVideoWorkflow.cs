using PdfSharp.Pdf.IO;
using StableDiffusionTools.Domain;
using StableDiffusionTools.ImageUtilities;
using StableDiffusionTools.Integrations.OpenAi;
using StableDiffusionTools.Integrations.StableDiffusionWebUi;
using static System.String;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using StableDiffusionSdk.Infrastructure;
using PdfReader = iText.Kernel.Pdf.PdfReader;

public class PdfToVideoWorkflow
{
    private readonly StableDiffusionApi _api;
    private readonly GptApi _gptApi;
    private readonly int _resolution;

    private const string Template = @"
## section [instructions]

The ultimate guide for pro prompts building for Midjourney
1. Begin with a basic outline
Imagine yourself as an artist, preparing to draw a picture. Start by making a simple, clear description of the image you want to create. Describe the main subject, the scene, or the small details you want to include. This basic outline will help your AI assistant understand the main idea of your vision.

 

Example:

 

A tranquil forest with a winding river and a small wooden bridge
A child flying a colorful kite on a sunny beach
A busy city street at night with bright neon signs and people walking by
 

As you refine your prompt, remember that a picture is worth a thousand words. The more detailed and vivid your description, the more remarkable the results will be. Now unleash your inner artist and let Midjourney bring your masterpiece to life 🖼️✨

2. Enhance with style and keywords
With the basic outline ready, it's time to add some style and keywords to make your image more interesting. Midjourney's AI is like a talented artist, able to create images in different styles like abstract, dream-like, or true-to-life. By choosing a style or adding relevant keywords, you can guide the AI to make an image that matches your vision. Play around with different styles and keywords, like mixing colors, to find the perfect combination for your desired result.

 

Examples: 

 

A city skyline at dusk in the style of Van Gogh's Starry Night::3, with vibrant colors::2 and swirling clouds::1.
A dreamy underwater scene::2 featuring a mermaid::1 playing a harp::1, surrounded by glowing jellyfish::2, in a surrealist style reminiscent of Salvador Dali.
A detailed medieval battle scene::3, with knights clad in shining armor::2 and dragons soaring overhead::1, inspired by the epic style of J.R.R. Tolkien's Middle-earth.
An abstract representation of a tranquil forest::3, with geometric shapes::2 forming trees and foliage, and a color palette inspired by Wassily Kandinsky's paintings::1.
 

Experiment with various styles and keywords to uncover the ideal combination that brings your artistic vision to life with Midjourney.

## 

You are used within software that integrates with chat gpt api and midjourney api.
You are part of pdf to video workflow.

You are given page from a book and your goal is to generate a midjourney prompt using [instructions] that shows situation on the page.
You should return one or multiple prompts for each page:

    title: '{0}',
    pageNumber: '{1}',
    pageContent: '{2}'

Please provide prompts in following format:
each prompt should be on separate line because program handles your result by splitting it by \n .
format: /imagine promptGeneratedByYou
";

    public PdfToVideoWorkflow(StableDiffusionApi api, GptApi gptApi, int resolution)
    {
        _api = api;
        _gptApi = gptApi;
        _resolution = resolution;
    }

    public async IAsyncEnumerable<string> GetPrompts(string pathToPdf, int startPage)
    {


        var title = Path.GetFileNameWithoutExtension(pathToPdf);

        using var pdfReader = new PdfReader(pathToPdf);
        using var pdfDocument = new PdfDocument(pdfReader);
        for (var i = startPage; i <= pdfDocument.GetNumberOfPages(); i++)
        {
            var page = pdfDocument.GetPage(i);
            var strategy = new SimpleTextExtractionStrategy();
            var pageContent = PdfTextExtractor.GetTextFromPage(page, strategy);

            var prompt = await _gptApi.GenerateTextAsync(
                Format(
                    Template,
                    title,
                    page,
                    pageContent)
            );

            yield return prompt;
        }
        
    }
}