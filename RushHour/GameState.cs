using System;
using System.Collections.Generic;

namespace RushHour
{
    // The GameState class stores all variable information about the game being solved. It cooperates with GameState to give meaning to the positions stored here.
    abstract class GameState
    {
        private readonly byte[] carPositions;

        protected GameState(int numCars) { carPositions = new byte[numCars]; }
        protected GameState(GameState other) : this(other.carPositions.Length) { }

        public byte this[int index]
        {
            get { return carPositions[index]; }
            set { carPositions[index] = value; }
        }

        public GameState CreateMoved(CarInfo movedCar, int placesMoved)
        {
            GameState newState = createFromThis();
            carPositions.CopyTo(newState.carPositions, 0);
            newState.carPositions[movedCar.carArrayIndex] = (byte)(carPositions[movedCar.carArrayIndex] + placesMoved);
            return newState;
        }

        public abstract int NumPrev { get; }
        public abstract GameState PrevState { get; }

        protected abstract GameState createFromThis();
    }

    class CountingGameState : GameState
    {
        public int numPrev = 0;

        private CountingGameState(GameState other) : base(other) { }
        public CountingGameState(int numCars) : base(numCars) { }

        protected override GameState createFromThis()
        {
            CountingGameState newState = new CountingGameState(this);
            newState.numPrev = numPrev + 1;
            return newState;
        }

        public override int NumPrev { get { return numPrev; } }
        public override GameState PrevState { get { return null; } }
    }

    class ReferencedGameState : GameState
    {
        public GameState prevState = null;

        private ReferencedGameState(GameState other) : base(other) { }
        public ReferencedGameState(int numCars) : base(numCars) { }

        protected override GameState createFromThis()
        {
            ReferencedGameState newState = new ReferencedGameState(this);
            newState.prevState = this;
            return newState;
        }

        public override int NumPrev
        {
            // count number of ancestors. Only called after solver is done, so it doesn't matter it's not efficient
            get
            {
                int num = 0;
                GameState ancestor = prevState;
                while (ancestor != null)
                {
                    ancestor = ancestor.PrevState;
                    ++num;
                }
                return num;
            }
        }
        public override GameState PrevState { get { return prevState; } }
    }
}
