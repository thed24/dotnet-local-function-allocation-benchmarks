using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class ScopeAllocationBenchmarks
{
    private int[] Data;
    private Dictionary<int, int> Cache;

    private static int SomeExpensiveComputation(int x) => x * 124 * 2 / 2 + 1;

    [GlobalSetup]
    public void GlobalSetup()
    {
        int totalSize = 1000;

        Data = new int[totalSize];
        Cache = new();
        Random random = new();

        for (int i = 0; i < Data.Length; i++)
        {
            Data[i] = random.Next(totalSize);
        }
    }

    [IterationSetup]
    public void IterationSetup()
    {
        Cache.Clear();
    }

    [Benchmark]
    public void LocalFunctionExample()
    {
        int GetOrAdd(int key, Func<int> valueFactory)
        {
            if (Cache.TryGetValue(key, out int value))
            {
                return value;
            }

            value = valueFactory();
            Cache.Add(key, value);
            return value;
        }

        for (int i = 0; i < Data.Length; i++)
        {
            GetOrAdd(Data[i], () => SomeExpensiveComputation(Data[i]));
        }
    }

    [Benchmark]
    public void StaticLocalFunctionExample()
    {
        int GetOrAdd(int key, Func<int, int> valueFactory)
        {
            if (Cache.TryGetValue(key, out int value))
            {
                return value;
            }

            value = valueFactory(key);
            Cache.Add(key, value);
            return value;
        }

        for (int i = 0; i < Data.Length; i++)
        {
            GetOrAdd(Data[i], static (x) => SomeExpensiveComputation(x));
        }
    }
}