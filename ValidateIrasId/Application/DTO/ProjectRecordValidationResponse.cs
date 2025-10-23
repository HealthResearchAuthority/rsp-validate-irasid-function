namespace ValidateIrasId.Application.DTO;

public class ProjectRecordValidationResponse
{
    public DateTime TimeStamp { get; set; }
    public string? Error { get; set; }
    public HarpProjectRecordDataDTO? Data { get; set; }
}