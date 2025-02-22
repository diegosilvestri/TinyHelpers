﻿namespace TinyHelpers.Http;

public class HeaderInjectorHttpClientHandler : DelegatingHandler
{
    private readonly Func<HttpRequestMessage, Task<Dictionary<string, string>>> getHeaders;

    public HeaderInjectorHttpClientHandler(Func<HttpRequestMessage, Task<Dictionary<string, string>>> getHeaders)
    {
        this.getHeaders = getHeaders ?? throw new ArgumentNullException(nameof(getHeaders));
    }

    public HeaderInjectorHttpClientHandler(Func<HttpRequestMessage, Task<Dictionary<string, string>>> getHeaders, HttpMessageHandler innerHandler)
        : base(innerHandler)
    {
        this.getHeaders = getHeaders ?? throw new ArgumentNullException(nameof(getHeaders));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var headers = await getHeaders(request).ConfigureAwait(false);
        if (headers is not null)
        {
            foreach (var header in headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
