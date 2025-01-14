public class NORMFlight : Flight
{
    public override double CalculateFees()
    {
        return ; 
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
        return RequestFee + ; 
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
        return RequestFee + ; 
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
        return RequestFee + ;
    }

    public override string ToString()
    {
        return base.ToString() + $", Type: CFFTFlight, Request Fee: {RequestFee}";
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


 class Airline : Flight
{
    public string name { get; set; }
    public string code { get; set; }

    public Dictionary<string, Flight> flights = new Dictionary<string, Flight>();

    public Airline(string name, string code)
    {
        this.name = name;
        this.code = code;

    }


}
//test//

//testing//
