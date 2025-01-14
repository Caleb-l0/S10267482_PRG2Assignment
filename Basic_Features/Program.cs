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
class Airline
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