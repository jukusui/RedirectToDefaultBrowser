using Shared.Ex;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Linq;
using Jukusui.Notify;

namespace Shared
{
    public class FullRedirectTest : NotifyBase, INotifyDataErrorInfo
    {


        private bool hasErrors;
        private string test;
        private Uri testUri;
        private string inputError;
        private string testingError;
        private IList<RedirectSetting> redirects;
        private string output;

        public string Test
        {
            get => test;
            set
            {
                if (OnPropertyChanged(ref test, value))
                    if (Uri.TryCreate(value, UriKind.Absolute, out Uri res))
                    {
                        InputError = "";
                        TestUri = res;
                    }
                    else
                    {
                        InputError = (new NotUrlException(value)).Message;
                        TestUri = null;
                    }
            }
        }

        public Uri TestUri
        {
            get => testUri; private set
            {
                if (OnPropertyChanged(ref testUri, value))
                    RunTest();
            }
        }

        public string Output { get => output; private set => OnPropertyChanged(ref output, value); }

        public IList<RedirectSetting> Redirects
        {
            get => redirects; set
            {
                var old = redirects;
                if (OnPropertyChanged(ref redirects, value))
                {
                    if (old != null && old is INotifyCollectionChanged oldCollection)
                        oldCollection.CollectionChanged -= OnRedirectsCollectionChanged;
                    if (value != null && value is INotifyCollectionChanged newCollection)
                        newCollection.CollectionChanged += OnRedirectsCollectionChanged;
                    UpdateCollectionBind();
                }
            }
        }

        private List<RedirectSetting> WatchedRedirects { get; } = new List<RedirectSetting>();

        private void OnRedirectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => UpdateCollectionBind();

        private void UpdateCollectionBind()
        {
            var removed = WatchedRedirects.Except(Redirects);
            var added = redirects.Except(WatchedRedirects);
            var anyChange = false;
            foreach (var item in removed)
            {
                item.PropertyChanged -= OnRedirectSettingChanged;
                anyChange = true;
            }
            foreach (var item in added)
            {
                item.PropertyChanged += OnRedirectSettingChanged;
                anyChange = true;
            }
            WatchedRedirects.Clear();
            WatchedRedirects.AddRange(Redirects);
            if (anyChange)
                RunTest();
        }

        private void OnRedirectSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case null:
                case nameof(RedirectSetting.Enable):
                case nameof(RedirectSetting.Input):
                case nameof(RedirectSetting.Output):
                case nameof(RedirectSetting.WillRemove):
                    RunTest();
                    break;
            }
        }

        public bool HasErrors { get => hasErrors; }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void RunTest()
        {
            if (TestUri == null)
            {
                TestingError = "";
                Output = "Error";
            }
            else
            {
                var uri = TestUri;
                try
                {
                    foreach (var item in Redirects)
                    {
                        if (item.Enable && !item.WillRemove)
                        {
                            uri = item.Apply(uri);
                        }
                    }
                    Output = uri.ToString();
                    TestingError = "";
                }
                catch (Exception ex)
                {
                    TestingError = ex.Message;
                    Output = "Error";
                }
            }
        }

        private void UpdateErrorState()
        {
            var newState =
                InputError != "" ||
                TestingError != "";
            if (newState != hasErrors)
            {
                hasErrors = newState;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
            }
        }

        public string InputError
        {
            get => inputError;
            set
            {
                if (OnPropertyChanged(ref inputError, value))
                {
                    UpdateErrorState();
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Test)));
                }
            }
        }

        public string TestingError
        {
            get => testingError;
            set
            {
                if (OnPropertyChanged(ref testingError, value))
                {
                    UpdateErrorState();
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Redirects)));
                }
            }
        }


        public IEnumerable GetErrors(string propertyName)
        {
            if ((propertyName == null || propertyName == nameof(Test)) &&
                InputError != "")
                yield return InputError;
            if ((propertyName == null || propertyName == nameof(Redirects)) &&
                TestingError != "")
                yield return TestingError;

        }
    }
}
