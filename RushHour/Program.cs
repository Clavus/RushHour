using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RushHour
{
    class SnapshotTrie
    {
        private class Node
        {
            internal Node[] children;
            internal Node(byte numChildren) { children = new Node[numChildren]; }
        }

        private Node root;
        private byte maxDepth_1;
        private byte maxBranches;

        public SnapshotTrie(byte maxDepth, byte maxBranches)
        {
            this.maxDepth_1 = (byte)(maxDepth - 1);
            this.maxBranches = maxBranches;
            this.root = new Node(maxBranches);
        }

        // true if it didn't exist
        public bool Put(ref GameSnapshot newSnapshot)
        {
            Node current = root;
            byte childIndex;
            for (byte depth = 0; depth < maxDepth_1; ++depth)
            {
                childIndex = newSnapshot.carPositions[depth];
                if (current.children[childIndex] == null)
                    current.children[childIndex] = new Node(maxBranches);
                current = current.children[childIndex];
            }
            childIndex = newSnapshot.carPositions[maxDepth_1];
            if (current.children[childIndex] == null)
            {
                // not found, create (just tag as existing)
                current.children[childIndex] = new Node(0);
                return true;
            }
            else
            {
                // found
                return false;
            }
        }
    }

    class SharedData
    {
        public readonly GameData gameData;

        public bool IsSolved = false;
        private Queue<GameSnapshot> todo = new Queue<GameSnapshot>();
        private List<GameSnapshot> visited = new List<GameSnapshot>();

        public SharedData(GameData gameData)
        {
            this.gameData = gameData;
            todo.Enqueue(gameData.startingState);

            byte maxTrieBranches = Math.Max(gameData.mapSize.x, gameData.mapSize.y);
            byte maxTrieDepth = (byte)gameData.startingState.carPositions.Length;
            SnapshotTrie trie = new SnapshotTrie(maxTrieDepth, maxTrieBranches);

            GameSnapshot next = todo.Dequeue();

            Console.WriteLine(trie.Put(ref next));
            Console.WriteLine(trie.Put(ref next));
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Input input = Input.test(OutputMode.Count, false);
            //Input input = Input.ReadFromConsole();

            SharedData sharedData = new SharedData(input.gameData);

            Console.WriteLine(input.gameData);
            Console.ReadKey();
        }
    }
}
