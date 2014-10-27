using System;
using System.Collections.Generic;
using System.Threading;

namespace RushHour
{
    class SolverShared
    {
        public readonly GameData gameData;

        private GameState rootState;
        private TodoQueue todo;
        private ConcurrentTrie visited;
        //private int numVisited = 0;
        //private int round = 0;
        private object solutionLock = new object();
        private GameState solvedState = null;

        public SolverShared(GameData gameData)
        {
            int maxSize = Math.Max(gameData.mapSize.x, gameData.mapSize.y) - 1; // defines the possible positions any car can be in, disregarding orientation
            int maxTrieDepth = gameData.cars.Length;
            this.visited = new ConcurrentTrie(maxTrieDepth, maxSize);
            this.gameData = gameData;
            this.rootState = gameData.startingState;

            if (gameData.use_a_star)
                todo = new MappedQueueSet(maxSize);
            else
                todo = new SingleQueue();

            // test if the first state starts out as solved
            TestSolved(rootState);

            if (!IsSolved)
                // no luck, now we have to solve
                TryPutState(rootState);
        }

        // Outputs the result of the solver
        public void PrintSolution()
        {
            if (solvedState == null)
            {
                if (gameData.mode == OutputMode.Count)
                    Console.WriteLine("-1");
                else
                    Console.WriteLine("Geen oplossing gevonden");
            }
            else
            {
                if (gameData.mode == OutputMode.Count)
                {
                    Console.WriteLine(solvedState.NumPrev);
                }
                else
                {
                    Stack<GameState> solvedStack = new Stack<GameState>();
                    GameState state = solvedState;
                    do
                    {
                        solvedStack.Push(state);
                        state = state.PrevState;
                    } while (state != null);

                    if (gameData.mode == OutputMode.Pretty)
                    {
                        int steps = solvedStack.Count - 1;
                        int i = 0;
                        while (solvedStack.Count > 0)
                        {
                            Console.Clear();
                            Console.WriteLine("Steps: " + (i++) + "/" + steps);
                            Console.WriteLine();
                            Console.WriteLine(gameData.ToString(solvedStack.Pop()));
                            Thread.Sleep(500);
                        }
                    }
                    else
                    {
                        state = solvedStack.Pop();
                        string res = stateDifference(state, solvedStack.Peek());
                        while (solvedStack.Count > 1)
                        {
                            state = solvedStack.Pop();
                            res += ", " + stateDifference(state, solvedStack.Peek());
                        }
                        Console.WriteLine(res);
                    }
                }
            }
        }

        // Helper function for the Solve output mode
        private string stateDifference(GameState current, GameState next)
        {
            for (int i = 0; i < gameData.cars.Length; ++i)
                if (current[i] != next[i])
                {
                    CarInfo car = gameData.cars[i];
                    int posDiff = (int)(next[i]) - (int)(current[i]);

                    string res = car.carID.ToString();
                    if (posDiff > 0)
                    {
                        if (car.laneOrientation == Orientation.horizontal)
                            res += "r" + posDiff;
                        else
                            res += "d" + posDiff;
                    }
                    else
                    {
                        posDiff = -posDiff;
                        if (car.laneOrientation == Orientation.horizontal)
                            res += "l" + posDiff;
                        else
                            res += "u" + posDiff;
                    }
                    return res;
                }
            return "";
        }

        public bool TryPutState(GameState state)
        {
            if (visited.TryPut(state))
            {
                //Console.WriteLine(gameData.ToString(state));
                todo.Put(state, this);
                //Interlocked.Increment(ref numVisited);
                return true;
            }
            return false;
        }

        public GameState GetNextState()
        {
            return todo.TryGet();
        }

        // Used by A*. Estimates how close to a solution a state is, using a simple measure of the amount of occupied cells in front of 'x' and the distance of 'x' to his goal.
        public int EstimateSolvedness(GameState state)
        {
            int distance = gameData.goalPos - state[gameData.targetCar.carArrayIndex];

            // solved
            if (distance == 0)
                return 0;

            int numBlocked = 0;
            int index = gameData.targetCar.carLength;
            int end = index + distance;
            if (index > end)
            {
                int tmp = index;
                index = end;
                end = tmp;
            }
            if (gameData.targetCar.laneOrientation == Orientation.horizontal)
            {
                int y = gameData.targetCar.laneIndex;
                for (; index != end; ++index)
                    if (IsOccupied(index, y, state))
                        ++numBlocked;
            }
            else
            {
                int x = gameData.targetCar.laneIndex;
                for (; index != end; ++index)
                    if (IsOccupied(x, index, state))
                        ++numBlocked;
            }

            return numBlocked;
        }


        public int MapWidth { get { return gameData.mapSize.x; } }
        public int MapHeight { get { return gameData.mapSize.y; } }

        // does a general test for if a cell is occupied by any car in the given state
        public bool IsOccupied(int x, int y, GameState state)
        {
            // optimized to only check cars that could possibly occupy a cell
            foreach (CarInfo possibleOccupant in gameData.cellPossibleCars[x, y])
                if (IsOccupying(x, y, state, possibleOccupant))
                    return true;
            return false;
        }

        // tests if a car occupies a specific cell in the given state
        private bool IsOccupying(int x, int y, GameState state, CarInfo possibleOccupant)
        {
            int pos = state[possibleOccupant.carArrayIndex];

            if (possibleOccupant.laneOrientation == Orientation.horizontal)
            {
                if (x >= pos && x < pos + possibleOccupant.carLength)
                    return true;
            }
            else
            {
                if (y >= pos && y < pos + possibleOccupant.carLength)
                    return true;
            }
            return false;
        }

        public bool IsSolved { get { return solvedState != null; } }

        public void TestSolved(GameState state)
        {
            if (state[gameData.targetCar.carArrayIndex] == gameData.goalPos)
            {
                // it might be possible to have multiple states solved, so lock and check if it's the case, and if so, pick the better
                lock (solutionLock)
                {
                    if (solvedState == null || solvedState.NumPrev > state.NumPrev)
                        solvedState = state;
                }
            }
        }
    }

}
