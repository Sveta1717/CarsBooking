using Microsoft.EntityFrameworkCore;

namespace CarBooking.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entity.User>   Users   { get; set; }
        public DbSet<Entity.Car>    Cars    { get; set; }
        public DbSet<Entity.Brand>  Brands  { get; set; }
        public DbSet<Entity.Rating> Ratings { get; set; }
      

        public DataContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("CarBooking");
            modelBuilder.Entity<Entity.Rating>()
                        .HasKey(nameof(Entity.Rating.Car_id),
                                nameof(Entity.Rating.User_id));
        }
    }
}
