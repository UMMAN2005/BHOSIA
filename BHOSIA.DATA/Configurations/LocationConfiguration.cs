namespace BHOSIA.DATA.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location> {
  public void Configure(EntityTypeBuilder<Location> builder) {
    builder.HasKey(x => x.AppUserId); // Use AppUserId as the primary key
    builder.Property(x => x.Latitude).IsRequired();
    builder.Property(x => x.Longitude).IsRequired();

    // Configure the relationship back to AppUser
    builder.HasOne(x => x.AppUser)
      .WithOne(x => x.Location)
      .HasForeignKey<Location>(x => x.AppUserId)
      .OnDelete(DeleteBehavior.Cascade); // Optionally configure delete behavior
  }
}
  