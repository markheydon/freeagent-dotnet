using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Categories;

namespace FreeAgent.Client.Services.Categories;

/// <summary>
/// Service for interacting with FreeAgent chart-of-accounts categories.
/// </summary>
public sealed class CategoryService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the category service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal CategoryService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all categories for the current company.
    /// </summary>
    /// <param name="includeSubAccounts">When <see langword="true"/>, includes sub accounts instead of top-level accounts where they exist.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All category sets returned by FreeAgent</returns>
    public async Task<CategorySets> GetCategoriesAsync(
        bool includeSubAccounts = false,
        CancellationToken cancellationToken = default)
    {
        var endpoint = includeSubAccounts
            ? FreeAgentQueryStringBuilder.BuildEndpoint("categories", [new KeyValuePair<string, string>("sub_accounts", "true")])
            : "categories";

        var response = await _requestClient.GetAsync<CategoryListResponse>(endpoint, cancellationToken);
        return CategoryResponseMapper.ToCollection(response);
    }

    /// <summary>
    /// Gets a single category by nominal code.
    /// </summary>
    /// <param name="nominalCode">Category nominal code (for example <c>001</c> or <c>602-1</c>)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category details</returns>
    public async Task<Category> GetCategoryAsync(string nominalCode, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);

        var response = await _requestClient.GetAsync<CategorySingleResponse>($"categories/{nominalCode}", cancellationToken);
        return CategoryResponseMapper.ToCategory(response);
    }

    /// <summary>
    /// Creates a category.
    /// </summary>
    /// <param name="category">Category attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category</returns>
    public async Task<Category> CreateCategoryAsync(CategoryWritePayload category, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(category);

        if (category.CategoryGroup is null)
        {
            throw new ArgumentException("CategoryGroup is required when creating a category.", nameof(category));
        }

        var content = FreeAgentJsonSerializer.CreateContent(new CategoryRequest { Category = category });
        var response = await _requestClient.PostAsync<CategorySingleResponse>("categories", content, cancellationToken);
        return CategoryResponseMapper.ToCategory(response);
    }

    /// <summary>
    /// Updates a category.
    /// </summary>
    /// <param name="nominalCode">Existing category nominal code</param>
    /// <param name="category">Category attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category</returns>
    public async Task<Category> UpdateCategoryAsync(
        string nominalCode,
        CategoryWritePayload category,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);
        ArgumentNullException.ThrowIfNull(category);

        var content = FreeAgentJsonSerializer.CreateContent(new CategoryRequest { Category = category });
        var response = await _requestClient.PutAsync<CategorySingleResponse>($"categories/{nominalCode}", content, cancellationToken);
        return CategoryResponseMapper.ToCategory(response);
    }

    /// <summary>
    /// Deletes a category.
    /// </summary>
    /// <param name="nominalCode">Category nominal code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteCategoryAsync(string nominalCode, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);

        return _requestClient.DeleteAsync($"categories/{nominalCode}", cancellationToken);
    }
}
