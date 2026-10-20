using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Models.Expenses;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.StockItems;
using FreeAgent.Client.Models.Users;
using FreeAgent.Client.Services.Categories;

namespace FreeAgent.Client.Services.Expenses;

/// <summary>
/// Service for interacting with FreeAgent expenses.
/// </summary>
/// <remarks>
/// Expenses require <c>user</c> and <c>category</c> (or mileage) on create.
/// See also <see cref="Users.UserService"/> and <see cref="Categories.CategoryService"/>.
/// </remarks>
public sealed class ExpenseService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the expenses service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal ExpenseService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists one page of expenses.
    /// </summary>
    public async Task<PaginatedResponse<Expense>> ListAsync(
        int page = 1,
        int perPage = 25,
        string? view = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        ProjectReference? project = null,
        long? projectId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(perPage, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(perPage, 100);

        var queryParameters = new List<KeyValuePair<string, string>>
        {
            new("page", page.ToString(CultureInfo.InvariantCulture)),
            new("per_page", perPage.ToString(CultureInfo.InvariantCulture))
        };

        if (!string.IsNullOrWhiteSpace(view))
        {
            queryParameters.Add(new KeyValuePair<string, string>("view", view));
        }

        if (fromDate is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "from_date",
                fromDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        if (toDate is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "to_date",
                toDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        if (updatedSince is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "updated_since",
                updatedSince.Value.ToString("o", CultureInfo.InvariantCulture)));
        }

        var projectFilter = ResolveProjectFilter(project, projectId);
        if (projectFilter is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("project", projectFilter));
        }

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("expenses", queryParameters);
        var response = await _requestClient.GetWithMetadataAsync<ExpensesResponse>(endpoint, cancellationToken);

        if (response.Data.Expenses is null)
        {
            throw new FreeAgentApiException("Expenses data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(
            response,
            page,
            perPage,
            response.Data.Expenses.Count);

        return new PaginatedResponse<Expense>(
            page,
            perPage,
            total,
            response.Data.Expenses);
    }

    /// <summary>
    /// Lists all expenses across all pages.
    /// </summary>
    public async IAsyncEnumerable<Expense> ListAutoPagingAsync(
        int perPage = 25,
        string? view = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        ProjectReference? project = null,
        long? projectId = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var expensesPage = await ListAsync(
                page,
                perPage,
                view,
                fromDate,
                toDate,
                updatedSince,
                project,
                projectId,
                cancellationToken);

            foreach (var expense in expensesPage.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return expense;
            }

            if (!expensesPage.HasNextPage)
            {
                yield break;
            }

            page = expensesPage.NextPage!.Value;
        }
    }

    /// <summary>
    /// Gets a single expense by identifier.
    /// </summary>
    public Task<Expense> GetExpenseAsync(long expenseId, CancellationToken cancellationToken = default) =>
        GetExpenseAsync(expenseId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single expense by identifier with optional linked-resource hydration.
    /// </summary>
    public async Task<Expense> GetExpenseAsync(
        long expenseId,
        ExpenseGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(expenseId);

        var response = await _requestClient.GetAsync<ExpenseResponse>(
            $"expenses/{expenseId}",
            cancellationToken);

        if (response.Expense is null)
        {
            throw new FreeAgentApiException("Expense data missing from API response");
        }

        await HydrateLinkedResourcesAsync(response.Expense, options, cancellationToken);

        return response.Expense;
    }

    /// <summary>
    /// Creates an expense.
    /// </summary>
    public async Task<Expense> CreateExpenseAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expense);

        var content = FreeAgentJsonSerializer.CreateContent(new ExpenseRequest
        {
            Expense = ExpenseWritePayload.FromExpense(expense, _requestClient.Environment)
        });

        var response = await _requestClient.PostAsync<ExpenseResponse>("expenses", content, cancellationToken);

        if (response.Expense is null)
        {
            throw new FreeAgentApiException("Expense data missing from API response");
        }

        return response.Expense;
    }

    /// <summary>
    /// Creates multiple expenses in one request.
    /// </summary>
    public async Task<IReadOnlyList<Expense>> CreateExpensesAsync(
        IReadOnlyList<Expense> expenses,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expenses);

        if (expenses.Count == 0)
        {
            throw new ArgumentException("At least one expense is required.", nameof(expenses));
        }

        var content = FreeAgentJsonSerializer.CreateContent(new ExpensesBatchRequest
        {
            Expenses = expenses
                .Select(e => ExpenseWritePayload.FromExpense(e, _requestClient.Environment))
                .ToList()
        });

        var response = await _requestClient.PostAsync<ExpensesResponse>("expenses", content, cancellationToken);

        if (response.Expenses is null)
        {
            throw new FreeAgentApiException("Expenses data missing from API response");
        }

        return response.Expenses;
    }

    /// <summary>
    /// Updates an expense.
    /// </summary>
    public async Task<Expense> UpdateExpenseAsync(
        long expenseId,
        Expense expense,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(expenseId);
        ArgumentNullException.ThrowIfNull(expense);

        var content = FreeAgentJsonSerializer.CreateContent(new ExpenseRequest
        {
            Expense = ExpenseWritePayload.FromExpense(expense, _requestClient.Environment)
        });

        var response = await _requestClient.PutAsync<ExpenseResponse>(
            $"expenses/{expenseId}",
            content,
            cancellationToken);

        if (response.Expense is null)
        {
            throw new FreeAgentApiException("Expense data missing from API response");
        }

        return response.Expense;
    }

    /// <summary>
    /// Deletes an expense.
    /// </summary>
    public Task DeleteExpenseAsync(long expenseId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(expenseId);

        return _requestClient.DeleteAsync($"expenses/{expenseId}", cancellationToken);
    }

    /// <summary>
    /// Gets mileage settings for the current company.
    /// </summary>
    public async Task<MileageSettings> GetMileageSettingsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<MileageSettingsResponse>(
            "expenses/mileage_settings",
            cancellationToken);

        if (response.MileageSettings is null)
        {
            throw new FreeAgentApiException("Mileage settings data missing from API response");
        }

        return response.MileageSettings;
    }

    private async Task HydrateLinkedResourcesAsync(
        Expense expense,
        ExpenseGetOptions? options,
        CancellationToken cancellationToken)
    {
        if (options is null)
        {
            return;
        }

        if (options.IncludeUser && expense.User is null && expense.UserId is long userId)
        {
            var userResponse = await _requestClient.GetAsync<UserResponse>($"users/{userId}", cancellationToken);
            if (userResponse.User is null)
            {
                throw new FreeAgentApiException("User data missing from API response");
            }

            expense.AttachUser(userResponse.User);
        }

        if (options.IncludeCategory
            && expense.Category is null
            && !expense.IsMileageCategory
            && expense.CategoryNominalCode is string nominalCode)
        {
            var categoryResponse = await _requestClient.GetAsync<CategorySingleResponse>(
                $"categories/{nominalCode}",
                cancellationToken);
            var category = CategoryResponseMapper.ToCategory(categoryResponse);
            expense.AttachCategory(category);
        }

        if (options.IncludeProject && expense.Project is null && expense.ProjectId is long projectId)
        {
            var projectResponse = await _requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);
            if (projectResponse.Project is null)
            {
                throw new FreeAgentApiException("Project data missing from API response");
            }

            expense.AttachProject(projectResponse.Project);
        }

        if (options.IncludeRebillToProject
            && expense.RebillToProject is null
            && expense.RebillToProjectId is long rebillProjectId)
        {
            var projectResponse = await _requestClient.GetAsync<ProjectResponse>(
                $"projects/{rebillProjectId}",
                cancellationToken);
            if (projectResponse.Project is null)
            {
                throw new FreeAgentApiException("Project data missing from API response");
            }

            expense.AttachRebillToProject(projectResponse.Project);
        }

        if (options.IncludeStockItem && expense.StockItem is null && expense.StockItemId is long stockItemId)
        {
            var stockResponse = await _requestClient.GetAsync<StockItemResponse>($"stock_items/{stockItemId}", cancellationToken);
            if (stockResponse.StockItem is null)
            {
                throw new FreeAgentApiException("Stock item data missing from API response");
            }

            expense.AttachStockItem(stockResponse.StockItem);
        }

        if (options.IncludeRebilledOnInvoice
            && expense.RebilledOnInvoice is null
            && expense.RebilledOnInvoiceId is long invoiceId)
        {
            var invoiceResponse = await _requestClient.GetAsync<InvoiceResponse>($"invoices/{invoiceId}", cancellationToken);
            if (invoiceResponse.Invoice is null)
            {
                throw new FreeAgentApiException("Invoice data missing from API response");
            }

            expense.AttachRebilledOnInvoice(invoiceResponse.Invoice);
        }
    }

    private string? ResolveProjectFilter(ProjectReference? project, long? projectId)
    {
        if (project is not null && projectId is not null)
        {
            throw new ArgumentException("Specify either project or projectId, not both.", nameof(project));
        }

        if (project is not null)
        {
            return project.Value.Uri;
        }

        if (projectId is null)
        {
            return null;
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(projectId.Value);
        return ProjectReference.ForEnvironment(_requestClient.Environment, projectId.Value).Uri;
    }
}
