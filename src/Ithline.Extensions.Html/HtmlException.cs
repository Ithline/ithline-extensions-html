using System;

namespace Ithline.Extensions.Html
{
    /// <summary>
    /// Represents a HTML parsing exception.
    /// </summary>
    public sealed class HtmlException : Exception
    {
        private const string DefaultMessage = "An error has occurrent during parsing of the HTML string.";

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlException"/> class with the specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HtmlException(string? message) : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlException"/> class with the specified error message and inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a <see langword="null"/> reference if not inner exception is specified.</param>
        public HtmlException(string? message, Exception? innerException)
            : base(message ?? DefaultMessage, innerException)
        {
        }
    }
}
