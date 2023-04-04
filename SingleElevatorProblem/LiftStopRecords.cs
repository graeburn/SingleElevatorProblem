using System;
using CsvHelper.Configuration.Attributes;

/**
 *  Format for the output data relating to each floor stop the lift makes. 
 *  Shown in the 'LiftStops.csv' file for the results
 *  run on the CSV test data.
 */

namespace AVAMAE_LiftExercise
{
    public class LiftStopRecords
    {
        [Name("Time of floor stop")]
        public int currentTime { get; set; }

        [Name("Floor number")]
        public int currentFloor { get; set; }

        [Name("People IDs in Lift")]
        public string? peopleInLift { get; set; }

        [Name("Floors waiting for pickups")]
        public string? remainingCallRequests { get; set; }

        [Name("Floor dropOff order")]
        public string? remainingDropOffRequests { get; set; }
    }
}

