
using System.Collections.Concurrent;
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

    interface TodoQueue
    {
        void Put(GameState state, SolverShared sharedData);
        void RetrieveAll(List<GameState> outStates);
    }

    // Used by A*. Two measures are used, one having priority over the other.
    class MappedQueueSet : TodoQueue
    {
        private ConcurrentQueue<GameState>[] todos;
        private readonly int size;

        public MappedQueueSet(int size)
        {
            this.size = size;
            todos = new ConcurrentQueue<GameState>[size];
            for (int i = 0; i < size; ++i)
                todos[i] = new ConcurrentQueue<GameState>();
        }

        public void Put(GameState state, SolverShared sharedData)
        {
            int numBlocked = sharedData.EstimateSolvedness(state);
            todos[numBlocked].Enqueue(state);
        }

        public void RetrieveAll(List<GameState> outStates)
        {   
            for (int i = 0; i < size; ++i)
            {
                // lowest index is best guess
                if (!todos[i].IsEmpty)
                {
                    GameState stored;
                    while (todos[i].TryDequeue(out stored))
                        outStates.Add(stored);
                    break;
                }
            }
        }
    }

    class SingleQueue : TodoQueue
    {
        private ConcurrentQueue<GameState> todo = new ConcurrentQueue<GameState>();

        public void Put(GameState state, SolverShared sharedData)
        {
            todo.Enqueue(state);
        }
        public void RetrieveAll(List<GameState> outStates)
        {
            GameState stored;
            while (todo.TryDequeue(out stored))
                outStates.Add(stored);
        }
    }

}
