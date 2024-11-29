using System.Reactive.Linq;

namespace FineTunedRXNETvsNotOptimizedRXNET
{
    public class NumberProcessor
    {
        public virtual IObservable<int> Process(IObservable<int> source)
        {
            return source
                .Where(number => number % 2 == 0)
                .Select(number => number * 2);
        }
    }

    public class FineTunedNumberProcessor : NumberProcessor
    {
        private readonly Func<int, bool> filterPredicate;
        private readonly Func<int, int> transformation;

        public FineTunedNumberProcessor(Func<int, bool> filterPredicate = null, Func<int, int> transformation = null)
        {
            this.filterPredicate = filterPredicate ?? (number => number % 2 == 0);
            this.transformation = transformation ?? (number => number * 2);
        }

        public override IObservable<int> Process(IObservable<int> source)
        {
            return source
                .Where(filterPredicate)
                .Select(transformation);
        }
    }
}
