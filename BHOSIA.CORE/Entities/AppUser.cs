using Microsoft.AspNetCore.Identity;

namespace BHOSIA.CORE.Entities;

public class AppUser : IdentityUser<string> {
  public Location? Location { get; set; }
  public AuthProvider Provider { get; set; }
  public string OtpCode { get; set; } = default!;
  public string TotpSecret { get; set; } = default!;
  public DateTime? OtpExpirationTime { get; set; }
  public DateTime? LastOtpRequestTime { get; set; }
}