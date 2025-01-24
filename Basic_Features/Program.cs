using System;
using System.Collections.Generic;
using System.IO;

public class Terminal
{
    public string TerminalName { get; set; }
    public Dictionary<string, Airline> Airlines { get; set; } = new Dictionary<string, Airline>();
    public Dictionary<string, Flight> Flights { get; set; } = new Dictionary<string, Flight>();
    public Dictionary<string, BoardingGate> BoardingGates { get; set; } = new Dictionary<string, BoardingGate>();
    public Dictionary<string, double> GateFees { get; set; } = new Dictionary<string, double>();

    public bool AddAirline(Airline airline)
    {
        if (!Airlines.ContainsKey(airline.Code))
        {
            Airlines[airline.Code] = airline;
            return true;
        }
        return false;
    }

    public bool AddBoardingGate(BoardingGate gate)
    {
        if (!BoardingGates.ContainsKey(gate.GateName))
        {
            BoardingGates[gate.GateName] = gate;
            return true;
        }
        return false;
    }

    public Airline GetAirlineFromFlight(Flight flight)
    {
        foreach (var airline in Airlines.Values)
        {
            if (airline.Flights.ContainsKey(flight.FlightNumber))
                return airline;
        }
        return null;
    }

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

    public bool AddFlight(Flight flight)
    {
        if (!Flights.ContainsKey(flight.FlightNumber))
        {
            Flights[flight.FlightNumber] = flight;
            return true;
        }
        return false;
    }

    public double CalculateFees()
    {
        double totalFees = 0;
        foreach (var flight in Flights.Values)
        {
            totalFees += flight.CalculateFees();
        }
        return totalFees;
    }

    public bool RemoveFlight(Flight flight)
    {
        return Flights.Remove(flight.FlightNumber);
    }

    public override string ToString()
    {
        return $"Airline: {Name}, Code: {Code}, Flights: {Flights.Count}";
    }
}

public abstract class Flight
{
    protected double baseFee = 100.0; // Default base fee

    public string FlightNumber { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DateTime ExpectedTime { get; set; }
    public string Status { get; set; }

    public abstract double CalculateFees();

    public override string ToString()
    {
        return $"Flight: {FlightNumber}, Origin: {Origin}, Destination: {Destination}, Status: {Status}";
    }
}

public class NORMFlight : Flight
{
    public override double CalculateFees()
    {
        return (Destination == "Singapore (SIN)" ? 500 : 800) + baseFee; //SG Destination = 500, else 800
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
                    Console.WriteLine($"Skipping invalid line: {lines[i]}");
                    continue;
                }

                string airlineName = data[0];
                string airlineCode = data[1];

                Airline airline = new Airline(airlineName, airlineCode);
                if (!terminal.AddAirline(airline))
                {
                    Console.WriteLine($"Airline {airlineName} already exists. Skipping...");
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("Error: Airlines file not found. " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while reading airlines. " + ex.Message);
        }
    }    

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
                    Console.WriteLine($"Skipping invalid line: {lines[i]}");
                    continue;
                }

                string gateName = data[0];
                bool supportsDDJB = data[1].Equals("True", StringComparison.OrdinalIgnoreCase);
                bool supportsCFFT = data[2].Equals("True", StringComparison.OrdinalIgnoreCase);
                bool supportsLWTT = data[3].Equals("True", StringComparison.OrdinalIgnoreCase);

                BoardingGate gate = new BoardingGate(gateName, supportsCFFT, supportsDDJB, supportsLWTT);
                if (!terminal.AddBoardingGate(gate))
                {
                    Console.WriteLine($"Boarding gate {gateName} already exists. Skipping...");
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("Error: Boarding gates file not found. " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while reading boarding gates. " + ex.Message);
        }
    }



    void DisplayAirlinesFlights()
    {
        Console.WriteLine("Airlines and Their Flights:");
        foreach (var airline in terminal.Airlines.Values)
        {
            Console.WriteLine($"Airline: {airline.Name}");
            foreach (var flight in airline.Flights.Values)
            {
                Console.WriteLine($" - Flight Number: {flight.FlightNumber}, Origin: {flight.Origin}, Destination: {flight.Destination}, Status: {flight.Status}");
            }
        }
    }

