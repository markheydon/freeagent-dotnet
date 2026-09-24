using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Timeslips;

/// <summary>
/// Writable timeslip attributes for create and update requests.
/// </summary>
internal sealed class TimeslipWritePayload
{
    [JsonPropertyName("task")]
    public TaskReference? Task { get; set; }

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

    public static TimeslipWritePayload FromTimeslip(Timeslip timeslip)
    {
        ArgumentNullException.ThrowIfNull(timeslip);

        return new TimeslipWritePayload
        {
            Task = timeslip.LinkedTask
                ?? (timeslip.TaskLink?.Uri is string taskUri ? TaskReference.Parse(taskUri) : null),
            User = timeslip.LinkedUser
                ?? (timeslip.UserLink?.Uri is string userUri ? UserReference.Parse(userUri) : null),
            Project = timeslip.LinkedProject
                ?? (timeslip.ProjectLink?.Uri is string projectUri ? ProjectReference.Parse(projectUri) : null),
            DatedOn = timeslip.DatedOn,
            Hours = timeslip.Hours,
            Comment = timeslip.Comment
        };
    }
}
