using NSwag;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public interface IParameter
    {
        SwaggerParameter Specification { get; }
        Operation Operation { get; }
        string ParameterId { get; }
        IArgument LocalArgument { get; }
        IArgument GlobalArgument { get; }

        Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri);
    }

    //public interface IParameter<T> : IParameter
    //{
    //    new IArgument<T> LocalArgument { get; }
    //    new IArgument<T> GlobalArgument { get; }
    //}
}
