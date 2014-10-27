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
        public readonly bool use_a_star;
        public readonly GameData gameData;

        private Input(OutputMode outputMode, Point goal, bool use_a_star, string[] map)
        {
            this.outputMode = outputMode;
            this.use_a_star = use_a_star;

            gameData = new GameData(map, goal, outputMode, use_a_star);
        }

        public static Input ReadFromConsole()
        {
            OutputMode m = (OutputMode)int.Parse(Console.ReadLine());
            Point dim = Point.Parse(Console.ReadLine());
            Point goal = Point.Parse(Console.ReadLine());
            bool use_a_star = int.Parse(Console.ReadLine()) == 1 ? true : false;

            string[] map = new string[dim.y];
            for (int i = 0; i < dim.y; ++i)
                map[i] = Console.ReadLine();

            return new Input(m, goal, use_a_star, map);
        }

        public static Input test_drie(OutputMode outputMode, bool use_a_star)
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
            return new Input(outputMode, endPoint, use_a_star, map);
        }

        public static Input test_st(OutputMode outputMode, bool use_a_star)
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
            return new Input(outputMode, endPoint, use_a_star, map);
        }

        public static Input test_rondje(OutputMode outputMode, bool use_a_star)
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
            return new Input(outputMode, endPoint, use_a_star, map);
        }

        public static Input test_kannie(OutputMode outputMode, bool use_a_star)
        {
            string[] map = new string[] { 
                ".xx......q..",
                ".........q..",
            };
            Point endPoint = new Point();
            endPoint.x = 10;
            endPoint.y = 0;
            return new Input(outputMode, endPoint, use_a_star, map);
        }

        public static Input test_fifty(OutputMode outputMode, bool use_a_star)
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
            return new Input(outputMode, endPoint, use_a_star, map);
        }

        public static Input test_hard1(OutputMode outputMode, bool use_a_star)
        {
            string[] map = new string[] { 
                "ggggrrrrtt...........",
                "iii....h.fjjjpo...s.u",
                "xxab...h.flk.po...s.u",
                "ccab...mm.lk.nnnvvq..",
                "............dddeeeq.."
            };
            Point endPoint = new Point();
            endPoint.x = 2;
            endPoint.y = 2;
            return new Input(outputMode, endPoint, use_a_star, map);
        }

        public static Input test_hard2(OutputMode outputMode, bool use_a_star)
        {
            string[] map = new string[] { 
                "ggggrrrrtt...........",
                "iii....h.fjjjpo...s.u",
                "xxab...h.flk.po...s.u",
                "ccab......lk.nnnvvq..",
                "............dddeeeq.."
            };
            Point endPoint = new Point();
            endPoint.x = 2;
            endPoint.y = 7;
            return new Input(outputMode, endPoint, use_a_star, map);
        }

        public static Input test_hard3(OutputMode outputMode, bool use_a_star)
        {
            string[] map = new string[] { 
                "ggggrrrrtt...........",
                "iii....h.fjjjpo...s.u",
                "xxab...h.flk.po...s.u",
                "ccab...mm.lk.nnnvvq..",
                "............dddeeeq.."
            };
            Point endPoint = new Point();
            endPoint.x = 2;
            endPoint.y = 7;
            return new Input(outputMode, endPoint, use_a_star, map);
        }

        public static Input test_hard4(OutputMode outputMode, bool use_a_star)
        {
            string[] map = new string[] { 
                "ggggrrrrtt...........",
                "iii....h.fjjjpo...s.u",
                "xxab...h.flk.po...s.u",
                "ccab...mm.lk.nnnvvq..",
                "............dddeeeq.."
            };
            Point endPoint = new Point();
            endPoint.x = 2;
            endPoint.y = 19;
            return new Input(outputMode, endPoint, use_a_star, map);
        }
    }
}
