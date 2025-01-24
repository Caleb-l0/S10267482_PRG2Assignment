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


    public void DisplayMenu()
    {
        try
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

            // Handle user input here, with proper exception handling:
            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    // List flights (implement this)
                    break;
                case 2:
                    // List boarding gates (implement this)
                    break;
                case 3:
                    // Assign boarding gate to flight (implement this)
                    break;
                case 4:
                    // Create flight (implement this)
                    break;
                case 5:
                    // Display airline flights (implement this)
                    break;
                case 6:
                    // Modify flight details (implement this)
                    break;
                case 7:
                    // Display flight schedule (implement this)
                    break;
                case 0:
                    Console.WriteLine("Exiting...");
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }
        catch (FormatException formatEx)
        {
            Console.WriteLine($"Input error: {formatEx.Message}. Please enter a valid number.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }


    static void Main(string[] args)
    {
        try
        {
            Program program = new Program();
            program.DisplayMenu();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
