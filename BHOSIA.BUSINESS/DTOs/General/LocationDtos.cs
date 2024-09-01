namespace BHOSIA.BUSINESS.DTOs.General;

public record LocationGetDto(
  double Latitude,
  double Longitude
);

public record LocationUpsertDto(
  double Latitude,
  double Longitude
);
