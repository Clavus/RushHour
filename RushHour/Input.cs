using System;

namespace RushHour
{
    enum OutputMode
    {
        Count = 0,
        Solve = 1, 
        Pretty = 2
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

            gameData = new GameData(map, goal, outputMode);
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
        
        public static Input test_drie(OutputMode outputMode, bool useAstar)
        {
            string[] map = new string[] { 
                "......",
                "..a...",
                "xxa.b.",
                "....b.",
                "......",
                "......"
            };
            Point endPoint = new Point();
            endPoint.x = 2;
            endPoint.y = 4;
            return new Input(outputMode, endPoint, useAstar, map);
        }

        public static Input test_st(OutputMode outputMode, bool useAstar)
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

        public static Input test_rondje(OutputMode outputMode, bool useAstar)
        {
            string[] map = new string[] { 
                "..cddd...q..",
                "a.ce.....q..",
                "axxe.....q..",
                "bbryyi......",
                ".gruui......",
                ".gtt.i...zz."
            };
            Point endPoint = new Point();
            endPoint.x = 2;
            endPoint.y = 10;
            return new Input(outputMode, endPoint, useAstar, map);
        }

        public static Input test_kannie(OutputMode outputMode, bool useAstar)
        {
            string[] map = new string[] { 
                ".xx......q..",
                ".........q..",
            };
            Point endPoint = new Point();
            endPoint.x = 0;
            endPoint.y = 10;
            return new Input(outputMode, endPoint, useAstar, map);
        }

        public static Input test_fifty(OutputMode outputMode, bool useAstar)
        {
            string[] map = new string[] { 
                "..cddd",
                "a.ce..",
                "axxe..",
                "bbryyi",
                ".gruui",
                ".gtt.i"
            };
            Point endPoint = new Point();
            endPoint.x = 2;
            endPoint.y = 4;
            return new Input(outputMode, endPoint, useAstar, map);
        }
    }
}
