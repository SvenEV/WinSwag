using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinSwag.Core;
using WinSwag.Core.Extensions;

namespace WinSwag.ViewModels
{
    public class ParameterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FallbackTemplate { get; set; }
        public DataTemplate BoolTemplate { get; set; }
        public DataTemplate EnumTemplate { get; set; }
        public DataTemplate DateTimeTemplate { get; set; }
        public DataTemplate FileTemplate { get; set; }

        public ParameterTemplateSelector()
        {
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (!(item is IArgument arg))
                return null;

            switch (arg)
            {
                case BoolArgument _: return BoolTemplate;
                case EnumArgument _: return EnumTemplate;
                case DateTimeArgument _: return DateTimeTemplate;
                case FileArgument _: return FileTemplate;
                default: return FallbackTemplate;
            }
        }
    }
}
