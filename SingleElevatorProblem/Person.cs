using System;
using CsvHelper.Configuration.Attributes;

/**
 * Record format for importing the input data from the test CSV data provided.
 * The file is stored locally as 'ExerciseData.csv'.
 */

namespace AVAMAE_LiftExercise
{
    public class Person
    {
        [Name("Person ID")]
        public int personID { get; set; }

        [Name("At Floor")]
        public int atFloor { get; set; }

        [Name("Going to Floor")]
        public int goingToFloor { get; set; }

        [Name("Time")]
        public int time { get; set; }
    }
}

