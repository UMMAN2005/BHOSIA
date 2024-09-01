namespace BHOSIA.BUSINESS.Helpers;

public class MapProfile : Profile {
  public MapProfile(IHttpContextAccessor accessor) {
    var context = accessor.HttpContext;

    var uriBuilder = new UriBuilder(context!.Request.Scheme, context.Request.Host.Host, context.Request.Host.Port ?? -1);
    if (uriBuilder.Uri.IsDefaultPort) uriBuilder.Port = -1;
    var baseUrl = uriBuilder.Uri.AbsoluteUri;

    CreateMap<Location, LocationUpsertDto>().ReverseMap();
    CreateMap<Location, LocationGetDto>().ReverseMap();
  }
}