    void ListBoardingGates()
    {
        Console.WriteLine("Boarding Gates and their Info:");
        foreach (var gate in terminal.BoardingGates.Values)
        {
            Console.WriteLine(gate.ToString());
        }
    }
void AssignGateToFlight()
    {
        try
        {
            Console.WriteLine("Enter Flight Number:");
            string flightNumber = Console.ReadLine();

            if (!terminal.Flights.ContainsKey(flightNumber))
            {
                Console.WriteLine("Flight not found!");
                return;
            }

            Console.WriteLine("Enter Boarding Gate:");
            string gateName = Console.ReadLine();

            if (terminal.BoardingGates.ContainsKey(gateName))
            {
                terminal.BoardingGates[gateName].Flight = terminal.Flights[flightNumber];
                Console.WriteLine($"Flight {flightNumber} assigned to Gate {gateName}");
            }
            else
            {
                Console.WriteLine("Boarding Gate not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while assigning a gate to the flight. " + ex.Message);
        }
    }
    
   
    void CreateFlight()
    {
        try
        {
            Console.WriteLine("Enter Flight Number:");
            string flightNumber = Console.ReadLine();

            if (terminal.Flights.ContainsKey(flightNumber))
            {
                Console.WriteLine("Flight already exists!");
                return;
            }

            Console.WriteLine("Enter Origin:");
            string origin = Console.ReadLine();

            Console.WriteLine("Enter Destination:");
            string destination = Console.ReadLine();

            Console.WriteLine("Enter Expected Departure/Arrival Time (YYYY/MM/DD HH:MM):");
            string expectedTime = Console.ReadLine();

            if (!DateTime.TryParse(expectedTime, out DateTime expectedDateTime))
            {
                Console.WriteLine("Invalid date format. Please enter the time in the correct format.");
                return;
            }

            Flight newFlight = new NORMFlight
            {
                FlightNumber = flightNumber,
                Origin = origin,
                Destination = destination,
                ExpectedTime = expectedDateTime,
                Status = "Scheduled"
            };

            terminal.Flights[flightNumber] = newFlight;

            Console.WriteLine("Enter Airline Code to associate with this flight:");
            string airlineCode = Console.ReadLine().ToUpper();

            if (terminal.Airlines.ContainsKey(airlineCode))
            {
                terminal.Airlines[airlineCode].AddFlight(newFlight);
                Console.WriteLine($"Flight {flightNumber} added to {terminal.Airlines[airlineCode].Name}.");
            }
            else
            {
                Console.WriteLine("Airline not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while creating the flight. " + ex.Message);
        }
    }
    
    void ModifyFlightDetails()
    {
        Console.WriteLine("Enter Flight Number to modify:");
        string flightNumber = Console.ReadLine();

        if (!terminal.Flights.ContainsKey(flightNumber))
        {
            Console.WriteLine("Flight not found!");
            return;
        }

        Flight flight = terminal.Flights[flightNumber];

        Console.WriteLine("Enter new Status for the Flight:");
        string newStatus = Console.ReadLine();
        flight.Status = newStatus;

        Console.WriteLine("Flight details updated successfully.");
    }

    void DisplayFlightSchedule()
    {
        Console.WriteLine("Flight Schedule:");
        foreach (var flight in terminal.Flights.Values)
        {
            Console.WriteLine($"{flight.FlightNumber}: {flight.Origin} to {flight.Destination}, Scheduled: {flight.ExpectedTime}");
        }
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
        Console.WriteLine("0. Exit");
        Console.Write("Choose an option: ");
    }

   public void Run()
    {
        try
        {
            ReadAirlines();
            ReadBoardingGates();

            while (true)
            {
                DisplayMenu();
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    try
                    {
                        switch (choice)
                        {
                            case 1:
                                DisplayAirlinesFlights();
                                break;
                            case 2:
                                ListBoardingGates();
                                break;
                            case 3:
                                AssignGateToFlight();
                                break;
                            case 4:
                                CreateFlight();
                                break;
                            case 5:
                                DisplayAirlinesFlights();
                                break;
                            case 6:
                                ModifyFlightDetails();
                                break;
                            case 7:
                                DisplayFlightSchedule();
                                break;
                            case 0:
                                Console.WriteLine("Exiting...");
                                return;
                            default:
                                Console.WriteLine("Invalid choice, try again.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while processing your request. " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input, please enter a valid option.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An unexpected error occurred. " + ex.Message);
        }
    }

    static void Main(string[] args)
    {
        Program program = new Program();
        program.Run();
    }
}

    //9//
    void FlightsInOrder()
{
    
    List<Flight> allFlights = new List<Flight>();
    foreach (var airline in Airlines.Values)
    {
        foreach (var flight in airline.Flights.Values)
        {
            allFlights.Add(flight);
        }
    }

    
    allFlights.Sort((f1, f2) => f1.ExpectedTime.CompareTo(f2.ExpectedTime));

    
    Console.WriteLine("Flights sorted by Expected Departure/Arrival Time:");
    foreach (var flight in allFlights)
    {
        Console.WriteLine($"{flight.FlightNumber} - {flight.Origin} to {flight.Destination} - {flight.ExpectedTime}");
    }
}





  public void DisplayMenu()
    {
        try
        {
            while (true)
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
                Console.WriteLine("0. Exit");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Implement listing flights
                        break;
                    case "2":
                        // Implement listing gates
                        break;
                    case "3":
                        // Implement gate assignment
                        break;
                    case "4":
                        CreateNewFlight();
                        break;
                    case "5":
                        // Implement airline flight details
                        break;
                    case "6":
                        // Implement modify flight details
                        break;
                    case "7":
                        // Implement display schedule
                        break;
                    case "0":
                        Console.WriteLine("Exiting program...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred in the menu.");
            Console.WriteLine(ex.Message);
        }
    }

    static void Main(string[] args)
    {
        Program program = new Program();
        program.DisplayMenu();
    }
}
