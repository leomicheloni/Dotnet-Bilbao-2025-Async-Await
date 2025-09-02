using System;
using System.Threading.Tasks;

var stopWatch = System.Diagnostics.Stopwatch.StartNew();
stopWatch.Start();
Console.WriteLine("Calling AsyncCall...");

AsyncCall();

Console.WriteLine("Called, took {0}", stopWatch.Elapsed);
Console.ReadLine();

static async void AsyncCall() 
{ 
    Console.Write("Starting remote call... ");

    await Task.Delay(2000);

    Console.WriteLine("Finishing ");
}

