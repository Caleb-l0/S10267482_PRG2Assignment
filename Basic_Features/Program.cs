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
    public void CalculateTotalFee()
    {
        // for promotional condition (not done)//
        return CalculateFees();
    }

    //1//
    public void ReadAirlines()
    {
        try
        {
            string[] lines = File.ReadAllLines("airlines.csv");
            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                if (data.Length < 2)
                {
                    Console.WriteLine($"Invalid data at line {i + 1}. Skipping.");
                    continue;
                }

                string airlineName = data[0];
                string airlineCode = data[1];

                Airline airline = new Airline(airlineName, airlineCode);
                Airlines.Add(airlineCode, airline);
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("Error: airlines.csv file not found.");
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while reading airlines.");
            Console.WriteLine(ex.Message);
        }
    }

    public void ReadBoardingGates()
    {
        try
        {
            string[] lines = File.ReadAllLines("boardinggates.csv");
            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                if (data.Length < 4)
                {
                    Console.WriteLine($"Invalid data at line {i + 1}. Skipping.");
                    continue;
                }

                string gateName = data[0];
                bool supportsDDJB = bool.TryParse(data[1], out bool ddjb) ? ddjb : false;
                bool supportsCFFT = bool.TryParse(data[2], out bool cfft) ? cfft : false;
                bool supportsLWTT = bool.TryParse(data[3], out bool lwtt) ? lwtt : false;

                BoardingGate gate = new BoardingGate(gateName, supportsCFFT, supportsDDJB, supportsLWTT);
                BoardingGates.Add(gateName, gate);
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("Error: boardinggates.csv file not found.");
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while reading boarding gates.");
            Console.WriteLine(ex.Message);
        }
    }

    //2//
