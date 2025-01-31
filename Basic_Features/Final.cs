void TotalAirlineFee()
{
    try
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Total Airline Fees for the Day");
        Console.WriteLine("=============================================");

        List<Flight> unassignedFlights = terminal.Flights.Values
            .Where(flight => flight.BoardingGate == null || flight.BoardingGate.Equals(""))
            .ToList();

        if (unassignedFlights.Count > 0)
        {
            Console.WriteLine("Some flights have no boarding gate assigned. Please assign all flights first.");
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

                if (flight.Destination == "Singapore (SIN)") 
                {
                    fee += 500; 
                }
                if (flight.Origin == "Singapore (SIN)") 
                {
                    fee += 800; 
                }

                if (flight.SpecialRequestCode == "DDJB") fee += 300;
                if (flight.SpecialRequestCode == "CFFT") fee += 150;
                if (flight.SpecialRequestCode == "LWTT") fee += 500;

                if (flight.FlightNumber.Length < 2)
                {
                    Console.WriteLine($"Warning: Invalid Flight Number format for {flight.FlightNumber}. Skipping.");
                    continue;
                }

                string airlineCode = new string(new char[] { flight.FlightNumber[0], flight.FlightNumber[1] });
                string airlineName = GetAirlineNameFromCode(airlineCode);

                if (!airlineFees.ContainsKey(airlineName))
                {
                    airlineFees[airlineName] = 0;
                }
                airlineFees[airlineName] += fee;

                if (!airlineFlightCounts.ContainsKey(airlineName))
                {
                    airlineFlightCounts[airlineName] = 0;
                }
                airlineFlightCounts[airlineName]++;

                totalFees += fee;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Flight {flight.FlightNumber}: {ex.Message}");
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

                    string airlineCode = new string(new char[] { flight.FlightNumber[0], flight.FlightNumber[1] });
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
                        if (string.IsNullOrEmpty(flight.SpecialRequestCode))
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

        Console.WriteLine("{0,-20} {1,-15} {2,-15}", "Airline Name", "Total Fees", "Discount Applied");
        foreach (var entry in airlineFees)
        {
            Console.WriteLine("{0,-20} ${1,-14:F2} ${2,-14:F2}", entry.Key, airlineFees[entry.Key], airlineDiscounts[entry.Key]);
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
        Console.WriteLine($"Unexpected Error: {ex.Message}");
    }
    finally
    {
        Console.WriteLine("Done processing airline fees.");
    }
}
