using System;
using System.Collections.Generic;

namespace RushHour
{
    struct Point
    {
        public int x, y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point Parse(string inText)
        {
            string[] parts = inText.Split(' ');
            return new Point(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }

    enum Orientation
    {
        vertical,
        horizontal,
        unknown
    }

    class GameState
    {
        public GameState originState = null;
        public readonly byte[] carPositions;

        public GameState(int numCars)
        {
            carPositions = new byte[numCars];
        }

        public GameState(GameState originState, CarInfo movedCar, int placesToMove)
        {
            this.originState = originState;

            carPositions = new byte[originState.carPositions.Length];
            originState.carPositions.CopyTo(carPositions, 0);
            carPositions[movedCar.carArrayIndex] = (byte)(carPositions[movedCar.carArrayIndex] + placesToMove);
        }
    }

    class CarInfo
    {
        public char carID;
        public int carArrayIndex;
        public int laneIndex;
        public Orientation laneOrientation;
        public int carLength;
    }

    class GameData
    {
        // target position for the 'x' car to reach
        public readonly byte goalPos;
        // dimensions of the map (movement limits)
        public readonly Point mapSize;
        // all cars by index
        public readonly CarInfo[] cars;
        // stores references to all cars that might at one point be present at a cell
        public readonly List<CarInfo>[,] cellPossibleCars;
        // the state of the game at starting positions
        public readonly GameState startingState;

        // helper class used during construction
        private class CarStart
        {
            internal char id;
            internal Orientation orientation = Orientation.unknown;
            internal Point startPos;
            internal int length = 1;

            internal CarStart(char id, Point start)
            {
                this.id = id;
                this.startPos = start;
            }
        }

        public GameData(string[] map, Point goalPoint)
        {
            if (map.Length < 1 || map[0].Length < 1) throw new Exception("Invalid input for GameData construction (mapsize)");

            this.mapSize = new Point(map.Length, map[0].Length);
            this.cellPossibleCars = new List<CarInfo>[mapSize.x, mapSize.y];
            for (int x = 0; x < mapSize.x; ++x)
                for (int y = 0; y < mapSize.y; ++y)
                    this.cellPossibleCars[x, y] = new List<CarInfo>();

            Dictionary<char, CarStart> carStarts = new Dictionary<char, CarStart>();

            CarStart cs = null;
            for (int x = 0; x < mapSize.x; ++x)
                for (int y = 0; y < mapSize.y; ++y)
                {
                    char car = map[x][y];
                    if (car == '.')
                        continue;

                    if (carStarts.TryGetValue(car, out cs))
                    {
                        if (cs.startPos.x == x) cs.orientation = Orientation.horizontal;
                        else if (cs.startPos.y == y) cs.orientation = Orientation.vertical;
                        else throw new Exception("Invalid input for GameData construction (badly aligned cars)");

                        if (cs.startPos.x > x) cs.startPos.x = x;
                        else if (cs.startPos.y > y) cs.startPos.y = y;

                        ++cs.length;
                    }
                    else
                        carStarts.Add(car, new CarStart(car, new Point(x, y)));
                }

            if (!carStarts.ContainsKey('x'))
                throw new Exception("Invalid input for GameData construction (no 'x' car found)");

            if (carStarts['x'].orientation == Orientation.horizontal)
                this.goalPos = (byte)goalPoint.y;
            else
                this.goalPos = (byte)goalPoint.x;

            cars = new CarInfo[carStarts.Count];
            startingState = new GameState(carStarts.Count);

            int index = 0;
            foreach (CarStart start in carStarts.Values)
            {
                CarInfo car = new CarInfo();
                car.carID = start.id;
                car.carArrayIndex = index;
                car.carLength = start.length;
                car.laneOrientation = start.orientation;

                if (start.orientation == Orientation.horizontal)
                {
                    for (int i = 0; i < mapSize.x; ++i)
                        cellPossibleCars[i, start.startPos.x].Add(car);

                    car.laneIndex = start.startPos.x;
                    startingState.carPositions[index] = (byte)start.startPos.y;
                }
                else if (start.orientation == Orientation.vertical)
                {
                    for (int i = 0; i < mapSize.y; ++i)
                        cellPossibleCars[start.startPos.y, i].Add(car);

                    car.laneIndex = start.startPos.y;
                    startingState.carPositions[index] = (byte)start.startPos.x;
                }
                else
                    throw new Exception("Invalid input for GameData construction (badly aligned cars (again))");

                cars[index++] = car;
            }
        }

        // Debug writing of the contents of the map at the specified state
        public string ToString(GameState gameState)
        {
            char[,] map = new char[mapSize.x, mapSize.y];
            for (int x = 0; x < mapSize.x; ++x)
                for (int y = 0; y < mapSize.y; ++y)
                    map[x, y] = '.';

            CarInfo targetCar = null;
            foreach (CarInfo car in cars)
            {
                if (car.carID == 'x') 
                    targetCar = car;

                int pos = gameState.carPositions[car.carArrayIndex];
                if (car.laneOrientation == Orientation.vertical)
                {
                    for (int y = pos; y < pos + car.carLength; ++y)
                        map[car.laneIndex, y] = car.carID;
                }
                else
                {
                    for (int x = pos; x < pos + car.carLength; ++x)
                        map[x, car.laneIndex] = car.carID;
                }
            }

            int gx = targetCar.laneIndex, gy = targetCar.laneIndex;
            if (targetCar.laneOrientation == Orientation.horizontal)
                gx = goalPos;
            else
                gy = goalPos;

            map[gx, gy] = map[gx, gy] == '.' ? '+' : char.ToUpper(map[gx, gy]);

            string res = "";
            for (int x = 0; x < mapSize.x; ++x)
            {
                for (int y = 0; y < mapSize.y; ++y)
                    res += map[y, x];
                res += '\n';
            }
            for (int x = 0; x < mapSize.x; ++x)
                res += ' ';
            res += "goal: x->" + map[gx, gy];
            return res;
        }

        // Debug writing of the contents of the map at starting positions
        public override string ToString()
        {
            return ToString(startingState);
        }
    }

}
