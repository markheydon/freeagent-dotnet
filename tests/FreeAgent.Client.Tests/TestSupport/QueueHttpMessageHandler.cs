using System.Net;
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
        if (_responses.Count == 0)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("No queued response")
            });
        }

        return Task.FromResult(_responses.Dequeue()(request));
    }
}
