using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class Response
    {
        public string RequestUri { get; }

        public bool IsSuccessful { get; }

        public bool IsFaulted => !IsSuccessful;

        public HttpResponseMessage Message { get; }

        public IResponseContent Content { get; }

        public string Status => Message == null ? "Error" : $"{(int)Message.StatusCode} {Message.StatusCode}";

        public Exception Exception { get; }

        private Response(string requestUri, HttpResponseMessage message, IResponseContent content)
        {
            RequestUri = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            IsSuccessful = true;
        }

        public Response(string requestUri, Exception exception)
        {
            RequestUri = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public static async Task<Response> FromResponseMessageAsync(string requestUri, HttpResponseMessage message, OpenApiSettings settings)
        {
            if (requestUri == null)
                throw new ArgumentNullException(nameof(requestUri));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var type = settings.ResponseContentTypes
                .FirstOrDefault(info => info.IsApplicable?.Invoke(message) ?? false)?.Type ??
                settings.FallbackResponseContentType;

            var content = (IResponseContent)Activator.CreateInstance(type);
            await content.InitAsync(message);

            return new Response(requestUri, message, content);
        }
    }

    public interface IResponseContent
    {
        Task InitAsync(HttpResponseMessage message);
    }
}
