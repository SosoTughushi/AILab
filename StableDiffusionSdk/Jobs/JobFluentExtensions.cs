using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StableDiffusionSdk.Workflows;

namespace StableDiffusionSdk.Jobs
{
    public static class JobFluentExtensions
    {
        public static IJob<TResultingParameterType, TResult> MapParameter<TSourceParameterType, TResult,
            TResultingParameterType>(
            this IJob<TSourceParameterType, TResult> decorated,
            Func<TResultingParameterType, TSourceParameterType> mapFunction
        )
        {
            return DynamicJob.Create(async (TResultingParameterType parameter) =>
            {
                var mappedParameter = mapFunction(parameter);
                var result = await decorated.Run(mappedParameter);

                return result;
            });
        }

        public static IJob<TResultingParameterType, TResult> MapParameter<TSourceParameterType, TResult,
            TResultingParameterType>(
            this IJob<TSourceParameterType, TResult> decorated,
            Func<TResultingParameterType, Task<TSourceParameterType>> mapFunction
        )
        {
            return DynamicJob.Create(async (TResultingParameterType parameter) =>
            {
                var mappedParameter = await mapFunction(parameter);
                var result = await decorated.Run(mappedParameter);

                return result;
            });
        }

        public static IJob<TParameter, TNewResult> MapResult<TParameter, TResult, TNewResult>(
            this IJob<TParameter, TResult> decorated,
            Func<TResult, TNewResult> mapFunction)
        {
            return DynamicJob.Create(async (TParameter parameter) =>
            {
                var result = await decorated.Run(parameter);
                return mapFunction(result);
            });
        }

        public static IJob<TParameter, TNewResult> MapResult<TParameter, TResult, TNewResult>(
            this IJob<TParameter, TResult> decorated,
            Func<TResult, Task<TNewResult>> mapFunction)
        {
            return DynamicJob.Create(async (TParameter parameter) =>
            {
                var result = await decorated.Run(parameter);
                return await mapFunction(result);
            });
        }

        public static async Task<TResult> RunSafe<TParameter, TResult>(this IJob<TParameter, TResult> job, TParameter parameter)
        {
            try
            {
                return await job.Run(parameter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default;
            }
        }
    }

    public class DynamicJob<TParameter, TResult> : IJob<TParameter, TResult>
    {
        private readonly Func<TParameter, Task<TResult>> _func;

        public DynamicJob(Func<TParameter, Task<TResult>> func)
        {
            _func = func;
        }

        public Task<TResult> Run(TParameter parameters)
        {
            return _func(parameters);
        }
    }

    public class DynamicJob
    {
        public static IJob<TParameter, TResult> Create<TParameter, TResult>(Func<TParameter, Task<TResult>> map) =>
            new DynamicJob<TParameter, TResult>(map);

        public static IJob<Unit, TResult> Create<TResult>(TResult result) =>
            DynamicJob.Create((Unit _) => Task.FromResult(result));
    }

    public class Unit{ }
}