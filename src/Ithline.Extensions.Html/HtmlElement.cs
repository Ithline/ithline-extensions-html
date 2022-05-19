using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Ithline.Extensions.Html
{
    /// <summary>
    /// Represents a HTML element.
    /// </summary>
    [DebuggerDisplay("<{DebuggerDisplayString,nq}>")]
    public sealed class HtmlElement : HtmlNode
    {
        internal HtmlElement(string elementName, IEnumerable<HtmlAttribute> attributes, IEnumerable<HtmlNode> children)
        {
            if (string.IsNullOrWhiteSpace(elementName))
            {
                throw new ArgumentException($"'{nameof(elementName)}' cannot be null or whitespace.", nameof(elementName));
            }

            if (attributes is null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            if (children is null)
            {
                throw new ArgumentNullException(nameof(children));
            }

            ElementName = elementName;
            Attributes = HtmlAttributeList.CreateRange(attributes);
            Children = HtmlNodeList.CreateRange(children);
        }

        /// <summary>
        /// Gets the element name.
        /// </summary>
        public string ElementName { get; }
        /// <summary>
        /// Gets the element attributes.
        /// </summary>
        public HtmlAttributeList Attributes { get; }
        /// <summary>
        /// Gets the element children.
        /// </summary>
        public HtmlNodeList Children { get; }

        private string DebuggerDisplayString => IsVoidElement(ElementName)
            ? $"<{ElementName}>"
            : $"<{ElementName}></{ElementName}>";

        /// <inheritdoc/>
        public override void WriteTo(TextWriter output)
        {
            output.Write('<');
            output.Write(ElementName);
            foreach (var attribute in Attributes)
            {
                output.Write(' ');
                output.Write(attribute.Name);
                if (attribute.Value is not null)
                {
                    output.Write('=');
                    output.Write('"');
                    output.Write(attribute.Value);
                    output.Write('"');
                }
            }

            if (IsVoidElement(ElementName))
            {
                output.Write("/>");
            }
            else if (ElementName[0] == '?')
            {
                output.Write("?>");
            }
            else if (ElementName[0] is '!')
            {
                output.Write('>');
            }
            else
            {
                output.Write('>');
                foreach (var child in Children)
                {
                    child.WriteTo(output);
                }
                output.Write("</");
                output.Write(ElementName);
                output.Write('>');
            }
        }

        internal static bool IsVoidElement(ReadOnlySpan<char> elementName)
        {
            return elementName.Equals("input".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("img".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("br".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("hr".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("source".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("meta".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("link".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("base".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("area".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("col".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("command".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("embed".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("param".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("track".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("keygen".AsSpan(), StringComparison.OrdinalIgnoreCase)
                || elementName.Equals("wbr".AsSpan(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
