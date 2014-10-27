using System;
using System.Threading;

namespace RushHour
{
    class Program
    {
        static void Main(string[] args)
        {
            //Input input = Input.test_drie(OutputMode.Pretty, false);
            //Input input = Input.test_st(OutputMode.Pretty, false);
            //Input input = Input.test_fifty(OutputMode.Pretty, false);
            //Input input = Input.test_rondje(OutputMode.Pretty, false);
            //Input input = Input.test_kannie(OutputMode.Pretty, false);
            //Input input = Input.test_hard1(OutputMode.Pretty, true);
            //Input input = Input.test_hard2(OutputMode.Pretty, false);
            //Input input = Input.test_hard3(OutputMode.Pretty, true);
            //Input input = Input.test_hard4(OutputMode.Pretty, true);
            Input input = Input.ReadFromConsole();

            //Console.WriteLine(input.gameData);

            SolverShared sharedData = new SolverShared(input.gameData);
            sharedData.Begin();

            //SolverTask task = new SolverTask(sharedData);
            //ThreadPool.QueueUserWorkItem(new WaitCallback(task.Iterate), sharedData.GetNextState());
            //WaitHandle.WaitAll(new ManualResetEvent[] { task.DoneHandle });
            
            

            //task.Begin();
            
            sharedData.PrintSolution();

            //Console.ReadKey();
        }
    }
}
