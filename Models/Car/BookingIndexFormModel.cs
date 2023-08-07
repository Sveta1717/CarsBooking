using Microsoft.AspNetCore.Mvc;

namespace CarBooking.Models.Car
{
    public class BookingIndexFormModel
    {
        [FromForm(Name = "carName")]
        public String Title { get; set; }

        [FromForm(Name = "carDescription")]
        public String? Description { get; set; }

        [FromForm(Name = "brand")]
        public Guid BrandId { get; set; }

        [FromForm(Name = "price_per_day")]
        public float Price_per_day { get; set; }

        [FromForm(Name = "Image")]
        public IFormFile Image { get; set; }        
    }
}
