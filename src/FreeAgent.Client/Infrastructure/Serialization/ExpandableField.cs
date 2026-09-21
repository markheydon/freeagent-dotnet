using System.Text.Json.Serialization;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Internal wire adapter for a link field that may be a resource URI string or a nested object.
/// </summary>
/// <typeparam name="T">Linked resource type.</typeparam>
[JsonConverter(typeof(ExpandableFieldJsonConverterFactory))]
internal sealed class ExpandableField<T> where T : class, IFreeAgentResource
{
    public ExpandableField(string? uri, T? value = null)
    {
        Uri = uri;
        Value = value;
    }

    public string? Uri { get; }

    public T? Value { get; }

    public bool IsExpanded => Value is not null;

    public long? Id
    {
        get
        {
            if (Value is not null)
            {
                return Value.ResourceId;
            }

            return FreeAgentResourceId.TryParse(Uri, out var id) ? id : null;
        }
    }
}
