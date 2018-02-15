using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinSwag.Core;

namespace WinSwag.ViewModels
{
    public class SecuritySchemeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BasicTemplate { get; set; }
        public DataTemplate ApiKeyTemplate { get; set; }
        public DataTemplate OAuth2Template { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (!(item is ISecurityScheme arg))
                return null;

            switch (arg)
            {
                case SecuritySchemeBasic _: return BasicTemplate;
                case SecuritySchemeApiKey _: return ApiKeyTemplate;
                case SecuritySchemeOAuth2 _: return OAuth2Template;
                default: return null;
            }
        }
    }
}
