using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinSwag.Controls
{
    public class Table : FrameworkElement
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(TableSpanDefinition), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((Table)d).OnItemsSourceChanged(e);

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Table), new PropertyMetadata(Orientation.Vertical));

        public static readonly DependencyProperty SpanDefinitionsProperty =
            DependencyProperty.Register(nameof(SpanDefinitions), typeof(ObservableCollection<TableSpanDefinition>), typeof(Table), new PropertyMetadata(new ObservableCollection<TableSpanDefinition>()));

        private List<DependencyObject> _uiItems = new List<DependencyObject>();

        public object ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public ObservableCollection<TableSpanDefinition> SpanDefinitions
        {
            get => (ObservableCollection<TableSpanDefinition>)GetValue(SpanDefinitionsProperty);
            set => SetValue(SpanDefinitionsProperty, value);
        }

        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldItems = (e.OldValue as IEnumerable<object>)?.ToList() ?? new List<object>();
            var newItems = (e.NewValue as IEnumerable<object>)?.ToList() ?? new List<object>();

            // regenerate UI items... also when spans change?
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var dataItems = (ItemsSource as IEnumerable<object>)?.ToList() ?? new List<object>();
            
            // 1) determine actual column sizes
            var spanIndices = Enumerable.Range(0, SpanDefinitions.Count);
            var perpIndices = Enumerable.Range(0, dataItems.Count);

            var spanSizes = new double[SpanDefinitions.Count];
            var sumStars = SpanDefinitions.Where(span => span.Size.IsStar).Sum(span => span.Size.Value);

            var remainingSpace =
                Orientation == Orientation.Vertical ? availableSize.Width :
                Orientation == Orientation.Horizontal ? availableSize.Height :
                throw new NotImplementedException();

            foreach (var i in spanIndices.Where(i => SpanDefinitions[i].Size.IsAbsolute))
                remainingSpace -= spanSizes[i] = SpanDefinitions[i].Size.Value;

            foreach (var i in spanIndices.Where(i => SpanDefinitions[i].Size.IsStar))
                spanSizes[i] = SpanDefinitions[i].Size.Value / sumStars * remainingSpace;

            remainingSpace = 0;

            // 2) for the children of each row, determine max height => this is the row height

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
    }

    public class TableSpanDefinition : DependencyObject
    {
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(nameof(Size), typeof(GridLength), typeof(TableSpanDefinition), new PropertyMetadata(new GridLength(1, GridUnitType.Star)));

        public static readonly DependencyProperty DataTemplateProperty =
            DependencyProperty.Register(nameof(DataTemplate), typeof(DataTemplate), typeof(TableSpanDefinition), new PropertyMetadata(null));

        public GridLength Size
        {
            get { return (GridLength)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public DataTemplate DataTemplate
        {
            get { return (DataTemplate)GetValue(DataTemplateProperty); }
            set { SetValue(DataTemplateProperty, value); }
        }
    }
}
