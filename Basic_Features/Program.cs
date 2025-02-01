//==========================================================//
// Student Number : S10267482//
// Student Name : Caleeb Low//
// Partner Name : Winston Chin//
//==========================================================//

using System;
using System.Collections.Generic;
using System.IO;

public class Terminal
{
    // Terminal name property
    public string TerminalName { get; set; }

    // Dictionary storing all airlines operating at the terminal
    public Dictionary<string, Airline> Airlines { get; set; } = new Dictionary<string, Airline>();

    // Dictionary storing all flights scheduled at the terminal
    public Dictionary<string, Flight> Flights { get; set; } = new Dictionary<string, Flight>();

    // Dictionary storing all boarding gates available at the terminal
    public Dictionary<string, BoardingGate> BoardingGates { get; set; } = new Dictionary<string, BoardingGate>();

    // Dictionary storing fees associated with different gates
    public Dictionary<string, double> GateFees { get; set; } = new Dictionary<string, double>();

    public bool AddAirline(Airline airline)
    {
        try
        {
            if (!Airlines.ContainsKey(airline.Code))
            {
                Airlines[airline.Code] = airline;
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding airline {airline.Code}: {ex.Message}");
        }
        return false;
    }

    // Adds a new airline to the terminal 
    public bool AddBoardingGate(BoardingGate gate)
    {
        try
        {
            if (!BoardingGates.ContainsKey(gate.GateName))
            {
                BoardingGates[gate.GateName] = gate;
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding boarding gate: {ex.Message}");
        }
        return false;
    }

    // Retrieves the airline that owns a specific flight
    public Airline GetAirlineFromFlight(Flight flight)
    {
        try
        {
            foreach (var airline in Airlines.Values)
            {
                if (airline.Flights.ContainsKey(flight.FlightNumber))
                    return airline;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving airline for flight {flight.FlightNumber}: {ex.Message}");
        }
        return null;
    }

    // Printing the fees for all boarding gates
    public void PrintAirlineFees()
    {
        foreach (var fee in GateFees)
        {
            Console.WriteLine($"{fee.Key}: {fee.Value}");
        }
    }

    public override string ToString()
    {
        return $"Terminal: {TerminalName}, Airlines: {Airlines.Count}, Boarding Gates: {BoardingGates.Count}";
    }
}

public class BoardingGate
{
    public string GateName { get; set; }
    public bool SupportsCFFT { get; set; }
    public bool SupportsDDJB { get; set; }
    public bool SupportsLWTT { get; set; }
    public Flight Flight { get; set; }

    public BoardingGate(string gateName, bool supportsCFFT, bool supportsDDJB, bool supportsLWTT)
    {
        GateName = gateName;
        SupportsCFFT = supportsCFFT;
        SupportsDDJB = supportsDDJB;
        SupportsLWTT = supportsLWTT;
    }

    //Base fees for this specific gates
    public double CalculateFees()
    {
        double baseFee = 300;
        return baseFee;
    }

    public override string ToString()
    {
        return $"Gate: {GateName}, Supports CFFT: {SupportsCFFT}, Supports DDJB: {SupportsDDJB}, Supports LWTT: {SupportsLWTT}";
    }
}

public class Airline
{
    public string Name { get; set; }
    public string Code { get; set; }
    public Dictionary<string, Flight> Flights { get; set; } = new Dictionary<string, Flight>();

    public Airline(string name, string code)
    {
        Name = name;
        Code = code;
    }

    // Adding of flight to the airline's list of flights
    public bool AddFlight(Flight flight, Terminal terminal)
    {
        try
        {
            if (!Flights.ContainsKey(flight.FlightNumber))
            {
                Flights[flight.FlightNumber] = flight;
                terminal.Flights[flight.FlightNumber] = flight; // Add to Terminal's flight list as well
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding flight {flight.FlightNumber}: {ex.Message}");
        }
        return false;
    }


    //Total fees for this airline's flights
    public double CalculateFees()
    {
        double totalFees = 0;
        foreach (var flight in Flights.Values)
        {
            totalFees += flight.CalculateFees();
        }
        return totalFees;
    }

    //Removes a flight from the airline's list
    public bool RemoveFlight(Flight flight)
    {
        return Flights.Remove(flight.FlightNumber);
    }

    public override string ToString()
    {
        return $"Airline: {Name}, Code: {Code}, Flights: {Flights.Count}";
    }
}

public abstract class Flight : IComparable<Flight>
{
    protected double baseFee = 100.0;

    public string FlightNumber { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DateTime ExpectedTime { get; set; }
    public string Status { get; set; }
    public string SpecialRequestCode { get; set; }
    public string BoardingGate { get; set; }

    public int CompareTo(Flight other)
    {
        if (other == null) return 1;
        return this.ExpectedTime.CompareTo(other.ExpectedTime);
    }

    public abstract double CalculateFees();

    public override string ToString()
    {
        return $"Flight: {FlightNumber}, Origin: {Origin}, Destination: {Destination}, Status: {Status}, Expected Time: {ExpectedTime}";
    }
}

public class NORMFlight : Flight
{
    public override double CalculateFees()
    {
        return (Destination == "Singapore (SIN)" ? 500 : 800) + baseFee;
    }

    public override string ToString()
    {
        return base.ToString() + ", Type: NORMFlight";
    }
}

public class LWTTFlight : Flight
{
    public double RequestFee { get; set; }

    public override double CalculateFees()
    {
        return (Destination == "Singapore (SIN)" ? 500 : 800) + baseFee + 500;
    }

    public override string ToString()
    {
        return base.ToString() + $", Type: LWTTFlight, Request Fee: {RequestFee}";
    }
}

public class DDJBFlight : Flight
{
    public double RequestFee { get; set; }

    public override double CalculateFees()
    {
        return (Destination == "Singapore (SIN)" ? 500 : 800) + baseFee + 300;
    }

    public override string ToString()
    {
        return base.ToString() + $", Type: DDJBFlight, Request Fee: {RequestFee}";
    }
}

public class CFFTFlight : Flight
{
    public double RequestFee { get; set; }

    public override double CalculateFees()
    {
        return (Destination == "Singapore (SIN)" ? 500 : 800) + baseFee + 150;
    }

    public override string ToString()
    {
        return base.ToString() + $", Type: CFFTFlight, Request Fee: {RequestFee}";
    }
}

class Program
{
    private Terminal terminal = new Terminal();

    //Calculates total fees for all the flights in the terminal
    public void CalculateTotalFee()
    {
        try
        {
            double totalFees = 0;
            foreach (var flight in terminal.Flights.Values)
            {
                totalFees += flight.CalculateFees();
            }
            Console.WriteLine($"Total Fees: {totalFees}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calculating total fees: {ex.Message}");
        }
    }

    //Reads airline data from airlines.csv and adds airlines to the terminal
    void ReadAirlines()
    {
        try
        {
            string[] lines = File.ReadAllLines("airlines.csv");
            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                if (data.Length < 2)
                {
                    throw new FormatException($"Invalid data format in airlines.csv at line {i + 1}");
                }
                string airlineName = data[0];
                string airlineCode = data[1];

                Airline airline = new Airline(airlineName, airlineCode);

                //If airline already exists
                if (!terminal.AddAirline(airline))
                {
                    Console.WriteLine($"Airline with code {airlineCode} already exists.");
                }
            }
        }
        catch (FileNotFoundException fnfEx)
        {
            Console.WriteLine($"Error: The file 'airlines.csv' was not found. {fnfEx.Message}");
        }
        catch (FormatException formatEx)
        {
            Console.WriteLine($"Error: {formatEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading airlines: {ex.Message}");
        }
    }


    //Reads boarding gates data from boardinggates.csv and adds gates to the terminal
    void ReadBoardingGates()
    {
        try
        {
            string[] lines = File.ReadAllLines("boardinggates.csv");
            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                if (data.Length < 4)
                {
                    throw new FormatException($"Invalid data format in boardinggates.csv at line {i + 1}");
                }

                string gateName = data[0];
                bool supportsDDJB = data[1] == "True";
                bool supportsCFFT = data[2] == "True";
                bool supportsLWTT = data[3] == "True";

                BoardingGate gate = new BoardingGate(gateName, supportsCFFT, supportsDDJB, supportsLWTT);

                //If boarding gate data already exists
                if (!terminal.AddBoardingGate(gate))
                {
                    Console.WriteLine($"Boarding gate {gateName} already exists.");
                }
            }
        }
        catch (FileNotFoundException fnfEx)
        {
            Console.WriteLine($"Error: The file 'boardinggates.csv' was not found. {fnfEx.Message}");
        }
        catch (FormatException formatEx)
        {
            Console.WriteLine($"Error: {formatEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading boarding gates: {ex.Message}");
        }
    }
    //Displays a list of airlines and asks user for airline code 
    void DisplayAirlinesFlights()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("{0,-15} {1,-20}", "Airline Code", "Airline Name");

        Console.WriteLine("{0,-15} {1,-20}", "SQ", "Singapore Airlines");
        Console.WriteLine("{0,-15} {1,-20}", "MH", "Malaysia Airlines");
        Console.WriteLine("{0,-15} {1,-20}", "JL", "Japan Airlines");
        Console.WriteLine("{0,-15} {1,-20}", "CX", "Cathay Pacific");
        Console.WriteLine("{0,-15} {1,-20}", "QF", "Qantas Airways");
        Console.WriteLine("{0,-15} {1,-20}", "TR", "AirAsia");
        Console.WriteLine("{0,-15} {1,-20}", "EK", "Emirates");
        Console.WriteLine("{0,-15} {1,-20}", "BA", "British Airways");
        Console.Write("Enter Airline Code: ");
        string airlineCode = Console.ReadLine().Trim().ToUpper();

        string airlineName = GetAirlineNameFromCode(airlineCode);

        if (airlineName == "Unknown Airline")
        {
            Console.WriteLine("Invalid airline code. Please try again.");
            return;
        }

        Console.WriteLine("=============================================");
        Console.WriteLine("List of Flights for {0}", airlineName);
        Console.WriteLine("=============================================");
        Console.WriteLine("{0,-15} {1,-20} {2,-25} {3,-25} {4,-30}",
            "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

        bool hasFlights = false;
        foreach (var flight in terminal.Flights.Values)
        {
            string flightAirlineCode = flight.FlightNumber.Substring(0, 2).ToUpper();

            if (flightAirlineCode == airlineCode)
            {
                hasFlights = true;
                Console.WriteLine("{0,-15} {1,-20} {2,-25} {3,-25} {4,-30}",
                    flight.FlightNumber, airlineName, flight.Origin, flight.Destination, flight.ExpectedTime.ToString("d/M/yyyy hh:mm:ss tt"));
            }
        }

        if (!hasFlights)
        {
            Console.WriteLine("No flights found for {0}.", airlineName);
        }

        Console.WriteLine("\n\n\n\n\n");
    }

    //Match airline code with their respective airlines
    public static class AirlineHelper
    {
        public static string GetAirlineNameFromCode(string airlineCode)
        {
            switch (airlineCode)
            {
                case "SQ": return "Singapore Airlines";
                case "MH": return "Malaysia Airlines";
                case "JL": return "Japan Airlines";
                case "CX": return "Cathay Pacific";
                case "QF": return "Qantas Airways";
                case "TR": return "AirAsia";
                case "EK": return "Emirates";
                case "BA": return "British Airways";
                default: return "Unknown Airline";
            }
        }
    }

    //Displays a list of boarding gates
    void ListBoardingGates()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("{0,-15} {1,-20} {2,-20} {3,-20}", "Gate Name", "DDJB", "CFFT", "LWTT");

        foreach (var gate in terminal.BoardingGates.Values)
        {
            Console.WriteLine("{0,-15} {1,-20} {2,-20} {3,-20}", gate.GateName, gate.SupportsDDJB, gate.SupportsCFFT, gate.SupportsLWTT);
        }
        Console.WriteLine("\n\n\n\n\n");
    }

    //Assign boarding gate to flight
    void AssignGateToFlight()
    {
        try
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Assign a Boarding Gate to a Flight");
            Console.WriteLine("=============================================");

            Console.WriteLine("Enter Flight Number:");
            string flightNumber = Console.ReadLine();

            if (!terminal.Flights.ContainsKey(flightNumber))
            {
                Console.WriteLine("Flight not found!");
                return;
            }

            Flight flight = terminal.Flights[flightNumber];

            BoardingGate selectedGate = null;
            while (selectedGate == null)
            {
                Console.WriteLine("Enter Boarding Gate Name:");
                string gateName = Console.ReadLine();

                if (terminal.BoardingGates.ContainsKey(gateName))
                {
                    BoardingGate gate = terminal.BoardingGates[gateName];

                    if (gate.Flight != null)
                    {
                        Console.WriteLine($"Boarding Gate {gate.GateName} is already assigned to another flight. Please choose a different gate.");
                    }
                    else
                    {
                        selectedGate = gate;
                        Console.WriteLine($"Flight Number: {flight.FlightNumber}");
                        Console.WriteLine($"Origin: {flight.Origin}");
                        Console.WriteLine($"Destination: {flight.Destination}");
                        Console.WriteLine($"Expected Time: {flight.ExpectedTime.ToString("d/M/yyyy hh:mm:ss tt")}");
                        Console.WriteLine($"Special Request Code: {flight.Status}");
                        Console.WriteLine($"Boarding Gate Name: {gate.GateName}");
                        Console.WriteLine($"Supports DDJB: {gate.SupportsDDJB}");
                        Console.WriteLine($"Supports CFFT: {gate.SupportsCFFT}");
                        Console.WriteLine($"Supports LWTT: {gate.SupportsLWTT}");
                    }
                }
                else
                {
                    Console.WriteLine("Boarding Gate not found.");
                }
            }

            Console.WriteLine("Would you like to update the status of the flight? (Y/N)");
            string updateStatus = Console.ReadLine().ToUpper();

            if (updateStatus == "Y")
            {
                Console.WriteLine("1. Delayed");
                Console.WriteLine("2. Boarding");
                Console.WriteLine("3. On Time");
                Console.WriteLine("Please select the new status of the flight:");

                string statusChoice = Console.ReadLine();

                switch (statusChoice)
                {
                    case "1":
                        flight.Status = "Delayed";
                        break;
                    case "2":
                        flight.Status = "Boarding";
                        break;
                    case "3":
                        flight.Status = "On Time";
                        break;
                    default:
                        Console.WriteLine("Invalid choice, setting status to 'On Time'.");
                        flight.Status = "On Time";
                        break;
                }
            }
            else
            {
                flight.Status = "On Time";
            }

            selectedGate.Flight = flight;

            Console.WriteLine($"Flight {flight.FlightNumber} has been assigned to Boarding Gate {selectedGate.GateName}!");
            Console.WriteLine("\n\n\n\n\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while assigning gate: {ex.Message}");
        }
    }

    void CreateFlight()
    {
        try
        {
            string flightNumber = ""; 
            bool validFlightNumber = false;

            //Flight number must be a number
            while (!validFlightNumber)
            {
                Console.Write("Enter Flight Number: ");
                flightNumber = Console.ReadLine();

                if (int.TryParse(flightNumber, out _))
                {
                    validFlightNumber = true;
                }
                else
                {
                    Console.WriteLine("Invalid Flight Number. Please enter a valid number.");
                }
            }

            string origin = "";
            bool validOrigin = false;

            //Origin must not be a number
            while (!validOrigin)
            {
                Console.Write("Enter Origin: ");
                origin = Console.ReadLine();

                if (!origin.Any(char.IsDigit))
                {
                    validOrigin = true;
                }
                else
                {
                    Console.WriteLine("Origin cannot contain numbers. Please enter a valid origin.");
                }
            }

            string destination = ""; 
            bool validDestination = false;

            //Destination must not be a number
            while (!validDestination)
            {
                Console.Write("Enter Destination: ");
                destination = Console.ReadLine();

                if (!destination.Any(char.IsDigit))  
                {
                    validDestination = true;
                }
                else
                {
                    Console.WriteLine("Destination cannot contain numbers. Please enter a valid destination.");
                }
            }

            DateTime expectedDateTime = DateTime.MinValue;
            bool validDate = false;

            //Check date format
            while (!validDate)
            {
                try
                {
                    Console.Write("Enter Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
                    string expectedTime = Console.ReadLine();

                    if (!DateTime.TryParseExact(expectedTime, "d/M/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out expectedDateTime))
                    {
                        throw new FormatException("Invalid date format. Please enter the time in the correct format (dd/mm/yyyy hh:mm).");
                    }

                    validDate = true;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            string specialRequestCode = string.Empty;
            bool validRequestCode = false;

            //Check if request code exists
            while (!validRequestCode)
            {
                try
                {
                    Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
                    specialRequestCode = Console.ReadLine().ToUpper();

                    if (specialRequestCode != "CFFT" && specialRequestCode != "DDJB" && specialRequestCode != "LWTT" && specialRequestCode != "NONE")
                    {
                        throw new ArgumentException("Invalid Special Request Code. Please enter a valid code (CFFT, DDJB, LWTT, or None).");
                    }

                    validRequestCode = true;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Flight newFlight = new NORMFlight
            {
                FlightNumber = flightNumber,  
                Origin = origin,             
                Destination = destination,    
                ExpectedTime = expectedDateTime,
                Status = "Scheduled",
            };

            terminal.Flights.Add(flightNumber, newFlight);

            Console.WriteLine($"Flight {flightNumber} has been added!");

            Console.WriteLine("Would you like to add another flight? (Y/N): ");
            string anotherFlight = Console.ReadLine().ToUpper();

            if (anotherFlight == "Y")
            {
                CreateFlight();
            }
            else
            {
                Console.WriteLine("\n\n\n\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while creating the flight: {ex.Message}");
        }
    }

    void ReadFlights()
    {
        try
        {
            string[] lines = File.ReadAllLines("flights.csv");
            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                if (data.Length < 5)
                {
                    Console.WriteLine($"Skipping invalid line at {i + 1}: {lines[i]}");
                    continue;
                }

                string flightNumber = data[0];
                string origin = data[1];
                string destination = data[2];
                DateTime expectedTime;
                if (!DateTime.TryParse(data[3], out expectedTime))
                {
                    Console.WriteLine($"Invalid date format for flight {flightNumber}. Skipping...");
                    continue;
                }
                string specialRequestCode = data[4];

                Flight flight;
                switch (specialRequestCode.Trim())
                {
                    case "DDJB":
                        flight = new DDJBFlight { FlightNumber = flightNumber, Origin = origin, Destination = destination, ExpectedTime = expectedTime, Status = "Scheduled" };
                        break;
                    case "LWTT":
                        flight = new LWTTFlight { FlightNumber = flightNumber, Origin = origin, Destination = destination, ExpectedTime = expectedTime, Status = "Scheduled" };
                        break;
                    case "CFFT":
                        flight = new CFFTFlight { FlightNumber = flightNumber, Origin = origin, Destination = destination, ExpectedTime = expectedTime, Status = "Scheduled" };
                        break;
                    default:
                        flight = new NORMFlight { FlightNumber = flightNumber, Origin = origin, Destination = destination, ExpectedTime = expectedTime, Status = "Scheduled" };
                        break;
                }

                if (!terminal.Flights.ContainsKey(flightNumber))
                {
                    terminal.Flights[flightNumber] = flight;
                }
                else
                {
                    Console.WriteLine($"Duplicate flight number {flightNumber}. Skipping...");
                }
            }
            Console.WriteLine("Flights loaded successfully.");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: The file 'flights.csv' was not found. {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading flights: {ex.Message}");
        }
    }

    void PrintFlights()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("\nList of Flights for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");

        Console.WriteLine("{0,-15} {1,-20} {2,-25} {3,-25} {4,-30}",
            "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

        foreach (var flight in terminal.Flights.Values)
        {
            string airlineName = terminal.GetAirlineFromFlight(flight)?.Name ?? "Unknown Airline";
            Console.WriteLine("{0,-15} {1,-20} {2,-25} {3,-25} {4,-30}",
                flight.FlightNumber, airlineName, flight.Origin, flight.Destination, flight.ExpectedTime.ToString("d/M/yyyy hh:mm:ss tt"));
        }
        Console.WriteLine("\n\n\n\n\n");
    }

    void ModifyFlightDetails()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("{0,-15} {1,-20}", "Airline Code", "Airline Name");
        Console.WriteLine("{0,-15} {1,-20}", "SQ", "Singapore Airlines");
        Console.WriteLine("{0,-15} {1,-20}", "MH", "Malaysia Airlines");
        Console.WriteLine("{0,-15} {1,-20}", "JL", "Japan Airlines");
        Console.WriteLine("{0,-15} {1,-20}", "CX", "Cathay Pacific");
        Console.WriteLine("{0,-15} {1,-20}", "QF", "Qantas Airways");
        Console.WriteLine("{0,-15} {1,-20}", "TR", "AirAsia");
        Console.WriteLine("{0,-15} {1,-20}", "EK", "Emirates");
        Console.WriteLine("{0,-15} {1,-20}", "BA", "British Airways");
        Console.Write("Enter Airline Code: ");
        string airlineCode = Console.ReadLine().Trim().ToUpper();

        string airlineName = GetAirlineNameFromCode(airlineCode);
        if (airlineName == "Unknown Airline")
        {
            Console.WriteLine("Invalid airline code. Please try again.");
            return;
        }

        Console.WriteLine("=============================================");
        Console.WriteLine("List of Flights for {0}", airlineName);
        Console.WriteLine("=============================================");
        Console.WriteLine("{0,-15} {1,-25} {2,-25} {3,-25}", "Flight Number", "Origin", "Destination", "Expected Departure/Arrival Time");

        var flights = terminal.Flights.Values.Where(f => f.FlightNumber.StartsWith(airlineCode)).ToList();

        if (flights.Count == 0)
        {
            Console.WriteLine("No flights found for {0}.", airlineName);
            return;
        }

        foreach (var flight in flights)
        {
            Console.WriteLine("{0,-15} {1,-25} {2,-25} {3,-25}",
                flight.FlightNumber, flight.Origin, flight.Destination, flight.ExpectedTime.ToString("d/M/yyyy hh:mm:ss tt"));
        }

        Console.WriteLine("Choose an existing Flight to modify or delete: ");
        string flightNumber = Console.ReadLine();
        Flight selectedFlight = flights.FirstOrDefault(f => f.FlightNumber == flightNumber);

        if (selectedFlight == null)
        {
            Console.WriteLine("Flight not found!");
            return;
        }

        Console.WriteLine("1. Modify Flight");
        Console.WriteLine("2. Delete Flight");
        Console.WriteLine("Choose an option: ");
        string option = Console.ReadLine();

        if (option == "1")
        {
            Console.WriteLine("[1] Modify Basic Information");
            Console.WriteLine("[2] Modify Status");
            Console.WriteLine("[3] Modify Special Request Code");
            Console.WriteLine("[4] Modify Boarding Gate");
            Console.Write("Choose an option: ");
            string modifyOption = Console.ReadLine();

            switch (modifyOption)
            {
                case "1":
                    Console.Write("Enter new Origin: ");
                    selectedFlight.Origin = Console.ReadLine();
                    Console.Write("Enter new Destination: ");
                    selectedFlight.Destination = Console.ReadLine();
                    Console.Write("Enter new Expected Departure/Arrival Time (dd/MM/yyyy hh:mm tt): ");
                    selectedFlight.ExpectedTime = DateTime.Parse(Console.ReadLine());
                    break;
                case "2":
                    Console.Write("Enter new Status: ");
                    selectedFlight.Status = Console.ReadLine();
                    break;
                case "3":
                    Console.Write("Enter new Special Request Code: ");
                    selectedFlight.SpecialRequestCode = Console.ReadLine();
                    break;
                case "4":
                    Console.Write("Enter new Boarding Gate: ");
                    selectedFlight.BoardingGate = Console.ReadLine();
                    break;
                default:
                    Console.WriteLine("Invalid choice!");
                    return;
            }

            Console.WriteLine("Flight updated!");
            Console.WriteLine("Flight Number: {0}", selectedFlight.FlightNumber);
            Console.WriteLine("Airline Name: {0}", airlineName);
            Console.WriteLine("Origin: {0}", selectedFlight.Origin);
            Console.WriteLine("Destination: {0}", selectedFlight.Destination);
            Console.WriteLine("Expected Departure/Arrival Time: {0}", selectedFlight.ExpectedTime.ToString("d/M/yyyy hh:mm:ss tt"));
            Console.WriteLine("Status: {0}", selectedFlight.Status);
            Console.WriteLine("Special Request Code: {0}", selectedFlight.SpecialRequestCode ?? "Unassigned");
            Console.WriteLine("Boarding Gate: {0}", selectedFlight.BoardingGate ?? "Unassigned");
        }
        else if (option == "2")
        {
            Console.Write("Are you sure you want to delete this flight? [Y/N]: ");
            string confirm = Console.ReadLine().Trim().ToUpper();
            if (confirm == "Y")
            {
                terminal.Flights.Remove(selectedFlight.FlightNumber);
                Console.WriteLine("Flight deleted successfully.");
            }
            else
            {
                Console.WriteLine("Deletion canceled.");
            }
        }
        else
        {
            Console.WriteLine("Invalid choice!");
        }
    }

    string GetAirlineNameFromCode(string airlineCode)
    {
        switch (airlineCode)
        {
            case "SQ": return "Singapore Airlines";
            case "MH": return "Malaysia Airlines";
            case "JL": return "Japan Airlines";
            case "CX": return "Cathay Pacific";
            case "QF": return "Qantas Airways";
            case "TR": return "AirAsia";
            case "EK": return "Emirates";
            case "BA": return "British Airways";
            default: return "Unknown Airline";
        }
    }

    void DisplayFlightSchedule()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Flight Schedule for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");

        var sortedFlights = terminal.Flights.Values.OrderBy(f => f).ToList();

        if (sortedFlights.Count == 0)
        {
            Console.WriteLine("No flights scheduled for today.");
            return;
        }

        Console.WriteLine("{0,-15} {1,-18} {2,-20} {3,-20} {4,-34} {5,-16} {6,-20}",
            "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time", "Status", "Boarding Gate");

        foreach (var flight in sortedFlights)
        {
            string airlineName = GetAirlineNameFromCode(flight.FlightNumber.Substring(0, 2));

            Console.WriteLine("{0,-15} {1,-18} {2,-20} {3,-20} {4,-34} {5,-16} {6,-25}",
                flight.FlightNumber,
                airlineName,
                flight.Origin,
                flight.Destination,
                flight.ExpectedTime.ToString("d/M/yyyy hh:mm:ss tt"),
                flight.Status,
                flight.BoardingGate ?? "-");
        }

        Console.WriteLine("\n\n\n\n\n");
    }

    public void DisplayMenu()
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Welcome to Changi Airport Terminal 5");
        Console.WriteLine("=============================================");

        Console.WriteLine("1. List All Flights");
        Console.WriteLine("2. List Boarding Gates");
        Console.WriteLine("3. Assign a Boarding Gate to a Flight");
        Console.WriteLine("4. Create Flight");
        Console.WriteLine("5. Display Airline Flights");
        Console.WriteLine("6. Modify Flight Details");
        Console.WriteLine("7. Display Flight Schedule");
        Console.WriteLine("0. Exit\n\n");
        Console.Write("Please select your option:\n");
    }

    static void Main(string[] args)
    {
        Program program = new Program();
        try
        {
            Console.WriteLine("Loading Airlines...");
            program.ReadAirlines();
            Console.WriteLine($"{program.terminal.Airlines.Count} Airlines Loaded!");
            Console.WriteLine("Loading Boarding Gates...");
            program.ReadBoardingGates();
            Console.WriteLine($"{program.terminal.BoardingGates.Count} Boarding Gates Loaded!");
            Console.WriteLine("Loading Flights...");
            program.ReadFlights();
            Console.WriteLine($"{program.terminal.Flights.Count} Flights Loaded!\n\n\n\n");

            while (true)
            {
                program.DisplayMenu();
                try
                {
                    int choice;
                    if (int.TryParse(Console.ReadLine(), out choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                program.PrintFlights();
                                break;
                            case 2:
                                program.ListBoardingGates();
                                break;
                            case 3:
                                program.AssignGateToFlight();
                                break;
                            case 4:
                                program.CreateFlight();
                                break;
                            case 5:
                                program.DisplayAirlinesFlights();
                                break;
                            case 6:
                                program.ModifyFlightDetails();
                                break;
                            case 7:
                                program.DisplayFlightSchedule();
                                break;
                            case 0:
                                Console.WriteLine("Goodbye!");
                                return;
                            default:
                                Console.WriteLine("Invalid choice, try again.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please enter a valid option.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while processing your request: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during initialization: {ex.Message}");
        }
    }
}
