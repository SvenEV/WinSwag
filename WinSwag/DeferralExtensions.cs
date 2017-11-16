using System;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace WinSwag
{
    public sealed class Disposable : IDisposable
    {
        private readonly Action _dispose;
        public Disposable(Action dispose) => _dispose = dispose ?? throw new ArgumentNullException(nameof(dispose));
        public void Dispose() => _dispose();
    }

    public static class DeferralExtensions
    {
        public static IDisposable ToDisposable(this Deferral d) => new Disposable(d.Complete);
        public static IDisposable ToDisposable(this DragOperationDeferral d) => new Disposable(d.Complete);
    }
}
