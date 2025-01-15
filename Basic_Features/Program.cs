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


    public BoardingGate(Flight flight)
    {
        GateName = this.GateName;
        SupportsCFFT = this.SupportsCFFT;
        SupportsDDJB = this.SupportsDDJB;
        SupportsLWTT = this.SupportsLWTT;
        Flight = flight;

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

    public Airline(string Name, string Code)
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
        return (Destination == "Singapore (SIN)" ? 500 : 800) + baseFee; //SG Destination = 500, else 800//
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
    void ReadAirlines()
    {
        string[] lines = File.ReadAllLines("airlines.csv");
        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Split(',');
            string AirlineName = data[0];
            string AirlineCode = data[1];

            Airlines().Add(new Airline { AirlineName, AirlineCode });
        }
    }

    void ReadBoardingGates()
    {
        string[] lines = File.ReadAllLines("boardinggates.csv");
        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Split(",");
            string BoardingGate = data[0];
            string DDJB = data[1];
            string CFFT = data[2];
            string LWTT = data[3];

            BoardingGates().Add(new BoardingGate { BoardingGate, DDJB, CFFT, LWTT });
        }
    }

    //2//
    void ReadFlights()
    {
        string[] lines = File.ReadAllLines("flights.csv");
        for (int i = 1; i < lines.Length;, i++)
        {
            string[] data = lines[i].Split(",");
            string FlightNumber = data[0];
            string Origin = data[1];
            string Destination = data[2];
            string ExpectedDPAR = data[3];
            string RequestCode = data[4];

            Flights().Add(new Flight { FlightNumber, Origin, Destination, ExpectedDPAR, RequestCode })
        }
    }



    //3//
    void DisplayFlightInfo()
    {
        Console.WriteLine("Flight Number      Origin      Destination      Expected Depature/Arrival    Request Code") // Add airline name and remove Request code//
        foreach (var flights in Flights)
        {
            Console.Write(flights.ToString());
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
           



    }

    //6//
    void CreateNewFlight(string flightno)
    {
        while true
        {
            
            Console.WriteLine("Enter Flight Number:");
            string flightNumber = Console.ReadLine();

            Console.WriteLine("Enter Origin:");
            string origin = Console.ReadLine();

            Console.WriteLine("Enter Destination:");
            string destination = Console.ReadLine();

            Console.WriteLine("Enter Expected Departure/Arrival Time (YYYY/MM/DD HH:MM):");
            string expectedTime = Console.ReadLine();

            
            Console.WriteLine("Would you like to add a Special Request Code? [Y/N]");
            string requestCode = string.Empty;

            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.WriteLine("Enter Special Request Code:");
                requestCode = Console.ReadLine();
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


    }
    static void Main(string args[])
    {


    }


}
