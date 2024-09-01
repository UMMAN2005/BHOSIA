//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Google;
//using Microsoft.AspNetCore.Identity;
//using System.Security.Claims;
//using BHOSIA.CORE.Entities;

namespace BHOSIA.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserAuthService userAuthService) : ControllerBase {

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromForm] UserLoginDto loginDto) {
    var response = await userAuthService.Login(loginDto);
    return StatusCode(response.StatusCode, response);
  }

  [HttpPost("resend-otp")]
  public async Task<IActionResult> ResendOtp([FromForm] UserResendOtpDto resendOtpDto) {
    var response = await userAuthService.ResendOtpAsync(resendOtpDto);
    return StatusCode(response.StatusCode, response);
  }

  //[HttpGet("login-google")]
  //public IActionResult Login() {
  //  var props = new AuthenticationProperties { RedirectUri = "api/auth/signin-google" };
  //  return Challenge(props, GoogleDefaults.AuthenticationScheme);
  //}

  //[HttpGet("signin-google")]
  //public async Task<IActionResult> GoogleLogin() {
  //  var response = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
  //  if (response.Principal == null) return BadRequest();

  //  var email = response.Principal.FindFirstValue(ClaimTypes.Email);
  //  var fullName = response.Principal.FindFirstValue(ClaimTypes.Name);
  //  var userName = response.Principal.FindFirstValue(ClaimTypes.Email);
  //  var profilePicture = response.Principal.FindFirstValue("picture");
  //  var location = await GetUserLocationAsync() ?? "N/A";


  //  if (email == null || fullName == null || userName == null) return BadRequest();

  //  BaseResponse result = new(400, "Something went wrong!", null, null);

  //  if (userManager.Users.Any(x => x.Email == email)) {
  //    var user = new UserLoginDto(email, "", false, true);
  //    result = await userAuthService.Login(user);
  //  }
  //  else {
  //    var guidPassword = new Guid();
  //    var password = guidPassword.ToString()[..8];
  //    var registerDto = new UserRegisterDto(email, fullName, userName, password, password, null, location);
  //    var registerResult = await userAuthService.Register(registerDto);

  //    if (registerResult.StatusCode == 201) {
  //      var user = new UserLoginDto(email, password, false, true);
  //      result = await userAuthService.Login(user);
  //    }
  //  }

  //  var userEntity = await userManager.FindByEmailAsync(email);
  //  userEntity!.AvatarLink = profilePicture;
  //  await userManager.UpdateAsync(userEntity);

  //  var redirectUrl = $"{configuration.GetSection("Client:URL").Value}Account/ExternalLoginCallback?token={result.Data}";
  //  return Redirect(redirectUrl);
  //}
}