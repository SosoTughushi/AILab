using System.Text;
using Newtonsoft.Json;
using StableDiffusionSdk.StableDiffusionApi;

namespace StableDiffusionSdk.Jobs;


public class LoggerNonGeneric
{
    protected static int _depth = 0;
    protected static int _counter = 0;
}

public class Logger<TParameters, TResult> : LoggerNonGeneric, IJob<TParameters, TResult>
{

    private readonly IJob<TParameters, TResult> _job;
    private readonly bool _outputsOnly;

    public Logger(IJob<TParameters, TResult> job, bool outputsOnly)
    {
        _job = job;
        _outputsOnly = outputsOnly;
    }

    public async Task<TResult> Run(TParameters parameters)
    {
        var counter = ++_counter;
        Log($"{typeof(TParameters).Name}-> {typeof(TResult).Name}", counter);
        if (!_outputsOnly)
        {
            Log(JsonConvert.SerializeObject(parameters), counter);
        }
        // Run the job

        _depth++;
        TResult result = await _job.Run(parameters);
        _depth--;
        
        
        Log($"{typeof(TParameters).Name} <- {typeof(TResult).Name}", counter);
        Log(JsonConvert.SerializeObject(result), counter);

        return result;
    }

    private void Log(string text, int counter)
    {
        var spaces = string.Join(" ",Enumerable.Repeat(string.Empty, _depth*5));

        foreach (var line in text.Split('\n'))
        {
            Console.WriteLine(counter+ spaces + line);
        }
    }
}

public static class LoggerJobExtensions
{
    public static Logger<TParameters, TResult> WithLogging<TParameters, TResult>(
        this IJob<TParameters, TResult> job, bool outputsOnly = true)
    {
        return new Logger<TParameters, TResult>(job, outputsOnly);
    }
}