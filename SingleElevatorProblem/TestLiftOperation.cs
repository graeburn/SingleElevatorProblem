using System;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;

/**
 * The lift operation is tested here using the provided CSV file data renamed here
 * as 'ExerciseData.csv'. The code here assumes it takes 10s to move between floors 
 * and 1s stopped at each floor to pickUp and dropOff passengers.
 */

namespace AVAMAE_LiftExercise
{
	public class TestLiftOperation
	{
        private int timer = 1;

        private int nextFloorArrivalTime = 0;

        // List to store test CSV file data
        private List<Person>? allPassengerRequestsList;

        // List to store records of each time the lift stops at a floor
        List<LiftStopRecords> completedRequestsList = new List<LiftStopRecords>();

        // Enter path to the ExerciseData.csv file stored in the ExerciseData folder
        private string testDataFilePath = " ";

        static void Main(string[] args)
        {
            TestLiftOperation liftTest = new TestLiftOperation();

            Lift medicineChestLift = new Lift(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 8);

            Building medicineChestBuilding = new Building(10, medicineChestLift);

            LiftOperation medicineChestLiftOperation = new LiftOperation(medicineChestBuilding);

            liftTest.allPassengerRequestsList = liftTest.CreateRequestList(liftTest.testDataFilePath);

            // Initialise output file to store passenger requests completed
            liftTest.InitialiseOutputFile(medicineChestLiftOperation, liftTest);

            do
            {
                liftTest.CheckNextLiftCall(medicineChestLiftOperation, liftTest, medicineChestLift);

                liftTest.MoveAndStopLiftAtFloors(medicineChestLiftOperation, liftTest, medicineChestLift);

                liftTest.timer++;

            } while (liftTest.allPassengerRequestsList.Any() || medicineChestLift.dropOffFloorRequest.Count > 0);

            // Write output files for all lift stops made and passenger requests completed
            liftTest.WriteCompletedLiftStopsFile(liftTest, liftTest.testDataFilePath);
            CompletePassengerRequestsFileData(medicineChestLiftOperation, liftTest);
            liftTest.WriteCompletedRequestsFile(medicineChestLiftOperation, liftTest.testDataFilePath);

        }

        /**
          * Import test CSV file data
        */
        public List<Person> CreateRequestList(string testDataFilePath)
        {
            //using (var streamReader = new StreamReader(@"/Users/g/Projects/AVAMAE_LiftExercise/AVAMAE_LiftExercise/ExerciseData/ExerciseData.csv"))
            using (var streamReader = new StreamReader(@testDataFilePath+"ExerciseData.csv"))
            {
                using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    return csvReader.GetRecords<Person>().ToList();
                }
            }
        }

        /**
          * Initialise file to store data about each passenger's completed lift request
        */
        public void InitialiseOutputFile(LiftOperation liftOperation, TestLiftOperation liftTest)
        {
            if (liftTest.allPassengerRequestsList != null)
            {
                for (int i = 0; i < liftTest.allPassengerRequestsList.Count; i++)
                {
                    liftOperation.completedPassengerRequestsList.Add(new PassengerRecords() {   passengerID = liftTest.allPassengerRequestsList.ElementAt(i).personID,
                                                                                                buttonTime = liftTest.allPassengerRequestsList.ElementAt(i).time,
                                                                                                pickupTime = 0,
                                                                                                dropOffTime = 0,
                                                                                                liftTime = 0,
                                                                                                completionTime = 0 });
                }
            }
                
        }

        /**
          * Fill in data on time taken to complete each passenger's lift request
        */
        public static void CompletePassengerRequestsFileData(LiftOperation liftOperation, TestLiftOperation liftTest)
        {
            if (liftTest.allPassengerRequestsList != null)
            {
                for (int i = 0; i < liftOperation.completedPassengerRequestsList.Count; i++)
                {
                    liftOperation.completedPassengerRequestsList.ElementAt(i).liftTime = liftOperation.completedPassengerRequestsList.ElementAt(i).dropOffTime - liftOperation.completedPassengerRequestsList.ElementAt(i).pickupTime;
                    liftOperation.completedPassengerRequestsList.ElementAt(i).completionTime = liftOperation.completedPassengerRequestsList.ElementAt(i).dropOffTime - liftOperation.completedPassengerRequestsList.ElementAt(i).buttonTime;
                }
            }

        }

