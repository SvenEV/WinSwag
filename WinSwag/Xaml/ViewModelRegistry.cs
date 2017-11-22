using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WinSwag.Xaml
{
    public static class ViewModelRegistry
    {
        private static readonly Dictionary<Type, Type> _viewModelTypes; // model -> viewmodel
        private static readonly ConditionalWeakTable<object, object> _viewModels = new ConditionalWeakTable<object, object>();

        static ViewModelRegistry()
        {
            _viewModelTypes = typeof(App).Assembly.GetTypes()
                .Select(t => (m: t.GetInterface("IViewModel`1")?.GetGenericArguments()[0], vm: t))
                .Where(t => t.m != null)
                .ToDictionary(t => t.m, t => t.vm);
        }

        public static T ViewModelFor<T>(object model) => (T)ViewModelFor(model);

        public static object ViewModelFor(object model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var modelType = model.GetType();

            return _viewModels.GetValue(model, _ =>
            {
                var typesToCheck = modelType.GetInterfaces().Prepend(modelType);

                foreach (var type in typesToCheck)
                    if (_viewModelTypes.TryGetValue(type, out var vmType))
                        return Activator.CreateInstance(vmType, model);

                throw new InvalidOperationException($"No ViewModel-type is known for Model-type '{model.GetType().Name}'");
            });
        }
    }
}
