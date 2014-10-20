using System;

namespace RushHour
{
    class Program
    {
        static void Main(string[] args)
        {
            Input input = Input.test(OutputMode.Verbose, false);
            //Input input = Input.ReadFromConsole();

            //Console.WriteLine(input.gameData);

            SolverShared sharedData = new SolverShared(input.gameData);

            SolverTask task = new SolverTask(sharedData);
            task.Begin();

            sharedData.PrintSolution(input.outputMode);

            Console.ReadKey();
        }
    }
}
