using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class CollectionBenchmarks
{
    private const int NumElements = 1000000;
    private List<int> list;
    private Dictionary<int, int> dict;
    private Queue<int> queue;
    private Stack<int> stack;
    private HashSet<int> hashSet;
    private SortedSet<int> sortedSet;

    private int nextElement;

    [GlobalSetup]
    public void Setup()
    {
        list = new List<int>(NumElements);
        dict = new Dictionary<int, int>(NumElements);
        queue = new Queue<int>(NumElements);
        stack = new Stack<int>(NumElements);
        hashSet = new HashSet<int>(NumElements);
        sortedSet = new SortedSet<int>();

        for (int i = 0; i < NumElements; i++)
        {
            list.Add(i);
            dict.Add(i, i);
            queue.Enqueue(i);
            stack.Push(i);
            hashSet.Add(i);
            sortedSet.Add(i);
        }

        // Start the counter for adding new elements
        nextElement = NumElements;
    }

    [Benchmark]
    public void List_Add() => list.Add(nextElement++);

    [Benchmark]
    public void Dictionary_Add() => dict.Add(nextElement++, nextElement);

    [Benchmark]
    public void Queue_Enqueue() => queue.Enqueue(nextElement++);

    [Benchmark]
    public void Stack_Push() => stack.Push(nextElement++);

    [Benchmark]
    public void HashSet_Add() => hashSet.Add(nextElement++);

    [Benchmark]
    public void SortedSet_Add() => sortedSet.Add(nextElement++);

    [Benchmark]
    public void List_Lookup() => _ = list[NumElements - 1];

    [Benchmark]
    public void Dictionary_Lookup() => _ = dict[NumElements - 1];

    [Benchmark]
    public void HashSet_Lookup() => _ = hashSet.Contains(NumElements - 1);

    [Benchmark]
    public void SortedSet_Lookup() => _ = sortedSet.Contains(NumElements - 1);

    // Cleanup after each iteration
    [IterationCleanup]
    public void CleanupAfterIteration()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        list = null;
        dict = null;
        queue = null;
        stack = null;
        hashSet = null;
        sortedSet = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
