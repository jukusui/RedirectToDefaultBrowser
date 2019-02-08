using System;
using Windows.System;

namespace Launcher.Ex
{

    public abstract class OpenException : Exception
    {
        internal OpenException(string message) : base(message) { }
    }

    //public class UnknownException : OpenException
    //{
    //    public UnknownException() : base(Properties.ExResources.ExUnknown) { }
    //}

    public class NotUrlException : OpenException, IText
    {
        private NotUrlException() : base(Properties.ExResources.ExNotUrl) { }

        public NotUrlException(string text) : this()
            => Text = text;

        public NotUrlException(string text, string message) : base(Properties.ExResources.ExNotUrl + "\r\n" + message)
        => Text = text;

        public string Text { get; }
    }

    public class NoUrlException : OpenException, IText
    {
        private NoUrlException() : base(Properties.ExResources.ExNoUrl) { }

        public NoUrlException(string text) : this()
            => Text = text;

        public string Text { get; }
    }

    public class BrowserException : OpenException, IUri
    {
        private BrowserException() : base(Properties.ExResources.ExBrowser) { }

        public BrowserException(Uri uri) : this()
            => Uri = uri;

        public BrowserException(string message) : base(Properties.ExResources.ExBrowser + "\r\n" + message) { }

        public BrowserException(Uri uri, string message) : this(message)
            => Uri = uri;

        public static BrowserException FromQueryStatus(LaunchQuerySupportStatus status, Uri uri = null)
        {
            switch (status)
            {
                case LaunchQuerySupportStatus.AppNotInstalled:
                    return new BrowserException(uri, Properties.ExResources.QueryStatusAppNotInstalled);
                case LaunchQuerySupportStatus.AppUnavailable:
                    return new BrowserException(uri, Properties.ExResources.QueryStatusAppUnavailable);
                case LaunchQuerySupportStatus.NotSupported:
                    return new BrowserException(uri, Properties.ExResources.QueryStatusNotSupported);
                case LaunchQuerySupportStatus.Available:
                case LaunchQuerySupportStatus.Unknown:
                default:
                    return new BrowserException(uri, Properties.ExResources.QueryStatusUnknown);
            }
        }

        public Uri Uri { get; }
    }

    public class SchemeException : OpenException, IUri
    {
        private SchemeException() : base(Properties.ExResources.ExScheme) { }

        public SchemeException(Uri uri) : this()
            => Uri = uri;

        public Uri Uri { get; }
    }

    public class NotMSEdgeSchemeException : OpenException, IText
    {
        private NotMSEdgeSchemeException() : base(Properties.ExResources.ExMsEdgeScheme) { }

        public NotMSEdgeSchemeException(string text) : this()
            => Text = text;

        public string Text { get; }
    }

    public interface IUri
    {
        Uri Uri { get; }
    }

    public interface IText
    {
        string Text { get; }
    }
}
