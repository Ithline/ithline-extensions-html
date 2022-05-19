using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ithline.Extensions.Html
{
    internal static class HtmlNodeParser
    {
        public static HtmlNode? Parse(ReadOnlySpan<char> span)
        {
            if (span.IsEmpty)
            {
                return null;
            }

            HtmlNode? node;
            var list = HtmlNodeList.Empty;
            while ((node = ParseNode(ref span)) is not null)
            {
                list = list.Add(node);
            }

            return list.Count switch
            {
                0 => null,
                1 => list[0],
                _ => list
            };
        }

        private static HtmlNode? ParseNode(ref ReadOnlySpan<char> span)
        {
            if (span.IsEmpty)
            {
                return null;
            }

            // try to parse HTML text
            if (TryParseText(ref span, out var htmlText))
            {
                return htmlText;
            }

            // try to parse HTML comment
            if (TryParseComment(ref span, out var htmlComment))
            {
                return htmlComment;
            }

            // try to parse HTML element
            if (TryParseElement(ref span, out var htmlElement))
            {
                return htmlElement;
            }

            return null;
        }

        private static bool TryParseText(ref ReadOnlySpan<char> span, [NotNullWhen(true)] out HtmlText? htmlText)
        {
            var elementStart = span.IndexOf('<');

            // if there is no '<' character and span contains any non-whitespace character,
            // we consider the value to be text
            if (elementStart < 0 && !span.IsWhiteSpace())
            {
                htmlText = HtmlNode.Text(span.ToString());
                span = ReadOnlySpan<char>.Empty;
                return true;
            }

            // if we've found '<' character at any other than the first place,
            // we try to extract all non-whitespace characters and return them as HTML text node
            if (elementStart > 0)
            {
                var textSlice = span.Slice(0, elementStart);
                span = span.Slice(elementStart);
                if (!textSlice.IsWhiteSpace())
                {
                    htmlText = HtmlNode.Text(textSlice.ToString());
                    return true;
                }
            }

            htmlText = null;
            return false;
        }

        private static bool TryParseComment(ref ReadOnlySpan<char> span, [NotNullWhen(true)] out HtmlComment? htmlComment)
        {
            // if the span starts with <!-- sequence, we've found the beggining of a comment node
            if (span.StartsWith("<!--", StringComparison.Ordinal))
            {
                span = span.Slice(4);
                var end = span.IndexOf("-->", StringComparison.Ordinal);
                if (end < 0)
                {
                    throw new HtmlException("No closing tag found for HTML comment.");
                }

                // create a comment node with the and remove closing tag from the input
                htmlComment = HtmlNode.Comment(span.Slice(0, end).ToString());
                span = span.Slice(end + 3);
                return true;
            }

            htmlComment = null;
            return false;
        }

        private static bool TryParseElement(ref ReadOnlySpan<char> span, [NotNullWhen(true)] out HtmlElement? htmlElement)
        {
            if (span.IsEmpty || span[0] is not '<')
            {
                htmlElement = null;
                return false;
            }
            span = EnsureNotEmpty(span.Slice(1));

            if (!TryGetElementName(ref span, out var elementName))
            {
                throw new HtmlException("HTML element must have valid non-empty name.");
            }

            var attributes = ParseAttributeList(ref span);
            span = EnsureNotEmpty(span);

            // void element has no children and ends either with "/>" or ">"
            if (HtmlElement.IsVoidElement(elementName))
            {
                if (span[0] is '/')
                {
                    span = EnsureNotEmpty(span.Slice(1));
                }

                if (span[0] is '>')
                {
                    span = span.Slice(1);
                    htmlElement = HtmlNode.Element(elementName.ToString(), attributes);
                    return true;
                }

                throw new HtmlException("HTML element is not properly closed.");
            }

            // the element is not closed
            if (span[0] is not '>')
            {
                throw new HtmlException("HTML element is not properly closed.");
            }
            span = EnsureNotEmpty(span.Slice(1), trim: false);

            var children = HtmlNodeList.Empty;
            while (!span.StartsWith("</", StringComparison.OrdinalIgnoreCase))
            {
                if (ParseNode(ref span) is HtmlNode child)
                {
                    children = children.Add(child);
                }
                span = span.TrimStart();
            }

            span = EnsureNotEmpty(span.Slice(2));
            if (!span.StartsWith(elementName, StringComparison.OrdinalIgnoreCase))
            {
                throw new HtmlException("Invalid closing tag encountered.");
            }

            span = EnsureNotEmpty(span.Slice(elementName.Length));
            if (span[0] is not '>')
            {
                throw new HtmlException("Closing tag is not properly closed.");
            }
            span = span.Slice(1);

            htmlElement = HtmlNode.Element(elementName.ToString(), attributes, children);
            return true;

            static bool TryGetElementName(ref ReadOnlySpan<char> span, out ReadOnlySpan<char> elementName)
            {
                elementName = ReadOnlySpan<char>.Empty;
                for (var i = 0; i < span.Length; i++)
                {
                    if (!IsAlphanumeric(span[i]))
                    {
                        elementName = span.Slice(0, i);
                        span = span.Slice(i);
                        break;
                    }
                }
                return !elementName.IsEmpty;
            }
        }

        private static HtmlAttributeList ParseAttributeList(ref ReadOnlySpan<char> span)
        {
            var attributes = HtmlAttributeList.Empty;
            while (span.Length > 0)
            {
                // if we found closing sequence of attribute list, we return current value
                span = EnsureNotEmpty(span);
                if (span[0] is '>' or '/')
                {
                    return attributes;
                }

                if (!IsAlphanumeric(span[0]))
                {
                    throw new HtmlException($"HTML tag cannot contain '{span[0]}' character.");
                }

                var attributeName = ReadOnlySpan<char>.Empty;
                for (var i = 0; i < span.Length; i++)
                {
                    if (char.IsWhiteSpace(span[i]) || span[i] is '=' or '/' or '>' or '\'' or '"')
                    {
                        // odložíme si názov a osekáme "hodnotu"
                        attributeName = span.Slice(0, i);
                        span = span.Slice(i);
                        break;
                    }
                }

                if (attributeName.IsEmpty)
                {
                    throw new HtmlException("HTML attribute name cannot be empty.");
                }
                span = EnsureNotEmpty(span);

                // first character after attribute name cannot be single or double quote
                if (span[0] is '\'' or '"')
                {
                    throw new HtmlException("HTML attribute name cannot be followed by single or double quotes.");
                }

                // we try to parse attribute value
                string? attributeValue = null;
                if (span[0] is '=')
                {
                    span = EnsureNotEmpty(span.Slice(1));
                    if (span[0] is '\'' or '"')
                    {
                        var attributeQuote = span[0];
                        span = span.Slice(1);
                        var index = span.IndexOf(attributeQuote);
                        if (index < 0)
                        {
                            throw new HtmlException("HTML attribute value is not closed.");
                        }
                        if (index > 0)
                        {
                            attributeValue = span.Slice(0, index).ToString();
                        }
                        span = span.Slice(index + 1);
                    }
                    else
                    {
                        var valueSpan = ReadOnlySpan<char>.Empty;
                        for (var i = 0; i < span.Length; i++)
                        {
                            if (char.IsWhiteSpace(span[i]) || span[i] is '=' or '/' or '>' or '\'' or '"' or '<' or '`')
                            {
                                valueSpan = span.Slice(0, i);
                                break;
                            }
                        }

                        if (valueSpan.Length == 0)
                        {
                            throw new HtmlException("HTML attribute has no value.");
                        }
                        attributeValue = valueSpan.ToString();
                        span = span.Slice(valueSpan.Length);
                    }
                }
                attributes = attributes.Set(HtmlNode.Attribute(attributeName.ToString(), attributeValue));
            }
            return attributes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAlphanumeric(char ch) => ch is (>= '0' and <= '9') or (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<char> EnsureNotEmpty(ReadOnlySpan<char> span, bool trim = true)
        {
            if (!span.IsEmpty && char.IsWhiteSpace(span[0]) && trim)
            {
                span = span.TrimStart();
            }

            if (span.IsEmpty)
            {
                throw new HtmlException("Unexpected EOF.");
            }
            return span;
        }
    }
}
