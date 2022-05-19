using System;
using System.Collections.Generic;
using System.IO;

namespace Ithline.Extensions.Html
{
    /// <summary>
    /// Represents a HTML base node.
    /// </summary>
    public abstract class HtmlNode
    {
        private protected HtmlNode()
        {
        }

        /// <summary>
        /// Returns a new <see cref="HtmlAttribute"/> with the specified name and value.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="value">Attribute value.</param>
        /// <returns>A new <see cref="HtmlAttribute"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or empty.</exception>
        public static HtmlAttribute Attribute(string name, string? value = null)
        {
            return new HtmlAttribute(name, value);
        }

        /// <summary>
        /// Returns a new <see cref="HtmlComment"/> with the specified value.
        /// </summary>
        /// <param name="value">Value of the comment.</param>
        /// <returns>A new <see cref="HtmlComment"/>.</returns>
        public static HtmlComment Comment(string value)
        {
            return new HtmlComment(value);
        }

        /// <summary>
        /// Returns a new <see cref="HtmlElement"/> with the specified name, attributes and children.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="attributes">Element attributes.</param>
        /// <param name="children">Element children.</param>
        /// <returns>A new <see cref="HtmlElement"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="elementName"/> is <see langword="null"/> or empty.</exception>
        public static HtmlElement Element(string elementName, IEnumerable<HtmlAttribute>? attributes = null, IEnumerable<HtmlNode>? children = null)
        {
            return new HtmlElement(elementName, attributes ?? HtmlAttributeList.Empty, children ?? HtmlNodeList.Empty);
        }

        /// <summary>
        /// Returns a new <see cref="HtmlText"/> with the specified value.
        /// </summary>
        /// <param name="value">Value of the text.</param>
        /// <returns>A new <see cref="HtmlText"/>.</returns>
        public static HtmlText Text(string value)
        {
            return new HtmlText(value);
        }

        /// <summary>
        /// Returns parsed HTML node tree from the specified input value.
        /// </summary>
        /// <param name="span">Input string.</param>
        /// <returns>A <see cref="HtmlNode"/> tree.</returns>
        /// <exception cref="HtmlException">Invalid HTML format.</exception>
        public static HtmlNode? Parse(ReadOnlySpan<char> span) => HtmlNodeParser.Parse(span);

        /// <summary>
        /// Writes the content of the element into the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="output">Text writer target.</param>
        public abstract void WriteTo(TextWriter output);

        /// <inheritdoc/>
        public override string ToString()
        {
            using var sw = new StringWriter();
            this.WriteTo(sw);
            return sw.ToString();
        }
    }
}
