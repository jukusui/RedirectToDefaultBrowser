using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace UserInterface.PartsUI
{
    public static class StringReaderExtension
    {

        public static char ReadChar(this StringReader reader)
        {
            int val;
            if ((val = reader.Read()) < 0)
                return '\0';
            else
                return (char)val;
        }
        public static char PeekChar(this StringReader reader)
        {
            int val;
            if ((val = reader.Peek()) < 0)
                return '\0';
            else
                return (char)val;
        }
    }

    public class SimpleMarkdown : DependencyObject
    {

        public static DependencyProperty MarkdownProperty =
            DependencyProperty.RegisterAttached(
                "Markdown", typeof(string), typeof(SimpleMarkdown),
                new PropertyMetadata(null, OnPropertyChanged));

        public static string? GetMarkdownProperty(DependencyObject d) =>
            d.GetValue(MarkdownProperty) as string;

        public static void SetMarkdownProperty(DependencyObject d, string? value) =>
            d.SetValue(MarkdownProperty, value);


        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineCollection? inline = null;
            if (d is TextBlock textBlock)
                inline = textBlock.Inlines;
            else if (d is Span span)
                inline = span.Inlines;
            if (inline == null)
                return;

            if (e.OldValue != null)
                inline.Clear();
            if (e.NewValue == null || !(e.NewValue is string text))
                return;
            using (var reader = new StringReader(text))
            {

                var span = new Span();
                Read(reader, span);
                inline.Add(span);
            }

        }

        private static void Read(StringReader reader, Span target)
        {
            var builder = new StringBuilder();
            bool ignore = false;
            char current;
            while ((current = reader.ReadChar()) != '\0')
            {
                switch (current)
                {
                    case '\\':
                        ignore = true;
                        break;
                    case '[':
                        if (ignore)
                            builder.Append('[');
                        else
                        {
                            target.Inlines.Add(new Run()
                            {
                                Text = builder.ToString()
                            });
                            builder.Clear();
                            ReadUri(reader, target);
                        }

                        break;
                    default:
                        builder.Append(current);
                        break;
                }
            }
            if (ignore)
                builder.Append('\\');
            target.Inlines.Add(new Run()
            {
                Text = builder.ToString()
            });
        }

        private static void ReadUri(StringReader reader, Span target)
        {
            var text = new StringBuilder();
            var uri = new StringBuilder();
            var writer = text;
            bool ignore = false;
            bool uriMode = false;
            char current;
            while ((current = reader.ReadChar()) != '\0')
            {
                if (current == '\n' || current == '\r')
                {
                    writer.Append(current);
                    break;
                }
                switch (current)
                {
                    case '\\':
                        ignore = true;
                        break;
                    case ']':
                        if (ignore || uriMode)
                        {
                            writer.Append(']');
                            ignore = false;
                        }
                        else
                        {
                            char next = reader.PeekChar();
                            if (next == '(')
                            {
                                uriMode = true;
                                reader.ReadChar();
                                writer = uri;
                            }
                            else
                                writer.Append(']');
                        }
                        break;
                    case ')':
                        if (ignore || !uriMode)
                        {
                            ignore = false;
                            writer.Append(')');
                        }
                        else
                        {
                            var hyperlink = new Hyperlink();
                            hyperlink.Inlines.Add(new Run() { Text = text.ToString() });
                            var uriText = uri.ToString();
                            if (uriText.StartsWith("*"))
                            {
                                uriText = uriText.Substring(1);
                                AppSelectorBehavior.SetSelectMode(hyperlink, true);
                            }
                            AppSelectorBehavior.SetDestUri(hyperlink, new Uri(uriText));
                            target.Inlines.Add(hyperlink);
                            return;
                        }
                        break;
                    default:
                        if (ignore)
                        {
                            ignore = false;
                            writer.Append('\\');
                        }
                        writer.Append(current);
                        break;
                }
            }
            text.Insert(0, '[');
            if (uriMode)
            {
                text.Append(']');
                text.Append('(');
                text.Append(uri.ToString());
            }
            if (ignore)
                text.Append('\\');
            target.Inlines.Add(new Run()
            {
                Text = text.ToString()
            });
        }
    }
}
