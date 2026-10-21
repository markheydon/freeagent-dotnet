using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Services.Properties;

/// <summary>
/// Service for interacting with FreeAgent landlord properties.
/// </summary>
/// <remarks>
/// Properties apply only to companies of type <c>UkUnincorporatedLandlord</c>.
/// </remarks>
public sealed class PropertiesService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the properties service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal PropertiesService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all properties for the current company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All properties returned by FreeAgent</returns>
    public async Task<IReadOnlyList<LandlordProperty>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<PropertyListResponse>("properties", cancellationToken);

        if (response.Properties is null)
        {
            throw new FreeAgentApiException("Properties data missing from API response");
        }

        return response.Properties;
    }

    /// <summary>
    /// Gets a single property by identifier.
    /// </summary>
    /// <param name="propertyId">Property identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Property details</returns>
    public async Task<LandlordProperty> GetPropertyAsync(long propertyId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(propertyId);

        var response = await _requestClient.GetAsync<PropertyResponse>($"properties/{propertyId}", cancellationToken);

        if (response.Property is null)
        {
            throw new FreeAgentApiException("Property data missing from API response");
        }

        return response.Property;
    }

    /// <summary>
    /// Creates a property.
    /// </summary>
    /// <param name="request">Property attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created property</returns>
    public async Task<LandlordProperty> CreatePropertyAsync(
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new PropertyRequest
        {
            Property = PropertyWritePayload.FromCreate(request)
        });

        var response = await _requestClient.PostAsync<PropertyResponse>("properties", content, cancellationToken);

        if (response.Property is null)
        {
            throw new FreeAgentApiException("Property data missing from API response");
        }

        return response.Property;
    }

    /// <summary>
    /// Updates a property.
    /// </summary>
    /// <param name="propertyId">Property identifier from the resource URL</param>
    /// <param name="request">Property attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated property</returns>
    public async Task<LandlordProperty> UpdatePropertyAsync(
        long propertyId,
        UpdatePropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(propertyId);
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new PropertyRequest
        {
            Property = PropertyWritePayload.FromUpdate(request)
        });

        var response = await _requestClient.PutAsync<PropertyResponse>($"properties/{propertyId}", content, cancellationToken);

        if (response.Property is null)
        {
            throw new FreeAgentApiException("Property data missing from API response");
        }

        return response.Property;
    }

    /// <summary>
    /// Deletes a property.
    /// </summary>
    /// <param name="propertyId">Property identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public System.Threading.Tasks.Task DeletePropertyAsync(long propertyId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(propertyId);

        return _requestClient.DeleteAsync($"properties/{propertyId}", cancellationToken);
    }
}
