namespace BHOSIA.BUSINESS.Validators;

public class UserLoginValidator : AbstractValidator<UserLoginDto> {
  public UserLoginValidator() {
    RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(50)
      .Must(IsValidEmail).WithMessage("Only BHOS email addresses are allowed");
    RuleFor(x => x.Password).NotEmpty();
    RuleFor(x => x.Otp).MaximumLength(6).When(x => x.Otp != null);
  }

  private static bool IsValidEmail(string email) {
    var domain = email.Split('@')[1];
    return domain.Equals("bhos.edu.az", StringComparison.OrdinalIgnoreCase);
  }
}

public class UserResendOtpValidator : AbstractValidator<UserResendOtpDto> {
  public UserResendOtpValidator() {
    RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(50)
      .Must(IsValidEmail).WithMessage("Only BHOS email addresses are allowed");
  }

  private static bool IsValidEmail(string email) {
    var domain = email.Split('@')[1];
    return domain.Equals("bhos.edu.az", StringComparison.OrdinalIgnoreCase);
  }
}