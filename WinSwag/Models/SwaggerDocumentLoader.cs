using NSwag;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WinSwag.Models
{
    public static class SwaggerDocumentLoader
    {
        public static async Task<SwaggerDocument> LoadFromUrlAsync(string url)
        {
            var http = new HttpClient();
            var response = await http.GetAsync(url);

            if (response.Content == null)
                throw new InvalidOperationException("Request did not return any content");

            var data = await response.Content.ReadAsStringAsync();

            switch (response.Content.Headers.ContentType?.MediaType)
            {
                case "text/json": return await SwaggerDocument.FromJsonAsync(data);
                case "text/yaml": return await SwaggerYamlDocument.FromYamlAsync(data);
                
                // If no proper content type is provided, try both: JSON, then YAML
                default: return await LoadFromStringAsync(data);
            }
        }

        public static async Task<SwaggerDocument> LoadFromStringAsync(string data)
        {
            try
            {
                return await SwaggerDocument.FromJsonAsync(data);
            }
            catch
            {
                try
                {
                    return await SwaggerYamlDocument.FromYamlAsync(data);
                }
                catch
                {
                    throw new InvalidOperationException("Could not determine content type");
                }
            }
        }
    }
}
