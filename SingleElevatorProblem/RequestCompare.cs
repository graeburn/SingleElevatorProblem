using System;

/**
 * Custom comparator for comparing dropOff floor requests added to the lift's
 * priority queue.
 */

namespace AVAMAE_LiftExercise
{
    public class RequestCompare : IComparer<Person>
    {
        public int currentFloor { get; set; }
        public int movementState { get; set; }
        public int floorsServed { get; set; }

        public int Compare(Person? p1, Person? p2)
        {
            if (p1 != null && p1.goingToFloor == currentFloor && p2 != null && p2.goingToFloor != currentFloor)
                return int.MinValue;
            else if (p2 != null && p2.goingToFloor == currentFloor && p1 != null && p1.goingToFloor != currentFloor)
                return int.MaxValue;
            else if (movementState == 1 && p1 != null && p2 != null)
            {
                int score1 = (p1.goingToFloor > currentFloor) ? (p1.goingToFloor - currentFloor - floorsServed) : (floorsServed - (p1.goingToFloor - currentFloor));
                int score2 = (p2.goingToFloor > currentFloor) ? (p2.goingToFloor - currentFloor - floorsServed) : (floorsServed - (p2.goingToFloor - currentFloor));
                return score1 - score2;
            }
            else if (movementState == -1 && p1 != null && p2 != null)
            {
                int score1 = (p1.goingToFloor < currentFloor) ? (currentFloor - p1.goingToFloor - floorsServed) : (floorsServed - (currentFloor - p1.goingToFloor));
                int score2 = (p2.goingToFloor < currentFloor) ? (currentFloor - p2.goingToFloor - floorsServed) : (floorsServed - (currentFloor - p2.goingToFloor));
                return score1 - score2;
            }
            else if (movementState == 0 && p1 != null && p2 != null)
                return Math.Abs(p1.goingToFloor - currentFloor) - Math.Abs(p2.goingToFloor - currentFloor);

            return int.MaxValue;
        }

        public RequestCompare(int floor, int state)
        {
            currentFloor = floor;
            movementState = state;
        }

    }
}

