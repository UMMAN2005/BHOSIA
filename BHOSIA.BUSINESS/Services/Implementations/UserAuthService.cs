using BHOSIA.CORE.Enums;
using BHOSIA.DATA.Repositories.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BHOSIA.BUSINESS.Services.Implementations;

public class UserAuthService(AppUserManager userManager, IConfiguration configuration, IEmailService emailService)
  : IUserAuthService {
  public async Task<BaseResponse> Login(UserLoginDto loginDto) {
    var user = await userManager.FindByEmailAsync(loginDto.Email);

    if (user is null) {
      // Create user if not exists
      user = new AppUser {
        Email = loginDto.Email,
        UserName = loginDto.Email,
        Provider = AuthProvider.Local,
      };

      var result = await userManager.CreateAsync(user, loginDto.Password!);

      if (!result.Succeeded) {
        var errors = string.Join(", ", result.Errors.Select(x => x.Description));
        throw new RestException(StatusCodes.Status400BadRequest, errors);
      }

      await userManager.AddToRoleAsync(user, "Member");

      // Generate OTP and send to user email
      var otp = await userManager.GenerateOtpAsync(user);
      emailService.Send(user.Email!, "Email Verification", EmailTemplates.GetVerifyEmailTemplate(otp));

      return new BaseResponse(200, "User created successfully! Please check your email for the OTP.", null, []);
    }

    if (user.Provider is AuthProvider.Local) {
      if (!await userManager.CheckPasswordAsync(user, loginDto.Password!))
        throw new RestException(StatusCodes.Status401Unauthorized, "Username or Password incorrect!");

      if (!await userManager.IsEmailConfirmedAsync(user)) {
        if (loginDto.Otp == null) {
          // Email is not confirmed and OTP code is not provided
          throw new RestException(StatusCodes.Status401Unauthorized, $"Email not verified! {user.Email}. Please provide the OTP sent to your email.");
        }

        // Validate OTP
        if (!await userManager.ValidateOtpAsync(user, loginDto.Otp))
          throw new RestException(StatusCodes.Status401Unauthorized, "Invalid or expired OTP!");

        // Mark email as confirmed after successful OTP validation
        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);
      }
    }
    else {
      user.EmailConfirmed = true;
      await userManager.UpdateAsync(user);
    }

    var claims = new List<Claim> {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Email, user.Email!)
    };

    var roles = await userManager.GetRolesAsync(user);
    claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList());

    var secret = configuration.GetSection("JWT:Secret").Value!;
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
    var expires = loginDto.RememberMe ? DateTime.Now.AddDays(7) : DateTime.Now.AddDays(1);

    JwtSecurityToken securityToken = new(
        issuer: configuration.GetSection("JWT:Issuer").Value,
        audience: configuration.GetSection("JWT:Audience").Value,
        claims: claims,
        signingCredentials: credentials,
        expires: expires
    );

    var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

    return new BaseResponse(200, "Login successful!", token, []);
  }

  public async Task<BaseResponse> ResendOtpAsync(UserResendOtpDto resendOtpDto) {
    var user = await userManager.FindByEmailAsync(resendOtpDto.Email)
      ?? throw new RestException(StatusCodes.Status404NotFound, "User not found!");

    if (user.LastOtpRequestTime.HasValue && user.LastOtpRequestTime.Value.AddMinutes(2) > DateTime.UtcNow) {
      var nextRequestTime = user.LastOtpRequestTime.Value.AddMinutes(2);
      var remainingTime = nextRequestTime.Subtract(DateTime.UtcNow).TotalSeconds;
      return new BaseResponse(429, $"You must wait {remainingTime:F1} seconds before requesting a new OTP.", null, []);
    }

    var otp = await userManager.GenerateOtpAsync(user);
    user.LastOtpRequestTime = DateTime.UtcNow;
    await userManager.UpdateAsync(user);
    emailService.Send(user.Email!, "Email Verification", EmailTemplates.GetVerifyEmailTemplate(otp));

    return new BaseResponse(200, "OTP has been resent to your email.", null, []);
  }
}
