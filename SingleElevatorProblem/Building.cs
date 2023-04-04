using System;

/**
 * A generic building class used for the MedicineChest office building.
 * Could be extended to add heuristics relating to particular building types.
 */

namespace AVAMAE_LiftExercise
{
	public class Building
	{
        public int numberOfFloors { get; set; }

		public Lift lift;

        public Building(int buildingFloors, Lift buildinglift)
		{
			numberOfFloors = buildingFloors;
			lift = buildinglift;
		}
	}
}

