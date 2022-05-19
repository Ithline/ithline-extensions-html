using System;
using System.Diagnostics;
using System.IO;

namespace Ithline.Extensions.Html
{
    /// <summary>
    /// Represents a HTML comment.
    /// </summary>
    [DebuggerDisplay("<!--{Value,nq}-->")]
    public sealed class HtmlComment : HtmlNode
    {
        internal HtmlComment(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the value of the comment.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc/>
        public override void WriteTo(TextWriter output)
        {
            output.Write("<!--");
            output.Write(Value);
            output.Write("-->");
        }
    }
}
