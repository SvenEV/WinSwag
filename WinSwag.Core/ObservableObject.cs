using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace WinSwag.Core
{
    public interface INotifyPropertyChangedEx : INotifyPropertyChanged
    {
        IObservable<PropertyChange> PropertyChanges { get; }
    }

    public class ObservableObjectEx : INotifyPropertyChangedEx
    {
        private readonly Subject<PropertyChange> _propertyChanges = new Subject<PropertyChange>();

        public IObservable<PropertyChange> PropertyChanges => _propertyChanges;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public bool Set<T>(ref T variable, T newValue, [CallerMemberName]string propertyName = null)
        {
            if (!Equals(variable, newValue))
            {
                variable = newValue;
                RaisePropertyChanged(newValue, propertyName);
                return true;
            }
            return false;
        }

        public void RaisePropertyChanged(object newValue, [CallerMemberName]string propertyName = null)
        {
            var change = new PropertyChange(this, propertyName, newValue);
            PropertyChanged?.Invoke(this, change);
            _propertyChanges.OnNext(change);
        }

        
    }

    public class PropertyChange : PropertyChangedEventArgs
    {
        public object Object { get; }
        public object NewValue { get; }

        public PropertyChange(object obj, string propertyName, object newValue) : base(propertyName)
        {
            Object = obj ?? throw new ArgumentNullException(nameof(obj));
            NewValue = newValue;
        }
    }

    public static class ObservableObjectExExtensions
    {
        public static IObservable<T> OfProperty<T>(this IObservable<PropertyChange> o, string propertyName)
        {
            return o
                .Where(change => change.PropertyName == propertyName)
                .Select(change => (T)change.NewValue);
        }
    }
}
