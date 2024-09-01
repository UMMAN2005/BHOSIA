using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BHOSIA.BUSINESS.Services.Implementations;

public class LocationService(ILocationRepository locationRepository, IHttpContextAccessor accessor, IMapper mapper) : ILocationService {
  public async Task<BaseResponse> Upsert(LocationUpsertDto upsertDto) {
    var location = mapper.Map<Location>(upsertDto);

    var token = accessor.HttpContext!.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last()
                ?? throw new RestException(StatusCodes.Status401Unauthorized, "Unauthorized");

    location.AppUserId = JwtHelper.GetClaimFromJwt(token, ClaimTypes.NameIdentifier)!;

    var existingLocation = await locationRepository.GetAsync(x => x.AppUserId == location.AppUserId);

    if (existingLocation != null) {
      existingLocation.Longitude = location.Longitude;
      existingLocation.Latitude = location.Latitude;
    }
    else {
      await locationRepository.AddAsync(location);
    }

    await locationRepository.SaveAsync();

    var responseDto = mapper.Map<LocationGetDto>(existingLocation ?? location);
    var message = existingLocation != null ? "Updated successfully" : "Created successfully";

    return new BaseResponse(200, message, responseDto, []);
  }


  public async Task<BaseResponse> Delete() {
    var token = accessor.HttpContext!.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last()
                ?? throw new RestException(StatusCodes.Status401Unauthorized, "Unauthorized");

    var appUserId = JwtHelper.GetClaimFromJwt(token, ClaimTypes.NameIdentifier)!;

    var location = await locationRepository.GetAsync(x => x.AppUserId == appUserId)
                  ?? throw new RestException(StatusCodes.Status404NotFound, "Location not found");

    await locationRepository.DeleteAsync(location);

    await locationRepository.SaveAsync();

    return new BaseResponse(204, "Deleted successfully", null, []);
  }

  public async Task<BaseResponse> GetPaginated(int pageNumber = 1, int pageSize = 1) {
    if (pageNumber <= 0 || pageSize <= 0) {
      throw new RestException(StatusCodes.Status400BadRequest, "Invalid parameters for paging");
    }

    var locations = await locationRepository.GetPaginatedAsync(x => true, pageNumber, pageSize);
    var paginated = PaginatedList<Location>.Create(locations, pageNumber, pageSize);
    var data = new PaginatedList<LocationGetDto>(mapper.Map<List<LocationGetDto>>(paginated.Items), paginated.TotalPages, pageNumber, pageSize);

    return new BaseResponse(200, "Success", data, []);
  }

  public async Task<BaseResponse> GetById() {
    var token = accessor.HttpContext!.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last()
                ?? throw new RestException(StatusCodes.Status401Unauthorized, "Unauthorized");

    var appUserId = JwtHelper.GetClaimFromJwt(token, ClaimTypes.NameIdentifier)!;

    var location = await locationRepository.GetAsync(x => x.AppUserId == appUserId);

    return location == null
      ? throw new RestException(StatusCodes.Status404NotFound, "Location not found")
      : new BaseResponse(200, "Success", mapper.Map<LocationGetDto>(location), []);
  }

  public async Task<BaseResponse> GetAll() {
    var locations = await locationRepository.GetAllAsync(x => true);

    return new BaseResponse(200, "Success", mapper.Map<List<LocationGetDto>>(locations), []);
  }
}