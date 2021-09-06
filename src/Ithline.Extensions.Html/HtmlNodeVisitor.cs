using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Ithline.Extensions.Html
{
    /// <summary>
    /// Represents a base class for <see cref="HtmlNode"/> visitor.
    /// </summary>
    public abstract class HtmlNodeVisitor
    {
        protected HtmlNodeVisitor()
        {
        }

        /// <summary>
        /// Dispatches the node to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">The node to visit.</param>
        /// <returns>The modified node, if it or any subnodes was modified; otherwise, returns the original node.</returns>
        [return: NotNullIfNotNull("node")]
        public virtual HtmlNode? Visit(HtmlNode? node)
        {
            return node switch
            {
                HtmlComment comment => this.VisitComment(comment),
                HtmlElement element => this.VisitElement(element),
                HtmlNodeList list => this.VisitNodeList(list),
                HtmlText text => this.VisitText(text),
                _ => node,
            };
        }

        /// <summary>
        /// Visits the <see cref="HtmlComment"/> node.
        /// </summary>
        /// <param name="node">The node to visit.</param>
        /// <returns>The modified node, if it or any subnodes was modified; otherwise, returns the original node.</returns>
        protected virtual HtmlNode VisitComment(HtmlComment node) => node;

        /// <summary>
        /// Visits the <see cref="HtmlElement"/> node.
        /// </summary>
        /// <param name="node">The node to visit.</param>
        /// <returns>The modified node, if it or any subnodes was modified; otherwise, returns the original node.</returns>
        protected virtual HtmlNode VisitElement(HtmlElement node)
        {
            var children = this.VisitNodeList(node.Children);
            var attributes = this.VisitAttributeList(node.Attributes);
            if (attributes != node.Attributes || children != node.Children)
            {
                return HtmlNode.Element(node.ElementName, attributes, children);
            }
            return node;
        }

        /// <summary>
        /// Visits the <see cref="HtmlText"/> node.
        /// </summary>
        /// <param name="text">The node to visit.</param>
        /// <returns>The modified node, if it or any subnodes was modified; otherwise, returns the original node.</returns>
        protected virtual HtmlText VisitText(HtmlText text) => text;

        /// <summary>
        /// Visits the <see cref="HtmlAttribute"/> node.
        /// </summary>
        /// <param name="node">The node to visit.</param>
        /// <returns>The modified node, if it or any subnodes was modified; otherwise, returns the original node.</returns>
        protected virtual HtmlAttribute? VisitAttribute(HtmlAttribute node) => node;

        /// <summary>
        /// Visits the <see cref="HtmlAttributeList"/> node.
        /// </summary>
        /// <param name="nodes">The nodes to visit.</param>
        /// <returns>The modified node, if it or any subnodes was modified; otherwise, returns the original node.</returns>
        protected HtmlAttributeList VisitAttributeList(HtmlAttributeList nodes)
        {
            var changed = false;
            var builder = new List<HtmlAttribute?>(nodes.Count);
            foreach (var attribute in nodes)
            {
                var visited = this.VisitAttribute(attribute);
                changed |= visited == attribute;
                builder.Add(visited);
            }
            return changed ? HtmlAttributeList.CreateRange(builder) : nodes;
        }

        /// <summary>
        /// Visits the <see cref="HtmlNodeList"/> node.
        /// </summary>
        /// <param name="nodes">The node to visit.</param>
        /// <returns>The modified node, if it or any subnodes was modified; otherwise, returns the original node.</returns>
        protected HtmlNodeList VisitNodeList(HtmlNodeList nodes)
        {
            var builder = new List<HtmlNode?>();
            for (var i = 0; i < nodes.Count; i++)
            {
                var visited = this.Visit(nodes[i]);
                if (visited != nodes[i] && builder is null)
                {
                    builder = new List<HtmlNode?>(nodes.Count);
                    for (var j = 0; j < i; j++)
                    {
                        builder.Add(nodes[j]);
                    }
                }
                builder?.Add(visited);
            }
            return builder is null ? nodes : HtmlNodeList.CreateRange(builder);
        }
    }
}
