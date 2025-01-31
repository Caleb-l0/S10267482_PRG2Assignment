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

 void TotalAirlineFee()
{
    try
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Total Airline Fees for the Day");
        Console.WriteLine("=============================================");

        List<Flight> unassignedFlights = new List<Flight>();

        foreach (var flight in terminal.Flights.Values)
        {
            if (flight.BoardingGate == null || flight.BoardingGate == "")
            {
                unassignedFlights.Add(flight);
            }
        }

        if (unassignedFlights.Count > 0)
        {
            Console.WriteLine("Some flights are not assigned a boarding gate. Please assign all flights before calculating fees.");
            return;
        }

        Dictionary<string, double> airlineFees = new Dictionary<string, double>();
        Dictionary<string, double> airlineDiscounts = new Dictionary<string, double>();
        Dictionary<string, int> airlineFlightCounts = new Dictionary<string, int>();

        double totalFees = 0;
        double totalDiscounts = 0;

        foreach (var flight in terminal.Flights.Values)
        {
            try
            {
                double fee = 300; 

                if (flight.Destination == "Singapore (SIN)") fee += 500;
                if (flight.Origin == "Singapore (SIN)") fee += 800;

                if (flight.SpecialRequestCode == "DDJB") fee += 300;
                if (flight.SpecialRequestCode == "CFFT") fee += 150;
                if (flight.SpecialRequestCode == "LWTT") fee += 500;

                if (flight.FlightNumber.Length < 2)
                {
                    Console.WriteLine($"Warning: Invalid flight number format for {flight.FlightNumber}. Skipping.");
                    continue;
                }

                char[] flightCodeArray = flight.FlightNumber.ToCharArray();
                string airlineCode = flightCodeArray[0].ToString() + flightCodeArray[1].ToString();
                string airlineName = GetAirlineNameFromCode(airlineCode);

                if (!airlineFlightCounts.ContainsKey(airlineName))
                {
                    airlineFlightCounts[airlineName] = 0;
                }
                airlineFlightCounts[airlineName]++;

                if (!airlineFees.ContainsKey(airlineName))
                {
                    airlineFees[airlineName] = 0;
                }
                airlineFees[airlineName] += fee;

                totalFees += fee;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing flight {flight.FlightNumber}: {ex.Message}");
            }
        }

        foreach (var airline in airlineFees.Keys)
        {
            try
            {
                double discount = 0;
                int flightCount = airlineFlightCounts[airline];

                discount += (flightCount / 3) * 350;

                foreach (var flight in terminal.Flights.Values)
                {
                    if (flight.FlightNumber.Length < 2) continue;
                    char[] flightCodeArray = flight.FlightNumber.ToCharArray();
                    string airlineCode = flightCodeArray[0].ToString() + flightCodeArray[1].ToString();
                    string currentAirline = GetAirlineNameFromCode(airlineCode);

                    if (currentAirline == airline)
                    {
                        if (flight.ExpectedTime.Hour < 11 || flight.ExpectedTime.Hour >= 21)
                        {
                            discount += 110;
                        }
                      
                        if (flight.Origin == "Dubai (DXB)" || flight.Origin == "Bangkok (BKK)" || flight.Origin == "Tokyo (NRT)")
                        {
                            discount += 25;
                        }
                      
                        if (!(flight.SpecialRequestCode != null && flight.SpecialRequestCode != ""))
                        {
                            discount += 50;
                        }
                    }
                }

                if (flightCount > 5)
                {
                    discount += airlineFees[airline] * 0.03;
                }

                airlineFees[airline] -= discount;
                airlineDiscounts[airline] = discount;
                totalDiscounts += discount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying discount for airline {airline}: {ex.Message}");
            }
        }

        Console.WriteLine("{0,-20} {1,-15} {2,-15}", "Airline Name", "Original Fees", "Discount Applied");
        foreach (var entry in airlineFees)
        {
            try
            {
                Console.WriteLine("{0,-20} ${1,-14:F2} ${2,-14:F2}", entry.Key, totalFees, totalDiscounts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying data for airline {entry.Key}: {ex.Message}");
            }
        }

        double finalTotalFees = totalFees - totalDiscounts;
        double discountPercentage = (totalDiscounts / totalFees) * 100;

        Console.WriteLine("\n=============================================");
        Console.WriteLine($"Total Fees Charged: ${totalFees:F2}");
        Console.WriteLine($"Total Discounts Applied: ${totalDiscounts:F2}");
        Console.WriteLine($"Final Fees Collected: ${finalTotalFees:F2}");
        Console.WriteLine($"Discount Percentage: {discountPercentage:F2}%");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }
    finally
    {
        Console.WriteLine("Done processing airline fees.");
    }
}

