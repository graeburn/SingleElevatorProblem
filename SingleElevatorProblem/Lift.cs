using System;
using System.Collections.Generic;
using System.Text;

/**
 * Lift class relating to lift movement between each floor, and dropOffs
 * from requests made by those inside the lift (stored in a priority queue).
 */

namespace AVAMAE_LiftExercise
{
	public class Lift
	{
		// Array allows setting which floors are served by the lift
		public int[] floorsServed { get; set; }

		public int maximumCapacity { get; set; }

        public int currentFloor { get; set; }

		public List<int> peopleOnBoard { get; set; }

		// Customer comparator and priority queue to store floor dropOff requests
		public RequestCompare compareFloorRequests = new RequestCompare(0,0);
		public PriorityQueue<Person, Person> dropOffFloorRequest;   

        public enum movementState
		{
			Idle = 0,
			Up = 1,
			Down = -1
		}

        public movementState currentLiftstate { get; set; }


        public Lift(int[] liftFloorsServed, int liftCapacity)
		{
			floorsServed = liftFloorsServed;
			maximumCapacity = liftCapacity;
			currentFloor = floorsServed[0];
			peopleOnBoard = new List<int>();
			currentLiftstate = movementState.Idle;
            dropOffFloorRequest = new PriorityQueue<Person, Person>(compareFloorRequests);
			compareFloorRequests.floorsServed = floorsServed.Count();
        }

        /**
		  * Lift moves between the building floors served, stored in an array
		*/
        public void MoveLiftToNextFloor(movementState state)
		{
			if(state == movementState.Up && currentFloor < floorsServed[floorsServed.Count() - 1])
			{
				currentFloor = floorsServed[Array.IndexOf(floorsServed, currentFloor) + 1];
				compareFloorRequests.currentFloor = currentFloor;
			}
            else if (state == movementState.Down && currentFloor > floorsServed[0])
            {
                currentFloor = floorsServed[Array.IndexOf(floorsServed, currentFloor) - 1];
                compareFloorRequests.currentFloor = currentFloor;
            }
        }

        /**
		  * Added for testing to print IDs of people currently in the lift
		*/
        public string PrintPeopleOnBoard()
        {
			StringBuilder allPeopleInLift = new StringBuilder("(");
            foreach(int person in peopleOnBoard)
			{
				allPeopleInLift.Append(person + " ");
			}
            if (allPeopleInLift.Length > 1)
                allPeopleInLift.Remove(allPeopleInLift.Length - 1, 1);
            allPeopleInLift.Append(")");
			return allPeopleInLift.ToString();
        }
    }
}

