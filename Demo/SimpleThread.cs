public class Example1
{
    public void SimpleThread()
    {
        var thread = new System.Threading.Thread(() =>
        {
            System.Console.WriteLine("Hello from another thread!");
        });
        thread.Start();
    }

}
