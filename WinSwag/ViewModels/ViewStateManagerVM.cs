using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Popups;
using WinSwag.Services;

namespace WinSwag.ViewModels
{
    public class ViewStateManagerVM : ViewModelBase, IViewStateManagerVM
    {
        private readonly ObservableCollection<ViewTask> _activeTasks = new ObservableCollection<ViewTask>();

        public bool IsBusy => _activeTasks.Count > 0;

        public bool IsIdle => _activeTasks.Count == 0;

        public IReadOnlyList<ViewTask> ActiveTasks => _activeTasks;

        public IDisposable BeginTask(string description)
        {
            var task = new ViewTask(description, OnTaskDisposed);
            _activeTasks.Add(task);
            RaisePropertyChanged(nameof(IsBusy));
            RaisePropertyChanged(nameof(IsIdle));
            return task;

            void OnTaskDisposed(ViewTask t)
            {
                _activeTasks.Remove(t);
                RaisePropertyChanged(nameof(IsBusy));
                RaisePropertyChanged(nameof(IsIdle));
            }
        }

        public async Task ShowMessageAsync(string content, string title)
        {
            var dialog = new MessageDialog(content, title);
            await dialog.ShowAsync();
        }
    }
}
