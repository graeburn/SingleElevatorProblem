using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

/**
 * LiftOperator to stop the lift at each floor based on the pickUp and 
 * dropOff requests made by those waiting and the people already on board.
 */

namespace AVAMAE_LiftExercise
{
	public class LiftOperation
	{
		public Building building { get; set; }

		public bool[] callFloorRequests { get; set; }

		public List<Person> requestsWaitingForPickup = new List<Person>();

        public int nextFloorRequest = -1;

        // Extra variables used for testing and printing completed requests to an output file
        public List<PassengerRecords> completedPassengerRequestsList = new List<PassengerRecords>();
        public List<Person> priorityQueueToPrint = new List<Person>();

        public LiftOperation(Building currentBuilding)
		{
			building = currentBuilding;
			callFloorRequests = new bool[building.numberOfFloors];
        }

        /**
         * Assign next floor request to stop at from both pickUp and dropOff requests made.
         * Lift will continue in current direction unless no further requests in that direction.
        */
        public void AssignNextLiftRequest()
		{
            if (building.lift.dropOffFloorRequest.Count == 0 && !callFloorRequests.Contains(true))
            {
                building.lift.currentLiftstate = Lift.movementState.Idle;
                return;
            }
            else if(building.lift.dropOffFloorRequest.Count == 0 && callFloorRequests.Contains(true) && building.lift.currentLiftstate == Lift.movementState.Up)
            {
                if (SearchForPickUpsAbove(nextFloorRequest) > nextFloorRequest)
                    return;
                else
                    building.lift.currentLiftstate = Lift.movementState.Down;
            }
            else if (building.lift.dropOffFloorRequest.Count == 0 && callFloorRequests.Contains(true) && building.lift.currentLiftstate == Lift.movementState.Down)
            {
                if (SearchForPickUpsBelow(nextFloorRequest) < nextFloorRequest)
                    return;
                else
                    building.lift.currentLiftstate = Lift.movementState.Up;
            }
            else if (building.lift.dropOffFloorRequest.Count == 0 && callFloorRequests.Contains(true) && building.lift.currentLiftstate == Lift.movementState.Idle)
            {
                int nearestFloorUp = building.lift.floorsServed.Count();
                int nearestFloorDown = building.lift.floorsServed.Count();

                nearestFloorUp = SearchForPickUpsAbove(nextFloorRequest) - building.lift.currentFloor;
                nearestFloorDown = building.lift.currentFloor - SearchForPickUpsBelow(nextFloorRequest);

                building.lift.currentLiftstate = (nearestFloorUp <= nearestFloorDown) ? Lift.movementState.Up : Lift.movementState.Down;
            }
            else if(building.lift.dropOffFloorRequest.Count > 0)
			{
                nextFloorRequest = building.lift.dropOffFloorRequest.Peek().goingToFloor;
     
                if (building.lift.currentLiftstate == Lift.movementState.Up && nextFloorRequest < building.lift.currentFloor)
                {
                    if (SearchForPickUpsAbove(nextFloorRequest) > nextFloorRequest)
                        return;
                    else
                        building.lift.currentLiftstate = Lift.movementState.Down;
                }
                else if (building.lift.currentLiftstate == Lift.movementState.Down && nextFloorRequest > building.lift.currentFloor)
                {
                    if (SearchForPickUpsBelow(nextFloorRequest) < nextFloorRequest)
                        return;
                    else
                        building.lift.currentLiftstate = Lift.movementState.Up;
                }
            }		
        }

        /**
         * Search for nearest floor above current one with a pickUp Request
        */
        public int SearchForPickUpsAbove(int nextFloorRequest)
        {
            for (int i = Array.IndexOf(building.lift.floorsServed, building.lift.currentFloor); i < building.lift.floorsServed.Count(); i++)
            {
                if (callFloorRequests[i] == true)
                {
                    nextFloorRequest = building.lift.floorsServed[i];
                    return nextFloorRequest;
                }
            }
            return nextFloorRequest;
        }

        /**
         * Search for nearest floor below current one with a pickUp Request
        */
        public int SearchForPickUpsBelow(int nextFloorRequest)
        {
            for (int i = Array.IndexOf(building.lift.floorsServed, building.lift.currentFloor); i >= 0; i--)
            {
                if (callFloorRequests[i] == true)
                {
                    nextFloorRequest = building.lift.floorsServed[i];
                    return nextFloorRequest;
                }
            }
            return nextFloorRequest;
        }

        /**
         * Returns TRUE if lift needs to stop at the approaching floor or FALSE 
         * to continue moving to the next floor the lift serves
        */
        public bool CheckIfLiftStoppingAtFloor(int floor)
        {
            if (building.lift.dropOffFloorRequest.Count > 0)
            {
                if (building.lift.dropOffFloorRequest.Peek().goingToFloor == floor)
                {
                    return true;
                }
            }
            if (callFloorRequests[Array.IndexOf(building.lift.floorsServed, floor)] == true && building.lift.peopleOnBoard.Count < building.lift.maximumCapacity)
                return true;
            else if (callFloorRequests[Array.IndexOf(building.lift.floorsServed, floor)] == true && building.lift.peopleOnBoard.Count == building.lift.maximumCapacity)
                return false;

            return false;
        }

