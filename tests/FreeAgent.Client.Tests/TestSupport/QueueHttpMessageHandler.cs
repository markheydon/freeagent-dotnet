using System.Net.Http;

namespace FreeAgent.Client.Tests.TestSupport;

internal sealed class QueueHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> _responses;

    public QueueHttpMessageHandler(IEnumerable<Func<HttpRequestMessage, HttpResponseMessage>> responses)
    {
        _responses = new Queue<Func<HttpRequestMessage, HttpResponseMessage>>(responses);
    }

    public QueueHttpMessageHandler(params Func<HttpRequestMessage, HttpResponseMessage>[] responses)
        : this((IEnumerable<Func<HttpRequestMessage, HttpResponseMessage>>)responses)
    {
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_responses.Count == 0)
        {
            throw new InvalidOperationException("No queued response available. Ensure the test has enqueued the correct number of responses.");
        }

        return Task.FromResult(_responses.Dequeue()(request));
    }
}
