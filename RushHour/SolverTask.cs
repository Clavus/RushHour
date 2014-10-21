using System;
using System.Collections.Generic;

namespace RushHour
{
    class SolverTask
    {
        private SolverShared sharedData;
        private GameState state;
        private List<GameState> nextStates = new List<GameState>();

        public SolverTask(SolverShared sharedData)
        {
            this.sharedData = sharedData;
        }

        private void addState(CarInfo movedCar, int placesMoved)
        {
            // create a new state and set the changes
            GameState newState = state.CreateMoved(movedCar, placesMoved);

            // test for completion
            sharedData.TestSolved(movedCar, newState);
            nextStates.Add(newState);
        }

        // tests all movements for the provided car, and adds them if they're valid (not overlapping other cars)
        private void testCarMovements(CarInfo car)
        {
            int pos = state[car.carArrayIndex];

            if (car.laneOrientation == Orientation.horizontal)
            {
                int y = car.laneIndex;

                for (int x = pos - 1, i = -1; x >= 0; --x, --i)
                    if (sharedData.IsOccupied(x, y, state)) break;
                    else addState(car, i);

                for (int x = pos + car.carLength, i = 1; x < sharedData.MapWidth; ++x, ++i)
                    if (sharedData.IsOccupied(x, y, state)) break;
                    else addState(car, i);
            }
            else
            {
                int x = car.laneIndex;

                for (int y = pos - 1, i = -1; y >= 0; --y, --i)
                    if (sharedData.IsOccupied(x, y, state)) break;
                    else addState(car, i);

                for (int y = pos + car.carLength, i = 1; y < sharedData.MapHeight; ++y, ++i)
                    if (sharedData.IsOccupied(x, y, state)) break;
                    else addState(car, i);
            }

        }

        private bool iterate()
        {
            state = sharedData.GetNextState();
            if (sharedData.IsSolved || state == null)
                return false;

            nextStates.Clear();

            foreach (CarInfo car in sharedData.gameData.cars)
            {
                testCarMovements(car);

                // test if this (or any other) thread has found a solution
                if (sharedData.IsSolved)
                    break;
            }

            if (!sharedData.IsSolved)
            {
                // add all new options to the shared queue
                foreach (GameState newState in nextStates)
                    sharedData.TryPutState(newState);
            }
            return true;
        }

        public void Begin()
        {
            while (iterate()) ;
        }
    }

}
