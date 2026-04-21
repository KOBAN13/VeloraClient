using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UI.Core;

namespace UI.Helpers
{
    public static class AutoBindResolver
    {
        private static readonly Dictionary<Type, FieldInfo[]> ViewBinderFieldsCache = new();
        private static readonly Dictionary<Type, FieldInfo[]> ViewModelBinderFieldsCache = new();

        public static ViewBinder[] GetViewBinders(View view)
        {
            return DescribeViewBinders(view)
                .Select(descriptor => descriptor.ViewBinder)
                .ToArray();
        }

        public static ViewModelBinder[] GetViewModelBinders(ViewModel viewModel)
        {
            return DescribeViewModelBinders(viewModel)
                .Select(descriptor => descriptor.ViewModelBinder)
                .ToArray();
        }

        public static void Bind(View view, ViewModel viewModel)
        {
            var viewDescriptors = DescribeViewBinders(view);
            var viewModelDescriptors = DescribeViewModelBinders(viewModel);

            foreach (var viewDescriptor in viewDescriptors)
            {
                var candidates = viewModelDescriptors
                    .Where(descriptor => !descriptor.IsBound && descriptor.ValueType == viewDescriptor.ValueType)
                    .ToList();

                if (candidates.Count == 0)
                    throw new InvalidOperationException($"No ViewModel binder found for `{viewDescriptor.Name}` ({viewDescriptor.ValueType.Name}).");

                var namedCandidates = candidates
                    .Where(candidate => !string.IsNullOrEmpty(viewDescriptor.Key) && viewDescriptor.Key == candidate.Key)
                    .ToList();

                var resolvedCandidate = namedCandidates.Count switch
                {
                    1 => namedCandidates[0],
                    > 1 => throw new InvalidOperationException($"Multiple ViewModel binders match `{viewDescriptor.Name}` with key `{viewDescriptor.Key}`."),
                    _ => candidates.Count switch
                    {
                        1 => candidates[0],
                        _ => throw new InvalidOperationException(
                            $"Multiple ViewModel binders found for `{viewDescriptor.Name}` ({viewDescriptor.ValueType.Name}). " +
                            "Rename fields consistently or set an explicit key in [AutoBind(\"...\")].")
                    }
                };

                resolvedCandidate.IsBound = true;
                viewDescriptor.ViewBinder.ViewModelBinder = resolvedCandidate.ViewModelBinder;
            }
        }

        private static List<ViewDescriptor> DescribeViewBinders(View view)
        {
            return GetFields(view.GetType(), typeof(ViewBinder), ViewBinderFieldsCache)
                .Select(field => CreateViewDescriptor(field, view))
                .Where(descriptor => descriptor != null)
                .ToList();
        }

        private static List<ViewModelDescriptor> DescribeViewModelBinders(ViewModel viewModel)
        {
            return GetFields(viewModel.GetType(), typeof(ViewModelBinder), ViewModelBinderFieldsCache)
                .Select(field => CreateViewModelDescriptor(field, viewModel))
                .Where(descriptor => descriptor != null)
                .ToList();
        }

        private static FieldInfo[] GetFields(Type type, Type binderType, IDictionary<Type, FieldInfo[]> cache)
        {
            if (cache.TryGetValue(type, out var cachedFields))
                return cachedFields;

            var fields = type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.GetCustomAttribute<AutoBindAttribute>() != null)
                .Where(field => binderType.IsAssignableFrom(field.FieldType))
                .ToArray();

            cache[type] = fields;
            return fields;
        }

        private static ViewDescriptor CreateViewDescriptor(FieldInfo field, View view)
        {
            if (field.GetValue(view) is not ViewBinder binder)
                return null;

            return new ViewDescriptor(
                binder,
                field.Name,
                GetBindingKey(field));
        }

        private static ViewModelDescriptor CreateViewModelDescriptor(FieldInfo field, ViewModel viewModel)
        {
            if (field.GetValue(viewModel) is not ViewModelBinder binder)
                return null;

            return new ViewModelDescriptor(
                binder,
                field.Name,
                GetBindingKey(field));
        }

        private static string GetBindingKey(FieldInfo field)
        {
            var attribute = field.GetCustomAttribute<AutoBindAttribute>();
            if (!string.IsNullOrWhiteSpace(attribute?.Key))
                return attribute.Key;

            return NormalizeFieldName(field.Name);
        }

        private static string NormalizeFieldName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return fieldName
                .TrimStart('_')
                .Replace("Binder", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("ViewModel", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("View", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Trim()
                .ToLowerInvariant();
        }

        private sealed class ViewDescriptor
        {
            public ViewDescriptor(ViewBinder viewBinder, string name, string key)
            {
                ViewBinder = viewBinder;
                Name = name;
                Key = key;
            }

            public ViewBinder ViewBinder { get; }
            public string Name { get; }
            public string Key { get; }
            public Type ValueType => ViewBinder.ValueType;
        }

        private sealed class ViewModelDescriptor
        {
            public ViewModelDescriptor(ViewModelBinder viewModelBinder, string name, string key)
            {
                ViewModelBinder = viewModelBinder;
                Name = name;
                Key = key;
            }

            public ViewModelBinder ViewModelBinder { get; }
            public string Name { get; }
            public string Key { get; }
            public Type ValueType => ViewModelBinder.ValueType;
            public bool IsBound { get; set; }
        }
    }
}
