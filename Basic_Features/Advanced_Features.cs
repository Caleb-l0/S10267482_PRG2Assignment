class program
{
  void UnassignedFlights() //s102674822
  {
    Queue<Flight> unassignedFlights = new Queue<Flight>();

     foreach (var flight in terminal.Flights.Values)
     {
     // Check if the flight is not currently assigned to any boarding gate
     bool isAssigned = false;
     foreach (var gate in terminal.BoardingGates.Values)
     {
         if (gate.Flight == flight)
         {
             isAssigned = true;
             break;
         }
     }
     if (!isAssigned)
     {
         unassignedFlights.Enqueue(flight);
     }
   }

    Console.WriteLine($"Total number of Flights without Boarding Gate assigned: {unassignedFlights.Count}");

    List<BoardingGate> unassignedGates = new List<BoardingGate>();

    foreach (var gate in terminal.BoardingGates.Values)
    {
        if (gate.Flight == null)
        {
            unassignedGates.Add(gate);
        }
    }

    Console.WriteLine($"Total number of Boarding Gates without a Flight assigned: {unassignedGates.Count}");

    int flightsAssigned = 0;
    int gatesAssigned = 0;

    while (unassignedFlights.Count > 0)
    {
        var flight = unassignedFlights.Dequeue();
        BoardingGate assignedGate = null;
        bool gateFound = false;

        foreach (var gate in unassignedGates)
        {
            if (flight.SpecialRequestCode == "DDJB" && gate.DDJB)
            {
                assignedGate = gate;
                gateFound = true;
                break;
            }
            else if (flight.SpecialRequestCode == "CFFT" && gate.CFFT)
            {
                assignedGate = gate;
                gateFound = true;
                break;
            }
            else if (flight.SpecialRequestCode == "LWTT" && gate.LWTT)
            {
                assignedGate = gate;
                gateFound = true;
                break;
            }
        }

        if (assignedGate != null && gateFound)
        {
            assignedGate.Flight = flight; 
            unassignedGates.Remove(assignedGate);
            flightsAssigned++;
            gatesAssigned++;

            Console.WriteLine($"Assigned Boarding Gate {assignedGate.GateNumber} to Flight {flight.FlightNumber}");
            DisplayAirlinesFlights();
        }
        else
        {
            Console.WriteLine($"No available boarding gate for Flight {flight.FlightNumber}.");
        }
    }

    Console.WriteLine($"Total number of Flights and Boarding Gates processed and assigned: {flightsAssigned}");
    Console.WriteLine($"Percentage of Flights and Boarding Gates processed and assigned: {(flightsAssigned * 100) / gatesAssigned}%");
    Console.WriteLine($"Total number of Flights and Boarding Gates processed and assigned: {flightsAssigned}/{gatesAssigned}");
  }


  //second feature//
  void TotalAirlineFee
  {
    
  
  }


}
