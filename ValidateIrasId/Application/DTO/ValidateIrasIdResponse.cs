namespace ValidateIrasId.Application.DTO;

public class ValidateIrasIdResponse
{
    public string Status { get; set; } = null!;
    public DateTime TimeStamp { get; set; }
    public string? Error { get; set; }
    public HarpProjectRecordDataDTO? Data { get; set; }
}