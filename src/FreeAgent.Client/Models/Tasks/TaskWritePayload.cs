using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Tasks;

/// <summary>
/// Writable task attributes for create and update requests.
/// </summary>
internal sealed class TaskWritePayload
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("is_billable")]
    public bool? IsBillable { get; set; }

    [JsonPropertyName("billing_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? BillingRate { get; set; }

    [JsonPropertyName("billing_period")]
    public ProjectTaskBillingPeriod? BillingPeriod { get; set; }

    [JsonPropertyName("status")]
    public ProjectTaskStatus? Status { get; set; }

    public static TaskWritePayload FromProjectTask(ProjectTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return new TaskWritePayload
        {
            Name = task.Name,
            IsBillable = task.IsBillable,
            BillingRate = task.BillingRate,
            BillingPeriod = task.BillingPeriod,
            Status = task.Status
        };
    }
}
