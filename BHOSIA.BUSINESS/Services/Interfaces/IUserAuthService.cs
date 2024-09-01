namespace BHOSIA.BUSINESS.Services.Interfaces;
public interface IUserAuthService {
  Task<BaseResponse> Login(UserLoginDto loginDto);
  Task<BaseResponse> ResendOtpAsync(UserResendOtpDto resendOtpDto);
}
