using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using WinSwag.Core;
using WinSwag.Core.Extensions;

namespace WinSwag.Services
{
    public class ApplicationInfo : ObservableObjectEx
    {
        private const string FirstTimeAppStartKey = "AppStartedOnce";

        private ElementTheme _selectedTheme = ElementTheme.Default;

        public string Version
        {
            get
            {
                var v = Package.Current.Id.Version;
                return $"v{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public string PackageFamilyName => Package.Current.Id.FamilyName;

        public OpenApiSettings Settings { get; }

        /// <summary>
        /// Indicates whether the app was launched for the first time after being installed.
        /// </summary>
        public bool IsLaunchedFirstTime { get; }

        public ElementTheme SelectedTheme
        {
            get => _selectedTheme;
            set => Set(ref _selectedTheme, value);
        }

        public IReadOnlyList<ElementTheme> ThemeOptions { get; } =
            new[] { ElementTheme.Default, ElementTheme.Dark, ElementTheme.Light };

        public ApplicationInfo()
        {
            var settings = ApplicationData.Current.LocalSettings.Values;
            IsLaunchedFirstTime = !settings.ContainsKey(FirstTimeAppStartKey);
            if (IsLaunchedFirstTime)
                settings.Add(FirstTimeAppStartKey, true);

            Settings = new OpenApiSettings(OpenApiSettings.Default)
            {
                ConfigureRequest = request => request.Headers.UserAgent.Add(new ProductInfoHeaderValue("WinSwag", Version)),
                ArgumentTypes =
                {
                    new ArgumentTypeInfo
                    {
                        Type = typeof(FileArgument),
                        IsApplicable = spec => spec.Type == JsonObjectType.File
                    }
                },
                ResponseContentTypes =
                {
                    new ResponseContentTypeInfo
                    {
                        Type = typeof(ImageResponse),
                        IsApplicable = spec => spec.Content?.Headers.ContentType?.MediaType?.StartsWith("image/") ?? false
                    },
                    new ResponseContentTypeInfo
                    {
                        Type = typeof(AudioResponse),
                        IsApplicable = spec => spec.Content?.Headers.ContentType?.MediaType?.StartsWith("audio/") ?? false
                    }
                }
            };
        }
    }
}
