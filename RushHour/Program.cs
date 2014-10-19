using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour
{
    class Program
    {
        static void Main(string[] args)
        {
            Input input = Input.test(OutputMode.Count, false);
            //Input input = Input.ReadFromConsole();
            
            Console.WriteLine(input.gameData);
            Console.ReadKey();
        }
    }
}
