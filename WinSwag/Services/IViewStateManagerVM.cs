using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WinSwag.Services
{
    public interface IViewStateManagerVM : INotifyPropertyChanged
    {
        bool IsBusy { get; }

        bool IsIdle { get; }

        IReadOnlyList<ViewTask> ActiveTasks { get; }

        IDisposable BeginTask(string description);

        Task ShowMessageAsync(string content, string title);
    }

    public class ViewTask : IDisposable
    {
        private readonly Action<ViewTask> _disposed;

        public string Description { get; }

        public ViewTask(string description, Action<ViewTask> disposed)
        {
            Description = description;
            _disposed = disposed;
        }

        public void Dispose() => _disposed?.Invoke(this);
    }
}
