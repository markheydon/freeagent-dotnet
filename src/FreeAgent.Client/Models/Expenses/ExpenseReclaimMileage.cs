using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Whether mileage is reclaimed at the Approved Mileage Allowance Payments (AMAP) rate.
/// </summary>
[JsonConverter(typeof(JsonInt32EnumConverter<ExpenseReclaimMileage>))]
public enum ExpenseReclaimMileage
{
    /// <summary>Do not reclaim mileage; rebill only.</summary>
    DoNotReclaim = 0,

    /// <summary>Reclaim at the AMAP rate.</summary>
    AtAmapRate = 1
}
