namespace BHOSIA.BUSINESS.DTOs.General;

public record AppUserGetDto(
  string Id,
  string UserName,
  string FullName,
  string Email,
  string AvatarLink,
  List<string> Roles
);
