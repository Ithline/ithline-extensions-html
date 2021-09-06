using System;
using System.Diagnostics;
using System.IO;

namespace Ithline.Extensions.Html
{
    /// <summary>
    /// Represents a HTML text.
    /// </summary>
    [DebuggerDisplay("{Text}")]
    public sealed class HtmlText : HtmlNode
    {
        internal HtmlText(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the value of the text node.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc/>
        public override void WriteTo(TextWriter output)
        {
            output.Write(Value);
        }
    }
}