        /**
         * If the lift stops at the current floor, update the passengers who get on and off.
         * Re-sort the lift's priority queue based on the new passengers' dropOff floor requests.
        */
        public void UpdateCompletedRequests(int timer)
        {
            DropOffPassengers(timer);

            LoadPassengers(timer);

            // Added for testing to get order of floor dropOffs stored in the priority queue
            UnpackAndRepackPriorityQueue();
        }

        /**
          * Remove passengers at current floor from both the lift and the dropOff
          * dropOff requests priority queue. Record dropOff time.
        */
        public void DropOffPassengers(int timer)
        {
            while (building.lift.dropOffFloorRequest.Count > 0)
            {
                if (building.lift.dropOffFloorRequest.Peek().goingToFloor == building.lift.currentFloor)
                {
                    completedPassengerRequestsList.ElementAt(building.lift.dropOffFloorRequest.Peek().personID - 1).dropOffTime = timer;
                    building.lift.peopleOnBoard.Remove(building.lift.dropOffFloorRequest.Dequeue().personID);
                }
                else
                    break;
            }
        }

        /**
          * Load passengers waiting whilst lift not at full capacity and add their dropOff
          * requests to the priority queue
        */
        public void LoadPassengers(int timer)
        {
            if (callFloorRequests[Array.IndexOf(building.lift.floorsServed, building.lift.currentFloor)] == true && building.lift.peopleOnBoard.Count < building.lift.maximumCapacity)
            {
                List<Person> temporaryPassengerList = new List<Person>();

                while (building.lift.dropOffFloorRequest.Count > 0)
                {
                    temporaryPassengerList.Add(building.lift.dropOffFloorRequest.Dequeue());
                }

                while (requestsWaitingForPickup.Find(person => person.atFloor == building.lift.currentFloor) != null && building.lift.peopleOnBoard.Count < building.lift.maximumCapacity)
                {
                    Person? p = requestsWaitingForPickup.Find(person => person.atFloor == building.lift.currentFloor);
                    if (p != null)
                    {
                        temporaryPassengerList.Add(p);

                        completedPassengerRequestsList.ElementAt(p.personID - 1).pickupTime = timer;
                        building.lift.peopleOnBoard.Add(p.personID);

                        requestsWaitingForPickup.Remove(p);
                        callFloorRequests[Array.IndexOf(building.lift.floorsServed, building.lift.currentFloor)] = false;
                    }
                }

                OrderDropOffRequests(temporaryPassengerList);
            }
        }

        /**
          * Re-sort priority queue based on dropOff requests from new passengers on board lift
        */
        public void OrderDropOffRequests(List<Person> temporaryPassengerList)
        {
            temporaryPassengerList.ForEach(p => building.lift.dropOffFloorRequest.Enqueue(p, p));

            if (requestsWaitingForPickup.Find(person => person.atFloor == building.lift.currentFloor) != null && building.lift.peopleOnBoard.Count == building.lift.maximumCapacity)
            {
                callFloorRequests[Array.IndexOf(building.lift.floorsServed, building.lift.currentFloor)] = true;
            }
        }

        /**
         * Added for testing to unpack and repack priority  queue to print order of requests stored
        */
            public void UnpackAndRepackPriorityQueue()
        {
            priorityQueueToPrint.Clear();
            while (building.lift.dropOffFloorRequest.Count > 0)
            {
                priorityQueueToPrint.Add(building.lift.dropOffFloorRequest.Dequeue());
            }
            priorityQueueToPrint.ForEach(p => building.lift.dropOffFloorRequest.Enqueue(p, p));
        }

        /**
         * Added for testing to print ordered pickUp floor requests from those waiting for lift
        */
        public string PrintPickUpRequests()
        {
            StringBuilder allPickUpRequests = new StringBuilder("(");
            for(int i = 0; i < callFloorRequests.Count(); i++)
            {
                if (callFloorRequests[i] == true)
                    allPickUpRequests.Append((i+1) + " ");
            }
            if (allPickUpRequests.Length > 1)
                allPickUpRequests.Remove(allPickUpRequests.Length - 1, 1);
            allPickUpRequests.Append(")");
            return allPickUpRequests.ToString();
        }

        /**
         * Added for testing to print ordered dropOff floor requests from those inside lift
        */
        public string PrintDropOffRequests()
        {
            StringBuilder allDropOffRequests = new StringBuilder("(");
            for (int i = 0; i < priorityQueueToPrint.Count(); i++)
            {
                allDropOffRequests.Append(priorityQueueToPrint.ElementAt(i).goingToFloor + " ");
            }
            if (allDropOffRequests.Length > 1)
                allDropOffRequests.Remove(allDropOffRequests.Length - 1, 1);
            allDropOffRequests.Append(")");
            return allDropOffRequests.ToString();
        }
    }
}

