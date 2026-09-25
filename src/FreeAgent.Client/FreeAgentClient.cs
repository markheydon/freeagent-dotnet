using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.Categories;
using FreeAgent.Client.Services.Company;
using FreeAgent.Client.Services.Contacts;
using FreeAgent.Client.Services.CreditNoteReconciliations;
using FreeAgent.Client.Services.CreditNotes;
using FreeAgent.Client.Services.EmailAddresses;
using FreeAgent.Client.Services.Estimates;
using FreeAgent.Client.Services.Invoices;
using FreeAgent.Client.Services.Notes;
using FreeAgent.Client.Services.PriceListItems;
using FreeAgent.Client.Services.Projects;
using FreeAgent.Client.Services.RecurringInvoices;
using FreeAgent.Client.Services.StockItems;
using FreeAgent.Client.Services.Tasks;
using FreeAgent.Client.Services.Timeslips;
using FreeAgent.Client.Services.Users;

namespace FreeAgent.Client;

/// <summary>
/// Main client for interacting with the FreeAgent API.
/// </summary>
public sealed class FreeAgentClient : IDisposable
{
    private readonly FreeAgentHttpClient _httpClient;
    private bool _disposed;

    /// <summary>
    /// Company API service.
    /// </summary>
    public CompanyService Company { get; }

    /// <summary>
    /// Contacts API service.
    /// </summary>
    public ContactService Contacts { get; }

    /// <summary>
    /// Categories API service.
    /// </summary>
    public CategoryService Categories { get; }

    /// <summary>
    /// Users API service.
    /// </summary>
    public UserService Users { get; }

    /// <summary>
    /// Email addresses API service.
    /// </summary>
    public EmailAddressesService EmailAddresses { get; }

    /// <summary>
    /// Projects API service.
    /// </summary>
    public ProjectService Projects { get; }

    /// <summary>
    /// Tasks API service.
    /// </summary>
    public TaskService Tasks { get; }

    /// <summary>
    /// Timeslips API service.
    /// </summary>
    public TimeslipService Timeslips { get; }

    /// <summary>
    /// Notes API service.
    /// </summary>
    public NoteService Notes { get; }

    /// <summary>
    /// Invoices API service.
    /// </summary>
    public InvoiceService Invoices { get; }

    /// <summary>
    /// Estimates API service.
    /// </summary>
    public EstimateService Estimates { get; }

    /// <summary>
    /// Recurring invoices API service.
    /// </summary>
    public RecurringInvoiceService RecurringInvoices { get; }

    /// <summary>
    /// Credit notes API service.
    /// </summary>
    public CreditNotesService CreditNotes { get; }

    /// <summary>
    /// Credit note reconciliations API service.
    /// </summary>
    public CreditNoteReconciliationsService CreditNoteReconciliations { get; }

    /// <summary>
    /// Stock items API service.
    /// </summary>
    public StockItemsService StockItems { get; }

    /// <summary>
    /// Price list items API service.
    /// </summary>
    public PriceListItemsService PriceListItems { get; }

    /// <summary>
    /// Target API environment for this client.
    /// </summary>
    public FreeAgentEnvironment Environment => _httpClient.Environment;

    /// <summary>
    /// Environment-correct resource URI builders.
    /// </summary>
    public FreeAgentResourceUrls Urls { get; }

    /// <summary>
    /// Initializes a new instance with an access token.
    /// </summary>
    /// <param name="accessToken">OAuth access token</param>
    /// <param name="environment">Target API environment. Defaults to <see cref="FreeAgentEnvironment.Production"/>.</param>
    /// <param name="options">HTTP client options</param>
    public FreeAgentClient(
        string accessToken,
        FreeAgentEnvironment environment = FreeAgentEnvironment.Production,
        FreeAgentHttpClientOptions? options = null)
        : this(new FreeAgentHttpClient(accessToken, environment, options))
    {
    }

    /// <summary>
    /// Initializes a new instance with OAuth client and token for automatic refresh.
    /// </summary>
    /// <param name="oauthClient">OAuth client</param>
    /// <param name="token">OAuth token</param>
    /// <param name="environment">Target API environment. Defaults to <see cref="FreeAgentEnvironment.Production"/>.</param>
    /// <param name="options">HTTP client options</param>
    public FreeAgentClient(
        FreeAgentOAuthClient oauthClient,
        OAuthTokenResponse token,
        FreeAgentEnvironment environment = FreeAgentEnvironment.Production,
        FreeAgentHttpClientOptions? options = null)
        : this(new FreeAgentHttpClient(oauthClient, token, environment, options))
    {
    }

    /// <summary>
    /// Initializes a new instance with a custom HTTP client.
    /// </summary>
    /// <param name="httpClient">Custom HTTP client</param>
    /// <param name="accessToken">OAuth access token</param>
    /// <param name="options">HTTP client options</param>
    public FreeAgentClient(HttpClient httpClient, string accessToken, FreeAgentHttpClientOptions? options = null)
        : this(new FreeAgentHttpClient(httpClient, accessToken, options))
    {
    }

    private FreeAgentClient(FreeAgentHttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        _httpClient = httpClient;
        Urls = new FreeAgentResourceUrls(_httpClient.Environment);
        Company = new CompanyService(_httpClient);
        Contacts = new ContactService(_httpClient);
        Categories = new CategoryService(_httpClient);
        Users = new UserService(_httpClient);
        EmailAddresses = new EmailAddressesService(_httpClient);
        Projects = new ProjectService(_httpClient);
        Tasks = new TaskService(_httpClient);
        Timeslips = new TimeslipService(_httpClient);
        Notes = new NoteService(_httpClient);
        Invoices = new InvoiceService(_httpClient);
        Estimates = new EstimateService(_httpClient);
        RecurringInvoices = new RecurringInvoiceService(_httpClient);
        CreditNotes = new CreditNotesService(_httpClient);
        CreditNoteReconciliations = new CreditNoteReconciliationsService(_httpClient);
        StockItems = new StockItemsService(_httpClient);
        PriceListItems = new PriceListItemsService(_httpClient);
    }

    /// <summary>
    /// Disposes the client and its resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes managed resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources</param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }

            _disposed = true;
        }
    }
}
