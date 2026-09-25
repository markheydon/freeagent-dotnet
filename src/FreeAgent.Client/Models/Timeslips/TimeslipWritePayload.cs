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
    [JsonConverter(typeof(WriteLinkJsonConverter<ProjectTaskReference>))]
    public WriteLink<ProjectTaskReference>? ProjectTask { get; set; }

    [JsonPropertyName("user")]
    [JsonConverter(typeof(WriteLinkJsonConverter<UserReference>))]
    public WriteLink<UserReference>? User { get; set; }

    [JsonPropertyName("project")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ProjectReference>))]
    public WriteLink<ProjectReference>? Project { get; set; }

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
            ProjectTask = LinkedResourceWriteMapper.ResolveProjectTaskReference(
                environment,
                timeslip.ProjectTaskIdBacking,
                timeslip.ProjectTaskLinkId),
            User = LinkedResourceWriteMapper.ResolveUserReference(
                environment,
                timeslip.UserIdBacking,
                timeslip.UserLinkId),
            Project = LinkedResourceWriteMapper.ResolveProjectReference(
                environment,
                timeslip.ProjectIdBacking,
                timeslip.ProjectLinkId),
            DatedOn = timeslip.DatedOn,
            Hours = timeslip.Hours,
            Comment = timeslip.Comment
        };
    }
}
