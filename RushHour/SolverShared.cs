using System;
using System.Collections.Generic;
using System.Threading;

namespace RushHour
{
    // Thread safe Trie structure (with strict maximum depth and branching)
    class ConcurrentTrie
    {
        private class Node
        {
            internal Node[] children;
            internal Node(int numChildren) { children = new Node[numChildren]; }
        }

        private Node root;
        private int maxDepth_minusOne;
        private int maxBranches;

        public ConcurrentTrie(int maxDepth, int maxBranches)
        {
            this.maxDepth_minusOne = maxDepth - 1;
            this.maxBranches = maxBranches;
            this.root = new Node(maxBranches);
        }

        // Tries to puts a byte array into the trie. Returns true if it didn't exist, false otherwise. 
        public bool TryPut(GameState state)
        {
            // node to be used to fill in a null position
            Node newNode = new Node(maxBranches);
            Node storedNode;

            // traverse the trie until the second last node is encountered
            Node current = root;
            int childIndex;
            for (int depth = 0; depth < maxDepth_minusOne; ++depth)
            {
                childIndex = state[depth];
                storedNode = Interlocked.CompareExchange(ref current.children[childIndex], newNode, null);
                if (storedNode == null)
                    // node was succesfully placed, new Node instance is needed
                    newNode = new Node(maxBranches);
                current = current.children[childIndex];
            }

            // only this last node is checked for the return value.
            childIndex = state[maxDepth_minusOne];
            return Interlocked.CompareExchange(ref current.children[childIndex], new Node(0), null) == null;
        }
    }

    class SolverShared
    {
        public readonly GameData gameData;

        private GameState rootState;
        private Queue<GameState> todo = new Queue<GameState>();
        private ConcurrentTrie visited;
        private int numVisited = 0;
        private GameState solvedState = null;

        public SolverShared(GameData gameData)
        {
            int maxTrieBranches = Math.Max(gameData.mapSize.x, gameData.mapSize.y) - 1; // defines the possible positions cars can be in, disregarding orientation
            int maxTrieDepth = gameData.cars.Length;
            this.visited = new ConcurrentTrie(maxTrieDepth, maxTrieBranches);
            this.gameData = gameData;
            this.rootState = gameData.startingState;

            // test if the first state starts out as solved
            foreach (CarInfo car in gameData.cars)
                TestSolved(car, rootState);

            if (!IsSolved)
                // no luck, now we have to solve
                TryPutState(rootState);
        }

        // Outputs the result of the solver
        public void PrintSolution(OutputMode mode)
        {
            if (solvedState == null)
            {
                if (mode == OutputMode.Count)
                    Console.WriteLine("-1");
                else
                    Console.WriteLine("Geen oplossing gevonden");
            }
            else
            {
                if (mode == OutputMode.Count)
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

                    if (mode == OutputMode.Pretty)
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

        public void TryPutState(GameState state)
        {
            if (visited.TryPut(state))
            {
                //Console.WriteLine(gameData.ToString(state));
                todo.Enqueue(state);
                Interlocked.Increment(ref numVisited);
            }
        }

        public GameState GetNextState()
        {
            if (todo.Count > 0)
                return todo.Dequeue();
            else
                return null;
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

        public void TestSolved(CarInfo car, GameState state)
        {
            if (!IsSolved && car.carID == 'x' && state[car.carArrayIndex] == gameData.goalPos)
                solvedState = state;
        }
    }

}
