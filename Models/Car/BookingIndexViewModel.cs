namespace CarBooking.Models.Car
{
    public class BookingIndexViewModel
    { 
        public List<Data.Entity.Brand> Brands { get; set; }
        public List<Data.Entity.Car> Cars { get; set; }
        public String? AddMessage { get; set; }
    }
}
