public class TravelTicket
{
    public TravelTicket(StationName p_startStation, StationName p_endStation, int p_points, TicketType p_ticketType = TicketType.NORMAL)
    {
        ticketType = p_ticketType;
        startStation = p_startStation;
        endStation = p_endStation;
        points = p_points;
    }

    public override string ToString()
    {
        return $"{startStation} to {endStation}, {points} pts";
    }

    public TicketType ticketType;
    public StationName startStation;
    public StationName endStation;
    public int points;
}