        /**
          * Read next lift request call at the time when the person calls the lift
          * and update the liftOperator that people are waiting for pickUps
        */
        public void CheckNextLiftCall(LiftOperation liftOperation, TestLiftOperation liftTest, Lift lift)
        {
            if(liftTest.allPassengerRequestsList != null)
            {
                if (liftTest.allPassengerRequestsList.Count > 0 && liftTest.timer == liftTest.allPassengerRequestsList.First().time)
                {
                    while (liftTest.timer == liftTest.allPassengerRequestsList.First().time)
                    {
                        liftOperation.callFloorRequests[Array.IndexOf(lift.floorsServed, liftTest.allPassengerRequestsList.First().atFloor)] = true;
                        liftOperation.requestsWaitingForPickup.Add(liftTest.allPassengerRequestsList.First());
                        liftTest.allPassengerRequestsList.RemoveAt(0);

                        if (liftTest.allPassengerRequestsList.Count == 0)
                            break;
                    }
                }
            }
        }

        /**
          * Move the lift between floors, stopping for a pickUp (when not full) or a dropOff.
          * Adds a new record on stopping at floor to output to file.
        */
        public void MoveAndStopLiftAtFloors(LiftOperation liftOperation, TestLiftOperation liftTest, Lift lift)
        {
            const int liftMoveTime = 10;

            if (lift.currentLiftstate != Lift.movementState.Idle && liftTest.timer == liftTest.nextFloorArrivalTime)
            {
                if (liftOperation.CheckIfLiftStoppingAtFloor(lift.currentFloor) == false)
                {
                    lift.MoveLiftToNextFloor(lift.currentLiftstate);
                    liftTest.nextFloorArrivalTime = liftTest.timer + liftMoveTime;
                }
                else
                {
                    liftOperation.UpdateCompletedRequests(liftTest.timer);
                    liftTest.completedRequestsList.Add(new LiftStopRecords() {  currentTime = liftTest.timer,
                                                                                currentFloor = lift.currentFloor,
                                                                                peopleInLift = lift.PrintPeopleOnBoard(),
                                                                                remainingCallRequests = liftOperation.PrintPickUpRequests(),
                                                                                remainingDropOffRequests = liftOperation.PrintDropOffRequests() });
                }
            }
            else if (liftTest.timer > liftTest.nextFloorArrivalTime)
            {
                liftOperation.AssignNextLiftRequest();

                if (lift.currentLiftstate != Lift.movementState.Idle)
                {
                    lift.MoveLiftToNextFloor(lift.currentLiftstate);
                    liftTest.nextFloorArrivalTime = liftTest.timer + liftMoveTime;
                }
            }
        }

        /**
          * Create output file of each lift stop made
        */
        public void WriteCompletedLiftStopsFile(TestLiftOperation liftTest, string testDataFilePath)
        {
            var streamWriter = new StreamWriter(@testDataFilePath + "LiftStops.csv");
            var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            csvWriter.WriteRecords(liftTest.completedRequestsList);

            csvWriter.Dispose();
            streamWriter.Dispose();
        }

        /**
          * Create output file of time taken to complete each lift passenger's request
        */
        public void WriteCompletedRequestsFile(LiftOperation liftOperation, string testDataFilePath)
        {
            var streamWriter = new StreamWriter(@testDataFilePath + "RequestsCompleted.csv");
            var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            csvWriter.WriteRecords(liftOperation.completedPassengerRequestsList);

            csvWriter.Dispose();
            streamWriter.Dispose();
        }
    }
}

