using System;

namespace Launcher.Ex
{

    public abstract class OpenException : Exception
    {
        internal OpenException(string message) : base(message) { }
    }

    public class UnknownException : OpenException
    {
        public UnknownException() : base(Properties.ExResources.ExUnknown) { }
    }

    public class NotUrlException : OpenException
    {
        public NotUrlException() : base(Properties.ExResources.ExNotUrl) { }
    }

    public class NoUrlException : OpenException
    {
        public NoUrlException() : base(Properties.ExResources.ExNoUrl) { }
    }

    public class BrowserException : OpenException
    {
        public BrowserException() : base(Properties.ExResources.ExBrowser) { }
    }

    public class SchemeException : OpenException
    {
        public SchemeException() : base(Properties.ExResources.ExScheme) { }
    }

    public class MSEdgeSchemeException : OpenException
    {
        public MSEdgeSchemeException() : base(Properties.ExResources.ExMsEdgeScheme) { }
    }
}
