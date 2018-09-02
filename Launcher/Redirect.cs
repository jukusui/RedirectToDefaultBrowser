using Launcher.Ex;
using Notify;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Launcher
{
    class Redirect

    {
        private Redirect()
        {
            Redirects = new ObservableCollection<RedirectSetting>(
                Properties.Settings.Default.Redirects ?? Enumerable.Empty<RedirectSetting>());
            Redirects.CollectionChanged += RedirectsCahnged;
            foreach (var redirect in Redirects)
                redirect.PropertyChanged += RedirectCahnged;
        }

        ~Redirect()
        {
            if (IsDirty)
            {
                var toSave = Redirects.ToList();
                foreach (var redirect in Redirects)
                    if (redirect.WillRemove)
                        toSave.Remove(redirect);
                Properties.Settings.Default.Redirects =
                    toSave.ToArray();
                Properties.Settings.Default.Save();
            }
        }

        private void RedirectCahnged(object sender, PropertyChangedEventArgs e)
        {
            Redirects.CollectionChanged -= RedirectsCahnged;
            foreach (var redirect in Redirects)
                redirect.PropertyChanged -= RedirectCahnged;
            IsDirty = true;
        }

        private void RedirectsCahnged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Redirects.CollectionChanged -= RedirectsCahnged;
            foreach (var redirect in Redirects)
                redirect.PropertyChanged -= RedirectCahnged;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    foreach (var redirect in e.OldItems)
                        ((RedirectSetting)redirect).PropertyChanged -= RedirectCahnged;
                    break;
            }
            IsDirty = true;
        }

        private bool IsDirty { get; set; } = false;

        private static Redirect instance = null;
        public static Redirect Instance
        {
            get => instance ?? (instance = new Redirect());
        }

        public ObservableCollection<RedirectSetting> Redirects { get; }

    }

    [Serializable]
    public class RedirectSetting : NotifyBase
    {
        private bool enable = true;
        public bool Enable { get => enable; set => OnPropertyChanged(ref enable, value); }
        private string title = "";
        public string Title { get => title; set => OnPropertyChanged(ref title, value); }
        private string input = "";
        public string Input { get => input; set => OnPropertyChanged(ref input, value); }
        private string output = "";
        public string Output { get => output; set => OnPropertyChanged(ref output, value); }

        [NonSerialized]
        private bool willRemove = false;
        public bool WillRemove { get => willRemove; set => OnPropertyChanged(ref willRemove, value); }

        public Uri Apply(Uri uri)
        {
            if (WillRemove || !enable) return uri;
            var str = uri.AbsoluteUri;
            try
            {
                var strRes = Regex.Replace(str, input, output);
                if (Uri.TryCreate(strRes, UriKind.Absolute, out Uri res))
                    return Opener.VaridateUri(res);
                else
                    throw new NotUrlException();
            }
            catch (Exception ex)
            {
                throw new RedirectException(this, ex);
            }
        }

    }

    public class RedirectException : Exception
    {
        public RedirectException(RedirectSetting setting, Exception innerException) :
            base(
                $"{string.Format(Properties.ExResources.ExRedirect, setting.Title)}\n{innerException.Message}",
                innerException)
        { }
    }
}
