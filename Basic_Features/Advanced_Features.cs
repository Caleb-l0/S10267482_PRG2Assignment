class program
{
  void UnassignedFlights //s102674822//
  {
    Queue<Flight> unassignedFlights = new Queue<Flight>();
    foreach (var flight in Terminal.Flights.Values)
    {
      if (flight != Flights.ContainsKey)
      {
        unassignedFlights.Enqueue(flight);
        
      }
    }

     Console.WriteLine($"Total number of Flights without Boarding Gate assigned: {unassignedFlights.Count}");

    List<BoardingGate> unassignedGates = new List<BoardingGate>();
    foreach (var gate in terminal.BoardingGates.Values)
    {
    
        if (gate.Flight == null)
        {
            unassignedGates.Add(gate);
        }

    }
    Console.WriteLine($"Total number of Boarding Gates without a Flight Number assigned: {unassignedGates.Count}");
    
    
    
  }


}
