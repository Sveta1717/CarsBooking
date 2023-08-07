namespace CarBooking.Data.Entity
{
    public class Rating
    {       
        public Guid     User_id { get; set; }
        public Guid     Car_id  { get; set; }
        public int      Rate    { get; set; }
        public DateTime Moment  { get; set; }

        public Car      Car     { get; set; } = null!;
    }
}
