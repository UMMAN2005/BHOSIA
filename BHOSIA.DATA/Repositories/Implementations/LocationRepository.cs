namespace BHOSIA.DATA.Repositories.Implementations;
public class LocationRepository(AppDbContext context) : Repository<Location>(context), ILocationRepository {
}
