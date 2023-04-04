using System;
using CsvHelper.Configuration.Attributes;

/**
 *  Format for the output data relating to the time to complete each person's 
 *  lift request. Shown in the 'RequestsCompleted.csv' file for the results
 *  run on the CSV test data.
 */

namespace AVAMAE_LiftExercise
{
    public class PassengerRecords
    {
        [Name("Person ID")]
        public int passengerID { get; set; }

        [Name("Time of button press")]
        public int buttonTime { get; set; }

        [Name("PickUp time")]
        public int pickupTime { get; set; }

        [Name("DropOff time")]
        public int dropOffTime { get; set; }

        [Name("Time in lift")]
        public int liftTime { get; set; }

        [Name("Total request completion time")]
        public int completionTime { get; set; }
    }
}

