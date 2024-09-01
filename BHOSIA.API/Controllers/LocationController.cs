namespace BHOSIA.API.Controllers;
[Route("api/user/[controller]")]
[ApiController]
public class LocationController(ILocationService locationService) : ControllerBase {
  [HttpPost]
  public async Task<IActionResult> Upsert(LocationUpsertDto upsertDto) {
    var response = await locationService.Upsert(upsertDto);
    return StatusCode(response.StatusCode, response);
  }

  [HttpGet("page")]
  public async Task<IActionResult> GetPaginated(int pageNumber = 1, int pageSize = 1) {
    var response = await locationService.GetPaginated(pageNumber, pageSize);
    return StatusCode(response.StatusCode, response);
  }

  [HttpGet]
  public async Task<IActionResult> GetById() {
    var response = await locationService.GetById();
    return StatusCode(response.StatusCode, response);
  }

  [HttpGet("all")]
  public async Task<IActionResult> GetAll() {
    var response = await locationService.GetAll();
    return StatusCode(response.StatusCode, response);
  }

  [HttpDelete]
  public async Task<IActionResult> Delete() {
    var response = await locationService.Delete();
    return StatusCode(response.StatusCode, response);
  }
}
