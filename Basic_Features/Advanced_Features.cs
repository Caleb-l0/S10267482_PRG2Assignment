class program
{
  public void UnassignedFlights() //s102674822
{
    try
    {
        Queue<Flight> unassignedFlights = new Queue<Flight>();


        //Check for unassigned flights//
        foreach (var flight in terminal.Flights.Values)
        {
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


        
        //Check for unassigned gates//
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


        //Assigning Flights to Boarding Gates//
        while (unassignedFlights.Count > 0)
        {
            var flight = unassignedFlights.Dequeue();
            BoardingGate assignedGate = null;
            bool gateFound = false;

            foreach (var gate in unassignedGates)
            {
                try
                {
                    if (flight is NORMFlight && assignedGate == null)
                    {
                        assignedGate = gate;
                        gateFound = true;
                        break;
                    }

                    else if (flight is DDJBFlight && gate.SupportsDDJB)
                    {
                        assignedGate = gate;
                        gateFound = true;
                        break;
                    }
                    else if (flight is CFFTFlight && gate.SupportsCFFT)
                    {
                        assignedGate = gate;
                        gateFound = true;
                        break;
                    }
                    else if (flight is LWTTFlight && gate.SupportsLWTT)
                    {
                        assignedGate = gate;
                        gateFound = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while checking gate for flight {flight.FlightNumber}: {ex.Message}");
                }
            }

            if (assignedGate != null && gateFound)
            {
                try
                {
                    assignedGate.Flight = flight;
                    unassignedGates.Remove(assignedGate);
                    flightsAssigned++;
                    gatesAssigned++;

                    Console.WriteLine($"Assigned Boarding Gate {assignedGate.GateName} to Flight {flight.FlightNumber}");
                    DisplayAirlinesFlights();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error assigning gate {assignedGate.GateName} to flight {flight.FlightNumber}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"No available boarding gate for Flight {flight.FlightNumber}.");
            }
        }

        
        int totalGatesAssigned = gatesAssigned;

        if (totalGatesAssigned == 0)
        {
            totalGatesAssigned = 1; 
        }

        double percentageAssigned = (flightsAssigned * 100.0) / totalGatesAssigned;

        Console.WriteLine($"Total number of Flights and Boarding Gates processed and assigned: {flightsAssigned}");
        Console.WriteLine($"Percentage of Flights and Boarding Gates processed and assigned: {percentageAssigned}%");
    }
   
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }
    finally
    {
        Console.WriteLine("Done.");
    }
}

  //second feature//
  void TotalAirlineFee
  {
    
  
  }


}
