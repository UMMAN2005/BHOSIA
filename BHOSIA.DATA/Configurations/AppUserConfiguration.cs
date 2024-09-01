namespace BHOSIA.DATA.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser> {
  public void Configure(EntityTypeBuilder<AppUser> builder) {
    builder.HasKey(x => x.Id);
    builder.Property(x => x.Id).ValueGeneratedOnAdd();
    builder.Property(x => x.Email).IsRequired().HasMaxLength(50);
    builder.Property(x => x.UserName).IsRequired().HasMaxLength(50);

    // New configurations for OTP and TOTP properties
    builder.Property(x => x.OtpCode)
      .HasMaxLength(6) // Assuming OTP is a 6-digit code
      .IsRequired(false); // Not required since it will be null until OTP is generated

    builder.Property(x => x.TotpSecret)
      .HasMaxLength(160) // 160 bits / 5 = 32 characters for Base32 encoding
      .IsRequired(false); // Not required since it may be null if TOTP is not used

    builder.Property(x => x.OtpExpirationTime)
      .IsRequired(false); // Optional, depending on if OTP has been generated

    builder.Property(x => x.LastOtpRequestTime)
      .IsRequired(false); // Optional, depending on if OTP has been requested

    // Optional: Configure the AuthProvider enum as a string or integer based on your requirements
    builder.Property(x => x.Provider)
      .HasConversion<string>() // Storing the enum as a string in the database
      .IsRequired();

    // Configure the one-to-one relationship with Location
    builder.HasOne(x => x.Location)
      .WithOne(x => x.AppUser)
      .HasForeignKey<Location>(x => x.AppUserId)
      .OnDelete(DeleteBehavior.Cascade); // Optionally configure delete behavior
  }
}
