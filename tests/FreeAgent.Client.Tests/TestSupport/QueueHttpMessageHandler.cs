using System.Net.Http;

namespace FreeAgent.Client.Tests.TestSupport;

internal sealed class QueueHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpRequestMessage, Task<HttpResponseMessage>>> _responses;

    public QueueHttpMessageHandler(IEnumerable<Func<HttpRequestMessage, HttpResponseMessage>> responses)
        : this(responses.Select(static response => (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(request => Task.FromResult(response(request)))))
    {
    }

    public QueueHttpMessageHandler(params Func<HttpRequestMessage, HttpResponseMessage>[] responses)
        : this((IEnumerable<Func<HttpRequestMessage, HttpResponseMessage>>)responses)
    {
    }

    public QueueHttpMessageHandler(IEnumerable<Func<HttpRequestMessage, Task<HttpResponseMessage>>> responses)
    {
        _responses = new Queue<Func<HttpRequestMessage, Task<HttpResponseMessage>>>(responses);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_responses.Count == 0)
        {
            throw new InvalidOperationException("No queued response available. Ensure the test has enqueued the correct number of responses.");
        }

        return _responses.Dequeue()(request);
    }
}
