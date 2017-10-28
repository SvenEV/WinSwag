using NJsonSchema;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinSwag.ViewModels
{
    public class ParameterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FallbackTemplate { get; set; }
        public DataTemplate BoolTemplate { get; set; }
        public DataTemplate EnumTemplate { get; set; }
        public DataTemplate DateTimeTemplate { get; set; }

        public ParameterTemplateSelector()
        {
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (!(item is ParameterViewModel param))
                return null;

            if (param.Model.Type == JsonObjectType.Boolean)
                return BoolTemplate ?? FallbackTemplate;

            if (param.Model.Format == "date-time")
                return DateTimeTemplate ?? FallbackTemplate;

            if (param.Model.ActualSchema.IsEnumeration)
                return EnumTemplate ?? FallbackTemplate;

            return FallbackTemplate;
        }
    }
}