public void ReadFlights()
    {
        try
        {
            string[] lines = File.ReadAllLines("flights.csv");
            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                if (data.Length < 5)
                {
                    Console.WriteLine($"Invalid data at line {i + 1}. Skipping.");
                    continue;
                }

                string flightNumber = data[0];
                string origin = data[1];
                string destination = data[2];
                string expectedTime = data[3];
                string requestCode = data[4];

                if (!DateTime.TryParse(expectedTime, out DateTime expectedDateTime))
                {
                    Console.WriteLine($"Invalid time format for flight {flightNumber}. Skipping.");
                    continue;
                }

                Flight flight = new NORMFlight
                {
                    FlightNumber = flightNumber,
                    Origin = origin,
                    Destination = destination,
                    ExpectedTime = expectedDateTime,
                    Status = "Scheduled"
                };

                Flights.Add(flightNumber, flight);
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("Error: flights.csv file not found.");
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while reading flights.");
            Console.WriteLine(ex.Message);
        }
    }

    //3//
    void DisplayFlightInfo()
    {
        Console.WriteLine("Flight Number      Airline Name      Origin      Destination      Expected Departure/Arrival");
        foreach (var airline in Airlines.Values)
        {
            foreach (var flight in airline.Flights.Values)
            {
                Console.WriteLine($"{flight.FlightNumber}      {airline.Name}      {flight.Origin}      {flight.Destination}      {flight.ExpectedTime}");
            }
        }
    }



    //4//

    void ListBoardingGates()
    {
        Console.WriteLine("Boarding Gates    Special Request Codes      Flight Numbers   ");
        foreach (var gate in terminal.BoardingGates.Values)
        {
            Console.WriteLine(gate.ToString());
        }

    }

    //5//
    void GateToFlight()
    {
        Console.WriteLine("Enter your flight number:");
        string flightno = Console.ReadLine();
        if (!Flights.ContainsKey(flightno))
        {
            Console.WriteLine("Flight not found!");
            return;
        }

        Console.WriteLine("Enter your boarding gate:");
        string gateName = Console.ReadLine();
        if (BoardingGates.ContainsKey(gateName) && BoardingGates[gateName].Flight == null)
        {
            BoardingGates[gateName].Flight = Flights[flightno];
            Console.WriteLine($"Flight {flightno} assigned to gate {gateName}");
        }
        else
        {
            Console.WriteLine("Gate is already occupied or invalid.");
        }
    }


    //6//
   public void CreateNewFlight()
    {
        try
        {
            Console.WriteLine("Enter Flight Number:");
            string flightNumber = Console.ReadLine();

            Console.WriteLine("Enter Origin:");
            string origin = Console.ReadLine();

            Console.WriteLine("Enter Destination:");
            string destination = Console.ReadLine();

            Console.WriteLine("Enter Expected Departure/Arrival Time (YYYY/MM/DD HH:MM):");
            string expectedTime = Console.ReadLine();

            if (!DateTime.TryParse(expectedTime, out DateTime expectedDateTime))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            Console.WriteLine("Would you like to add a Special Request Code? [Y/N]");
            string requestCode = string.Empty;
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.WriteLine("Enter Special Request Code:");
                requestCode = Console.ReadLine();
            }

            Flight newFlight = new NORMFlight
            {
                FlightNumber = flightNumber,
                Origin = origin,
                Destination = destination,
                ExpectedTime = expectedDateTime,
                Status = "Scheduled"
            };

            Flights.Add(flightNumber, newFlight);
            Console.WriteLine($"Flight {flightNumber} created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while creating the flight.");
            Console.WriteLine(ex.Message);
        }
    }

    //7//
    void DisplayAirlineDetails()
    {
        
        Console.WriteLine("Available Airlines:");
        foreach (var airline in Airlines.Values) 
        {
            Console.WriteLine($"{airline.Code}: {airline.Name}");
        }

        
        Console.WriteLine("Enter the 2-Letter Airline Code (e.g., SQ, MH):");
        string airlineCode = Console.ReadLine().ToUpper(); 

        
        if (Airlines.ContainsKey(airlineCode))
        {
            Airline selectedAirline = Airlines[airlineCode];

            
            Console.WriteLine($"Flights for {selectedAirline.Name}:");
            foreach (var flight in selectedAirline.Flights.Values) 
            {
                Console.WriteLine($"{flight.FlightNumber} - {flight.Origin} to {flight.Destination}");
            }

            
            Console.WriteLine("Enter the Flight Number to view details:");
            string selectedFlightNumber = Console.ReadLine().ToUpper();

           
            if (selectedAirline.Flights.ContainsKey(selectedFlightNumber))
            {
                Flight selectedFlight = selectedAirline.Flights[selectedFlightNumber];
                Console.WriteLine($"Flight Details for {selectedFlight.FlightNumber}:");
                Console.WriteLine($"Airline: {selectedAirline.Name}");
                Console.WriteLine($"Origin: {selectedFlight.Origin}");
                Console.WriteLine($"Destination: {selectedFlight.Destination}");
                Console.WriteLine($"Expected Departure/Arrival: {selectedFlight.ExpectedTime}");
                Console.WriteLine($"Status: {selectedFlight.Status}");

                
                if (!string.IsNullOrEmpty(selectedFlight.RequestCode))
                {
                    Console.WriteLine($"Special Request Code: {selectedFlight.RequestCode}");
                }

                
                if (selectedFlight is BoardingGateFlight gateFlight && gateFlight.BoardingGate != null)
                {
                    Console.WriteLine($"Boarding Gate: {gateFlight.BoardingGate.GateName}");
                }
                else
                {
                    Console.WriteLine("No boarding gate assigned.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Flight Number entered.");
            }
        }
        else
        {
            Console.WriteLine("Invalid Airline Code entered.");
        }
    }




    //8//
    void ModifyFlightDetails()
{
    Console.WriteLine("Enter the Flight Number you want to modify:");
    string flightNumber = Console.ReadLine();

    Flight flightToModify = null;
    foreach (var airline in Airlines.Values)
    {
        if (airline.Flights.ContainsKey(flightNumber))
        {
            flightToModify = airline.Flights[flightNumber];
            break;
        }
    }

    if (flightToModify == null)
    {
        Console.WriteLine("Flight not found!");
        return;
    }

    Console.WriteLine("1) Modify an existing flight");
    Console.WriteLine("2) Delete an existing flight");
    string option = Console.ReadLine();

    if (option == "1")
    {
        Console.WriteLine("What would you like to modify?");
        Console.WriteLine("1. Origin");
        Console.WriteLine("2. Destination");
        Console.WriteLine("3. Expected Departure/Arrival Time");
        Console.WriteLine("4. Special Request Code");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.WriteLine("Enter new Origin:");
                flightToModify.Origin = Console.ReadLine();
                Console.WriteLine("Origin updated successfully.");
                break;

            case "2":
                Console.WriteLine("Enter new Destination:");
                flightToModify.Destination = Console.ReadLine();
                Console.WriteLine("Destination updated successfully.");
                break;

            case "3":
                Console.WriteLine("Enter new Expected Departure/Arrival Time (YYYY/MM/DD HH:MM):");
                string newTimeInput = Console.ReadLine();
                try
                {
                    DateTime newTime = DateTime.Parse(newTimeInput); // Using DateTime.Parse
                    flightToModify.ExpectedTime = newTime;
                    Console.WriteLine("Expected Departure/Arrival Time updated successfully.");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid date format.");
                }
                break;

            case "4":
                Console.WriteLine("Enter new Special Request Code:");
                flightToModify.RequestCode = Console.ReadLine();
                Console.WriteLine("Special Request Code updated successfully.");
                break;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
    else if (option == "2")
    {
        Console.WriteLine("Are you sure you want to delete this flight? [Y/N]");
        string confirmation = Console.ReadLine().ToUpper();
        if (confirmation == "Y")
        {
            foreach (var airline in Airlines.Values)
            {
                if (airline.Flights.ContainsKey(flightNumber))
                {
                    airline.Flights.Remove(flightNumber);
                    Console.WriteLine("Flight deleted successfully.");
                    break;
                }
            }
        }
        else
        {
            Console.WriteLine("Flight deletion canceled.");
        }
    }
    else
    {
        Console.WriteLine("Invalid option.");
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
