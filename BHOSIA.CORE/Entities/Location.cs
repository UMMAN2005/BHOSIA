namespace BHOSIA.CORE.Entities;
public class Location {
  public double Latitude { get; set; }
  public double Longitude { get; set; }
  public AppUser AppUser { get; set; } = default!;
  public string AppUserId { get; set; } = default!;
}
