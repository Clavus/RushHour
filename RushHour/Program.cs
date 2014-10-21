using System;

namespace RushHour
{
    class Program
    {
        static void Main(string[] args)
        {
            //Input input = Input.test_drie(OutputMode.Pretty, false);
            //Input input = Input.test_st(OutputMode.Pretty, false);
            //Input input = Input.test_fifty(OutputMode.Pretty, false);
            Input input = Input.test_rondje(OutputMode.Pretty, false);
            //Input input = Input.test_kannie(OutputMode.Pretty, false);
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
