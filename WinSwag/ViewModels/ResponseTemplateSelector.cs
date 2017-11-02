using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinSwag.Models.Responses;

namespace WinSwag.ViewModels
{
    class ResponseTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FallbackTemplate { get; set; }
        public DataTemplate JsonTemplate { get; set; }
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate AudioTemplate { get; set; }

        public ResponseTemplateSelector()
        {
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (!(item is ResponseViewModel arg))
                return null;

            switch (arg)
            {
                case StringResponse _: return StringTemplate;
                case JsonResponse _: return JsonTemplate;
                case ImageResponse _: return ImageTemplate;
                case AudioResponse _: return AudioTemplate;
                default: return FallbackTemplate;
            }
        }
    }
}