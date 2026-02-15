namespace BlazorApp.Data;

public class ThreadSafeCounterDemo
{
    private static readonly object _gate = new();
    private static int _counter;

    public static void Increment()
    {
        for (int i = 0; i < 100_000; i++)
        {
            lock (_gate)
            {
                _counter++;
            }
        }
    }

    public int Run()
    {
        _counter = 0;

        Parallel.Invoke(
            () => Increment(),
            () => Increment(),
            () => Increment(),
            () => Increment()
        );

        Console.WriteLine(_counter);
        return _counter;
    }
}
