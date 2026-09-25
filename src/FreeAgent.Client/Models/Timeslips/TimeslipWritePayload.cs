using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Timeslips;

/// <summary>
/// Writable timeslip attributes for create and update requests.
/// </summary>
internal sealed class TimeslipWritePayload
{
    [JsonPropertyName("task")]
    public ProjectTaskReference? ProjectTask { get; set; }

    [JsonPropertyName("user")]
    public UserReference? User { get; set; }

    [JsonPropertyName("project")]
    public ProjectReference? Project { get; set; }

    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    [JsonPropertyName("hours")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Hours { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    public static TimeslipWritePayload FromTimeslip(Timeslip timeslip, FreeAgentEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(timeslip);

        return new TimeslipWritePayload
        {
            ProjectTask = LinkedResourceWriteMapper.ToProjectTaskReference(environment, timeslip.ProjectTaskId),
            User = LinkedResourceWriteMapper.ToUserReference(environment, timeslip.UserId),
            Project = LinkedResourceWriteMapper.ToProjectReference(environment, timeslip.ProjectId),
            DatedOn = timeslip.DatedOn,
            Hours = timeslip.Hours,
            Comment = timeslip.Comment
        };
    }
}
