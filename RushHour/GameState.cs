using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushHour
{
    struct Point
    {
        public byte x, y;

        public Point(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point Parse(string inText)
        {
            string[] parts = inText.Split(' ');
            return new Point(byte.Parse(parts[0]), byte.Parse(parts[1]));
        }
    }

    enum Orientation : byte
    {
        vertical,
        horizontal,
        unknown
    }

    class GameState
    {
        public readonly byte[] carPositions;

        public GameState(int numCars)
        {
            carPositions = new byte[numCars];
        }
    }

    struct CarLane
    {
        public byte laneIndex;
        public Orientation laneOrientation;
    }

    class CarInfo
    {
        public char carID;
        public byte carArrayIndex;
        public CarLane lane;
        public byte carLength;
    }

    class GameData
    {
        // target position for the 'x' car to reach
        public readonly Point goalPoint;
        // dimensions of the map (movement limits)
        public readonly Point mapSize;
        // all cars by index
        public readonly CarInfo[] cars;
        // index of the 'x' car in the above cars array
        public readonly byte targetCar;
        // stores references to all cars that might at one point be present at a cell
        public readonly List<CarInfo>[,] cellPossibleCars;
        // the state of the game at starting positions
        public readonly GameState startingState;

        // helper class used during construction
        private class CarStart
        {
            public char id;
            public Orientation orientation = Orientation.unknown;
            public Point startPos;
            public byte length = 1;

            public CarStart(char id, Point start)
            {
                this.id = id;
                this.startPos = start;
            }
        }

        public GameData(string[] map, Point goalPoint)
        {
            if (map.Length < 1 || map[0].Length < 1) throw new Exception("Invalid input for GameData construction (mapsize)");

            this.goalPoint = goalPoint;
            this.mapSize = new Point((byte)map.Length, (byte)map[0].Length);
            this.cellPossibleCars = new List<CarInfo>[mapSize.x, mapSize.y];
            for (byte x = 0; x < mapSize.x; ++x)
                for (byte y = 0; y < mapSize.y; ++y)
                    this.cellPossibleCars[x, y] = new List<CarInfo>();

            Dictionary<char, CarStart> carStarts = new Dictionary<char, CarStart>();

            CarStart cs = null;
            for (byte x = 0; x < mapSize.x; ++x)
                for (byte y = 0; y < mapSize.y; ++y)
                {
                    char car = map[x][y];
                    if (car == '.')
                        continue;

                    if (carStarts.TryGetValue(car, out cs))
                    {
                        if (cs.startPos.x == x) cs.orientation = Orientation.vertical;
                        else if (cs.startPos.y == y) cs.orientation = Orientation.horizontal;
                        else throw new Exception("Invalid input for GameData construction (badly aligned cars)");

                        if (cs.startPos.x > x) cs.startPos.x = x;
                        else if (cs.startPos.y > y) cs.startPos.y = y;

                        ++cs.length;
                    }
                    else
                        carStarts.Add(car, new CarStart(car, new Point(x, y)));
                }

            cars = new CarInfo[carStarts.Count];
            startingState = new GameState(carStarts.Count);

            byte targetIndex = 255;
            byte index = 0;
            foreach (CarStart start in carStarts.Values)
            {
                CarInfo car = new CarInfo();
                car.carID = start.id;
                car.carArrayIndex = index;
                car.carLength = start.length;
                car.lane.laneOrientation = start.orientation;

                if (start.orientation == Orientation.horizontal)
                {
                    for (int i = 0; i < mapSize.x; ++i)
                        cellPossibleCars[i, start.startPos.y].Add(car);

                    car.lane.laneIndex = start.startPos.y;
                    startingState.carPositions[index] = start.startPos.x;
                }
                else if (start.orientation == Orientation.vertical)
                {
                    for (int i = 0; i < mapSize.y; ++i)
                        cellPossibleCars[start.startPos.x, i].Add(car);

                    car.lane.laneIndex = start.startPos.x;
                    startingState.carPositions[index] = start.startPos.y;
                }
                else
                    throw new Exception("Invalid input for GameData construction (badly aligned cars (again))");

                if (start.id == 'x')
                    targetIndex = index;

                cars[index++] = car;
            }

            if (targetIndex == 255)
                throw new Exception("Invalid input for GameData construction (no 'x' car found)");
            targetCar = targetIndex;
        }

        // Debug writing of the contents of the map at the specified state
        public string ToString(GameState gameState)
        {
            char[,] map = new char[mapSize.x, mapSize.y];
            for (int x = 0; x < mapSize.x; ++x)
                for (int y = 0; y < mapSize.y; ++y)
                    map[x, y] = '.';

            for (int i = 0; i < cars.Length; ++i)
            {
                byte pos = startingState.carPositions[i];
                if (cars[i].lane.laneOrientation == Orientation.vertical)
                {
                    for (byte y = pos; y < pos + cars[i].carLength; ++y)
                        map[cars[i].lane.laneIndex, y] = cars[i].carID;
                }
                else
                {
                    for (byte x = pos; x < pos + cars[i].carLength; ++x)
                        map[x, cars[i].lane.laneIndex] = cars[i].carID;
                }
            }

            map[goalPoint.x, goalPoint.y] = map[goalPoint.x, goalPoint.y] == '.' ? '+' : char.ToUpper(map[goalPoint.x, goalPoint.y]);

            string res = "goal: x->" + map[goalPoint.x, goalPoint.y] + '\n';
            for (int x = 0; x < mapSize.x; ++x)
            {
                for (int y = 0; y < mapSize.y; ++y)
                    res += map[x, y];
                res += '\n';
            }
            return res;
        }

        // Debug writing of the contents of the map at starting positions
        public override string ToString()
        {
            return ToString(startingState);
        }
    }

}
