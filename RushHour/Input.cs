using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour
{
    enum OutputMode
    {
        Count,
        Solve
    }

    class Input
    {
        public readonly OutputMode outputMode;
        public readonly bool useAstar;
        public readonly GameData gameData;

        private Input(OutputMode outputMode, Point goal, bool useAstar, string[] map)
        {
            this.outputMode = outputMode;
            this.useAstar = useAstar;

            gameData = new GameData(map, goal);
        }

        public static Input ReadFromConsole()
        {
            OutputMode m = (OutputMode)int.Parse(Console.ReadLine());
            Point dim = Point.Parse(Console.ReadLine());
            Point goal = Point.Parse(Console.ReadLine());
            bool useAstar = int.Parse(Console.ReadLine()) == 1 ? true : false;

            string[] map = new string[dim.y];
            for (int i = 0; i < dim.y; ++i)
                map[i] = Console.ReadLine();

            return new Input(m, goal, useAstar, map);
        }

        public static Input test(OutputMode outputMode, bool useAstar)
        {
            string[] map = new string[] { 
                "aaobcc",
                "..ob..",
                "xxo...",
                "deeffp",
                "d..k.p",
                "hh.k.p"
            };
            Point endPoint = new Point();
            endPoint.x = 2;
            endPoint.y = 4;
            return new Input(outputMode, endPoint, useAstar, map);
        }
    }

}
