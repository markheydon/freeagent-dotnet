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
    /// Creates an income category.
    /// </summary>
    /// <param name="request">Income category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category</returns>
    public Task<Category> CreateIncomeCategoryAsync(
        CreateIncomeCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return CreateCategoryAsync(CategoryWritePayloadMapper.FromCreate(request), cancellationToken);
    }

    /// <summary>
    /// Creates a cost of sales category.
    /// </summary>
    /// <param name="request">Cost of sales category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category</returns>
    public Task<Category> CreateCostOfSalesCategoryAsync(
        CreateCostOfSalesCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return CreateCategoryAsync(CategoryWritePayloadMapper.FromCreate(request), cancellationToken);
    }

    /// <summary>
    /// Creates an admin expenses category.
    /// </summary>
    /// <param name="request">Admin expenses category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category</returns>
    public Task<Category> CreateAdminExpensesCategoryAsync(
        CreateAdminExpensesCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return CreateCategoryAsync(CategoryWritePayloadMapper.FromCreate(request), cancellationToken);
    }

    /// <summary>
    /// Creates a current asset category.
    /// </summary>
    /// <param name="request">Current asset category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category</returns>
    public Task<Category> CreateCurrentAssetCategoryAsync(
        CreateCurrentAssetCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return CreateCategoryAsync(CategoryWritePayloadMapper.FromCreate(request), cancellationToken);
    }

    /// <summary>
    /// Creates a liabilities category.
    /// </summary>
    /// <param name="request">Liabilities category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category</returns>
    public Task<Category> CreateLiabilitiesCategoryAsync(
        CreateLiabilitiesCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return CreateCategoryAsync(CategoryWritePayloadMapper.FromCreate(request), cancellationToken);
    }

    /// <summary>
    /// Creates an equity category.
    /// </summary>
    /// <param name="request">Equity category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category</returns>
    public Task<Category> CreateEquityCategoryAsync(
        CreateEquityCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return CreateCategoryAsync(CategoryWritePayloadMapper.FromCreate(request), cancellationToken);
    }

    /// <summary>
    /// Updates an income category.
    /// </summary>
    /// <param name="nominalCode">Existing category nominal code</param>
    /// <param name="request">Income category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category</returns>
    public Task<Category> UpdateIncomeCategoryAsync(
        string nominalCode,
        UpdateIncomeCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return UpdateCategoryAsync(nominalCode, CategoryWritePayloadMapper.FromUpdate(request), cancellationToken);
    }

    /// <summary>
    /// Updates a cost of sales category.
    /// </summary>
    /// <param name="nominalCode">Existing category nominal code</param>
    /// <param name="request">Cost of sales category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category</returns>
    public Task<Category> UpdateCostOfSalesCategoryAsync(
        string nominalCode,
        UpdateCostOfSalesCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return UpdateCategoryAsync(nominalCode, CategoryWritePayloadMapper.FromUpdate(request), cancellationToken);
    }

    /// <summary>
    /// Updates an admin expenses category.
    /// </summary>
    /// <param name="nominalCode">Existing category nominal code</param>
    /// <param name="request">Admin expenses category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category</returns>
    public Task<Category> UpdateAdminExpensesCategoryAsync(
        string nominalCode,
        UpdateAdminExpensesCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return UpdateCategoryAsync(nominalCode, CategoryWritePayloadMapper.FromUpdate(request), cancellationToken);
    }

    /// <summary>
    /// Updates a current asset category.
    /// </summary>
    /// <param name="nominalCode">Existing category nominal code</param>
    /// <param name="request">Current asset category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category</returns>
    public Task<Category> UpdateCurrentAssetCategoryAsync(
        string nominalCode,
        UpdateCurrentAssetCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return UpdateCategoryAsync(nominalCode, CategoryWritePayloadMapper.FromUpdate(request), cancellationToken);
    }

    /// <summary>
    /// Updates a liabilities category.
    /// </summary>
    /// <param name="nominalCode">Existing category nominal code</param>
    /// <param name="request">Liabilities category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category</returns>
    public Task<Category> UpdateLiabilitiesCategoryAsync(
        string nominalCode,
        UpdateLiabilitiesCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return UpdateCategoryAsync(nominalCode, CategoryWritePayloadMapper.FromUpdate(request), cancellationToken);
    }

    /// <summary>
    /// Updates an equity category.
    /// </summary>
    /// <param name="nominalCode">Existing category nominal code</param>
    /// <param name="request">Equity category attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category</returns>
    public Task<Category> UpdateEquityCategoryAsync(
        string nominalCode,
        UpdateEquityCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return UpdateCategoryAsync(nominalCode, CategoryWritePayloadMapper.FromUpdate(request), cancellationToken);
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

    private async Task<Category> CreateCategoryAsync(
        CategoryWritePayload payload,
        CancellationToken cancellationToken)
    {
        var content = FreeAgentJsonSerializer.CreateContent(new CategoryRequest { Category = payload });
        var response = await _requestClient.PostAsync<CategorySingleResponse>("categories", content, cancellationToken);
        return CategoryResponseMapper.ToCategory(response);
    }

    private async Task<Category> UpdateCategoryAsync(
        string nominalCode,
        CategoryWritePayload payload,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nominalCode);

        var content = FreeAgentJsonSerializer.CreateContent(new CategoryRequest { Category = payload });
        var response = await _requestClient.PutAsync<CategorySingleResponse>($"categories/{nominalCode}", content, cancellationToken);
        return CategoryResponseMapper.ToCategory(response);
    }
}
