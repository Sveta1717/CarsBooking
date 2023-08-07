namespace CarBooking.Data.Entity
{
    public class Car
    {
        public Guid      Id            { get; set; }
        public Guid      Brand_id      { get; set; }
        public String    Title         { get; set; } = null!;
        public String?   Description   { get; set; }
        public DateTime? DeleteDt      { get; set; }
        public DateTime? CreateDt      { get; set; }
        public String    ImageUri      { get; set; } = null!;        
        public float     Price_per_day { get; set; }

        public List<Rating> Rates      { get; set; } = null!;
    }
}
