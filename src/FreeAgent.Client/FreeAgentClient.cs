using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Services.Attachments;
using FreeAgent.Client.Services.BankAccounts;
using FreeAgent.Client.Services.BankFeeds;
using FreeAgent.Client.Services.BankTransactionExplanations;
using FreeAgent.Client.Services.BankTransactions;
using FreeAgent.Client.Services.Bills;
using FreeAgent.Client.Services.CapitalAssetTypes;
using FreeAgent.Client.Services.Categories;
using FreeAgent.Client.Services.CisBands;
using FreeAgent.Client.Services.CisSettings;
using FreeAgent.Client.Services.Company;
using FreeAgent.Client.Services.Contacts;
using FreeAgent.Client.Services.CreditNoteReconciliations;
using FreeAgent.Client.Services.CreditNotes;
using FreeAgent.Client.Services.EmailAddresses;
using FreeAgent.Client.Services.Estimates;
using FreeAgent.Client.Services.Expenses;
using FreeAgent.Client.Services.HirePurchases;
using FreeAgent.Client.Services.Invoices;
using FreeAgent.Client.Services.Notes;
using FreeAgent.Client.Services.PriceListItems;
using FreeAgent.Client.Services.ProfitAndLoss;
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
    public ProjectTaskService ProjectTasks { get; }

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
    /// Attachments API service.
    /// </summary>
    public AttachmentsService Attachments { get; }

    /// <summary>
    /// Bills API service.
    /// </summary>
    public BillService Bills { get; }

    /// <summary>
    /// Bank accounts API service.
    /// </summary>
    public BankAccountsService BankAccounts { get; }

    /// <summary>
    /// Bank feeds API service.
    /// </summary>
    public BankFeedsService BankFeeds { get; }

    /// <summary>
    /// Bank transactions API service.
    /// </summary>
    public BankTransactionsService BankTransactions { get; }

    /// <summary>
    /// Bank transaction explanations API service.
    /// </summary>
    public BankTransactionExplanationsService BankTransactionExplanations { get; }

    /// <summary>
    /// Hire purchases API service.
    /// </summary>
    public HirePurchasesService HirePurchases { get; }

    /// <summary>
    /// Expenses API service.
    /// </summary>
    public ExpenseService Expenses { get; }

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
    /// Capital asset types API service.
    /// </summary>
    public CapitalAssetTypesService CapitalAssetTypes { get; }

    /// <summary>
    /// CIS bands API service.
    /// </summary>
    public CisBandsService CisBands { get; }

    /// <summary>
    /// CIS settings API service.
    /// </summary>
    public CisSettingsService CisSettings { get; }

    /// <summary>
    /// Profit and loss report API service.
    /// </summary>
    public ProfitAndLossService ProfitAndLoss { get; }

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
        ProjectTasks = new ProjectTaskService(_httpClient);
        Timeslips = new TimeslipService(_httpClient);
        Notes = new NoteService(_httpClient);
        Invoices = new InvoiceService(_httpClient);
        Estimates = new EstimateService(_httpClient);
        RecurringInvoices = new RecurringInvoiceService(_httpClient);
        CreditNotes = new CreditNotesService(_httpClient);
        Attachments = new AttachmentsService(_httpClient);
        Bills = new BillService(_httpClient);
        BankAccounts = new BankAccountsService(_httpClient);
        BankFeeds = new BankFeedsService(_httpClient);
        BankTransactions = new BankTransactionsService(_httpClient);
        BankTransactionExplanations = new BankTransactionExplanationsService(_httpClient);
        HirePurchases = new HirePurchasesService(_httpClient);
        Expenses = new ExpenseService(_httpClient);
        CreditNoteReconciliations = new CreditNoteReconciliationsService(_httpClient);
        StockItems = new StockItemsService(_httpClient);
        PriceListItems = new PriceListItemsService(_httpClient);
        CapitalAssetTypes = new CapitalAssetTypesService(_httpClient);
        CisBands = new CisBandsService(_httpClient);
        CisSettings = new CisSettingsService(_httpClient);
        ProfitAndLoss = new ProfitAndLossService(_httpClient);
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
