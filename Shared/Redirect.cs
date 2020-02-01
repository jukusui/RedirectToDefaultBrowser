using Shared.Ex;
using Notify;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shared
{
    public class Redirect

    {
        public Redirect(RedirectSetting[] settings)
        {
            Redirects = new ObservableCollection<RedirectSetting>(
                settings ?? Enumerable.Empty<RedirectSetting>());
        }

        ~Redirect()
        {

        }

        public bool Attached { get; private set; }
        private RedirectSetting[] AttachedItems { get; set; }

        public void Attach()
        {
            if (!Attached)
            {
                Attached = true;
                Redirects.CollectionChanged += RedirectsCahnged;
                AttachedItems = Redirects.ToArray();
                foreach (var redirect in AttachedItems)
                    redirect.PropertyChanged += RedirectCahnged;
                IsDirty = false;
            }
        }

        public void Detach()
        {
            if (Attached)
            {
                Redirects.CollectionChanged += RedirectsCahnged;
                AttachedItems = Redirects.ToArray();
                foreach (var redirect in AttachedItems)
                    redirect.PropertyChanged += RedirectCahnged;
                AttachedItems = null;
                Attached = false;
            }
        }

        public void Refresh()
        {
            Detach();
            foreach (var redirect in Redirects.ToList())
            {
                if (redirect.WillRemove)
                    Redirects.Remove(redirect);
            }
            Attach();
        }

        private void RedirectCahnged(object sender, PropertyChangedEventArgs e)
        {
            Detach();
            IsDirty = true;
        }

        private void RedirectsCahnged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Detach();
            IsDirty = true;
        }

        public bool IsDirty { get; set; } = false;

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
                    throw new NotUrlException(strRes);
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
