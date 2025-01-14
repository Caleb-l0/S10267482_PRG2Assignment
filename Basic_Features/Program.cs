class NORMFlight
{
    public double CalculateFees()
    {

    }
    public override string ToString()
    {
        return base.ToString();
    }
}

class LWTTFlight
{
    public double RequestFee { get; set; }

    public LWTTFlight(double RequestFee)
    {
        RequestFee = this.RequestFee;
    }



}
class DDJBFlight
{
    public double RequestFee { get; set; }
}

class CFFTFlight
{
    public double RequestFee { get; set; }
}

public abstract class Flight
{
    public string flightNumber {  get; set; }
    public string origin { get; set; }
    public string destination { get; set; }
    public DateTime expectedTime { get; set; }

    public string status { get; set; }

    public Flight() 
    { 

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
