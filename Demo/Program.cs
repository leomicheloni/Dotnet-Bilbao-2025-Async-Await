using System;
using System.Threading.Tasks;

internal class Program
{
    public static void Main(string[] args)
    {
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        stopWatch.Start();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Calling AsyncCall...");

        Task.Run(()=>AsyncCall());

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Called, took {0}", stopWatch.Elapsed);

        Console.ReadLine();
    }

    static async Task<int> AsyncCall()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Async Method starts... ");

        await Task.Delay(2000);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Async Method finishing ");
        return 42;
    }
}