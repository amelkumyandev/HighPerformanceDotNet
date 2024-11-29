using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace FineTunedRXNETvsNotOptimizedRXNET
{
    [MemoryDiagnoser]
    public class NumberProcessorBenchmark
    {
        private readonly IObservable<int> source = Observable.Range(1, 10000000);
        private readonly NumberProcessor defaultProcessor = new NumberProcessor();
        private readonly FineTunedNumberProcessor fineTunedProcessor = new FineTunedNumberProcessor();

        [Benchmark]
        public void DefaultProcessor()
        {
            defaultProcessor.Process(source).Subscribe();
        }

        [Benchmark]
        public void FineTunedProcessor()
        {
            fineTunedProcessor.Process(source).Subscribe();
        }
    }
}
