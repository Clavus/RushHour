using System;
using System.Collections.Generic;
using System.Threading;

namespace RushHour
{
    // Thread safe Trie structure (with strict maximum depth and branching)
    class Trie
    {
        private class Node
        {
            internal Node[] children;
            internal Node(int numChildren) { children = new Node[numChildren]; }
        }

        private Node root;
        private int maxDepth_minusOne;
        private int maxBranches;

        public Trie(int maxDepth, int maxBranches)
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
                childIndex = state.carPositions[depth];
                storedNode = Interlocked.CompareExchange(ref current.children[childIndex], newNode, null);
                if (storedNode == null)
                    // node was succesfully placed, new Node instance is needed
                    newNode = new Node(maxBranches);
                current = current.children[childIndex];
            }

            // only this last node is checked for the return value.
            childIndex = state.carPositions[maxDepth_minusOne];
            return Interlocked.CompareExchange(ref current.children[childIndex], new Node(0), null) == null;
        }
    }

    class SolverShared
    {
        public readonly GameData gameData;

        private GameState rootState;
        private Queue<GameState> todo = new Queue<GameState>();
        private Trie visited;
        private int numVisited = 0;
        private GameState solvedState = null;

        public SolverShared(GameData gameData)
        {
            int maxTrieBranches = Math.Max(gameData.mapSize.x, gameData.mapSize.y);
            int maxTrieDepth = gameData.startingState.carPositions.Length;
            this.visited = new Trie(maxTrieDepth, maxTrieBranches);
            this.gameData = gameData;
            this.rootState = gameData.startingState;

            // test if the first state starts out as solved
            foreach (CarInfo car in gameData.cars)
                TestSolved(car, rootState);

            if (!IsSolved)
                // no luck, now we have to solve
                TryPutState(rootState);
        }

        public void PrintSolution(OutputMode mode)
        {
            if (solvedState != null)
            {
                if (mode == OutputMode.Pretty)
                {
                    Stack<GameState> solvedStack = new Stack<GameState>();
                    GameState state = solvedState;
                    do
                    {
                        solvedStack.Push(state);
                        state = state.originState;
                    }
                    while (state != null);
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
            }
            else if (mode == OutputMode.Count)
                Console.WriteLine("-1");
            else
                Console.WriteLine("Geen oplossing gevonden");
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
            return todo.Dequeue();
        }

        public int MapWidth { get { return gameData.mapSize.x; } }
        public int MapHeight { get { return gameData.mapSize.y; } }

        public bool IsOccupied(int x, int y, GameState state)
        {
            foreach (CarInfo possibleOccupant in gameData.cellPossibleCars[x, y])
                if (IsOccupying(x, y, state, possibleOccupant))
                    return true;
            return false;
        }

        private bool IsOccupying(int x, int y, GameState state, CarInfo possibleOccupant)
        {
            int pos = state.carPositions[possibleOccupant.carArrayIndex];

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
            if (!IsSolved && car.carID == 'x' && state.carPositions[car.carArrayIndex] == gameData.goalPos)
                solvedState = state;
        }
    }

}
