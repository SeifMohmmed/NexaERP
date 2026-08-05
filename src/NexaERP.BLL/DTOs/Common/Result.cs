namespace NexaERP.BLL.DTOs.Common;

public class Result
{
    public bool Succeeded { get; init; }

    public Dictionary<string, string>? Errors { get; init; }
}
