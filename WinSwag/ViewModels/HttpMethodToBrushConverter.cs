using NSwag;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace WinSwag.ViewModels
{
    class HttpMethodToBrushConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty GetBrushProperty = DependencyProperty.Register(nameof(GetBrush), typeof(Brush), typeof(HttpMethodToBrushConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty PostBrushProperty = DependencyProperty.Register(nameof(PostBrush), typeof(Brush), typeof(HttpMethodToBrushConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty PutBrushProperty = DependencyProperty.Register(nameof(PutBrush), typeof(Brush), typeof(HttpMethodToBrushConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty PatchBrushProperty = DependencyProperty.Register(nameof(PatchBrush), typeof(Brush), typeof(HttpMethodToBrushConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty DeleteBrushProperty = DependencyProperty.Register(nameof(DeleteBrush), typeof(Brush), typeof(HttpMethodToBrushConverter), new PropertyMetadata(null));
        public static readonly DependencyProperty FallbackBrushProperty = DependencyProperty.Register(nameof(FallbackBrush), typeof(Brush), typeof(HttpMethodToBrushConverter), new PropertyMetadata(null));

        public Brush GetBrush { get => (Brush)GetValue(GetBrushProperty); set => SetValue(GetBrushProperty, value); }
        public Brush PostBrush { get => (Brush)GetValue(PostBrushProperty); set => SetValue(PostBrushProperty, value); }
        public Brush PutBrush { get => (Brush)GetValue(PutBrushProperty); set => SetValue(PutBrushProperty, value); }
        public Brush PatchBrush { get => (Brush)GetValue(PatchBrushProperty); set => SetValue(PatchBrushProperty, value); }
        public Brush DeleteBrush { get => (Brush)GetValue(DeleteBrushProperty); set => SetValue(DeleteBrushProperty, value); }
        public Brush FallbackBrush { get => (Brush)GetValue(FallbackBrushProperty); set => SetValue(GetBrushProperty, value); }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case SwaggerOperationMethod.Get: return GetBrush;
                case SwaggerOperationMethod.Post: return PostBrush;
                case SwaggerOperationMethod.Put: return PutBrush;
                case SwaggerOperationMethod.Patch: return PatchBrush;
                case SwaggerOperationMethod.Delete: return DeleteBrush;
                default: return FallbackBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => 
            throw new NotImplementedException();
    }
}
