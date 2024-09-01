namespace BHOSIA.BUSINESS.Services.Interfaces;
public interface ILocationService {
  public Task<BaseResponse> Upsert(LocationUpsertDto upsertDto);
  public Task<BaseResponse> GetPaginated(int pageNumber = 1, int pageSize = 1);
  public Task<BaseResponse> GetAll();
  public Task<BaseResponse> GetById();
  public Task<BaseResponse> Delete();
}
