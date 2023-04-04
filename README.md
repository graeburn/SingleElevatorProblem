# Single Elevator Problem Solution

A test project aiming to optimise the elevator operation for a building containing one lift. It is assumed that there is one call button on each floor and the lift has a limited capacity.

The current project produces two output CSV files, each including the following categories:

## 1. ‘LiftStops.csv’ - A record of the floors the elevator stopped at

Column 1 - The time the elevator stopped.
Column 2 - The floor stopped at.
Column 3 - The passengers in the elevator after it had let people out.
Column 4 - The remaining floors with passengers waiting to be picked up. 
Column 5 - An ordered list of the remaining floors to drop off passengers at.

## 2. ‘RequestsCompleted.csv’ - A record of how long it took to complete each passenger's request

Column 1 - The passenger's ID.
Column 2 - The time the passenger made the request.
Column 3 - The time the elevator picked up the passenger.
Column 4 - The time the passenger was dropped off.
Column 5 - The time the passenger spent in the lift (Column 4 - Column 3).
Column 6 - The time taken to complete the request from pickup to drop off (Column 4 - Column 2).

## The project includes the following classes
### 1. Building
This class has variables for the number of floors in the building and a lift object. 

### 2. Lift
This class contains the floors the elevator serves, its maximum capacity, current floor, and current movement state (Up, Down or Idle). A priority queue ranks the drop offs requests made from inside the elevator, based on how near they were to the current floor and the direction of travel. A method is also included to move the elevator between the served floors.

### 3. RequestCompare
This class represented the custom comparator used to sort the drop off requests from passengers wanting to go to a particular floor. This involved scoring the different requests based on how near they were, the total number of floors, and the current direction of travel. This queue was resorted at each floor stopped at, after passengers got on and off.

### 4. LiftOperation
This class controlled which floor the elevator went to next by comparing the drop off requests from those on board, to the pickup requests from those passengers waiting on different floors. It currently prioritises dropping off passengers already on board, unless this involved changing direction. In this case it would pick up waiting passengers in the same direction of travel first. 

### 5. TestLiftOperation
This class tests the elevator using a set of test data.

### 6. Person
A class to format the test data.

### 7. LiftStopRecords
A class to format the data associated with each floor stop the elevator makes.

### 8. PassengerRecords
A class to format the data associated with how each passenger's elevator call was handled.
