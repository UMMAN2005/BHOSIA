namespace BHOSIA.BUSINESS.DTOs.User;

public record UserLoginDto(
  string Email, string? Password, bool RememberMe, string? Otp = null
);

public record UserResendOtpDto(
  string Email
);

