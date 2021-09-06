using FluentAssertions;
using Xunit;

namespace Ithline.Extensions.Html
{
    public sealed class HtmlNodeTest
    {
        [Theory]
        [InlineData("<p></p>")]
        [InlineData("<p>abcd</p>")]
        [InlineData("<p class=white></p>")]
        [InlineData("<p class='white'></p>")]
        [InlineData("<p class=\"white\"></p>")]
        [InlineData("<p><a name=\"_dx_frag_EndFragment\"></a><span>&nbsp;</span></p>")]
        [InlineData("<ul><li><span>Nastaviteľn&yacute; kľ&uacute;č.</span></li><li><span>V&yacute;borne sa hod&iacute; k uchopovaniu, držaniu, stlačovaniu a oh&yacute;baniu obrobkov.</span></li><li><span>Nahr&aacute;dza sadu kľ&uacute;čov na skrutky, metrick&eacute; i palcov&eacute;.</span></li><li><span>Chromvanadiov&aacute; elektrooceľ, kovan&aacute;, kalen&aacute; v oleji.</span></li></ul>")]
        [InlineData("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"636\" style=\"border-collapse:collapse;\"><tr style=\"height:15.75pt;\"><td style=\"padding:0.75pt 0.75pt 0pt 6.75pt;background-color:#D9D9D9;border-right:1pt #FFFFFF solid;border-bottom:1pt #FFFFFF solid;border-left:1pt #FFFFFF solid;\"><p style=\"background-color:#D9D9D9;\"><span style=\"background-color:#D9D9D9;\">čist&aacute; hmotnosť</span></p></td><td style=\"padding:0.75pt 0.75pt 0pt 6.75pt;background-color:#A6A6A6;border-right:1pt #FFFFFF solid;border-bottom:1pt #FFFFFF solid;border-left:none;\"><p style=\"background-color:#A6A6A6;\"><span style=\"background-color:#A6A6A6;font-weight:bold;\">729 g</span></p></td></tr></table>")]
        [InlineData("<body>\r\n\t<p style=\"text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;\"><span style=\"color:#000000;font-size:18pt;font-weight:bold;\">Nadpis</span></p><p style=\"text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;\"><span style=\"color:#000000;\">trochu textu, aby to nebolo prazdne</span></p><ul style=\"margin-top:0;margin-bottom:0;\">\r\n\t\t<li style=\"text-align:left;margin:0pt 0pt 0pt 0pt;list-style-type:disc;color:#000000;\"><span style=\"color:#000000;background-color:transparent;font-family:Calibri;font-size:11pt;font-weight:normal;font-style:normal;\">nejaka</span></li><li style=\"text-align:left;margin:0pt 0pt 0pt 0pt;list-style-type:disc;color:#000000;\"><span style=\"color:#000000;background-color:transparent;font-family:Calibri;font-size:11pt;font-weight:normal;font-style:normal;\">ta</span></li><li style=\"text-align:left;margin:0pt 0pt 0pt 0pt;list-style-type:disc;color:#000000;\"><span style=\"color:#000000;background-color:transparent;font-family:Calibri;font-size:11pt;font-weight:normal;font-style:normal;\">odrazka</span></li></ul>\r\n\t<p style=\"text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;\"><span style=\"color:#000000;\">&nbsp;</span></p><p style=\"text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;\"><span style=\"color:#000000;\">a na koniec zase trochu textu.</span></p></body>\r\n")]
        public void ParsingValidInput_ReturnsHtmlNode(string rawHtml)
        {
            // act
            var nodes = HtmlNode.Parse(rawHtml);

            // assert
            nodes.Should().NotBeNull();
        }

        [Theory]
        [InlineData("<")]
        [InlineData("<p")]
        [InlineData("<p>")]
        [InlineData("<p=>")]
        [InlineData("<p/>")]
        [InlineData("<p></x>")]
        [InlineData("<p-></p>")]
        [InlineData("<p:></p>")]
        [InlineData("<p=tag></p>")]
        [InlineData("<img></img>")]
        [InlineData("<p class=></p>")]
        [InlineData("<p class= ></p>")]
        [InlineData("<p class\"></p>")]
        [InlineData("<p class'></p>")]
        public void ParsingInvalidInput_ThrowsHtmlException(string rawHtml)
        {
            // act
            var action = FluentActions.Invoking(() => HtmlNode.Parse(rawHtml));

            // assert
            action.Should().Throw<HtmlException>();
        }


        [Fact]
        public void Parse_WhenContainsBody_ReturnsDivElement()
        {
            // arrange
            var rawHtml = "<body>\r\n\t<p style=\"text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;\"><span style=\"color:#000000;font-size:18pt;font-weight:bold;\">Nadpis</span></p><p style=\"text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;\"><span style=\"color:#000000;\">trochu textu, aby to nebolo prazdne</span></p><ul style=\"margin-top:0;margin-bottom:0;\">\r\n\t\t<li style=\"text-align:left;margin:0pt 0pt 0pt 0pt;list-style-type:disc;color:#000000;\"><span style=\"color:#000000;background-color:transparent;font-family:Calibri;font-size:11pt;font-weight:normal;font-style:normal;\">nejaka</span></li><li style=\"text-align:left;margin:0pt 0pt 0pt 0pt;list-style-type:disc;color:#000000;\"><span style=\"color:#000000;background-color:transparent;font-family:Calibri;font-size:11pt;font-weight:normal;font-style:normal;\">ta</span></li><li style=\"text-align:left;margin:0pt 0pt 0pt 0pt;list-style-type:disc;color:#000000;\"><span style=\"color:#000000;background-color:transparent;font-family:Calibri;font-size:11pt;font-weight:normal;font-style:normal;\">odrazka</span></li></ul>\r\n\t<p style=\"text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;\"><span style=\"color:#000000;\">&nbsp;</span></p><p style=\"text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;\"><span style=\"color:#000000;\">a na koniec zase trochu textu.</span></p></body>\r\n";

            // act
            var body = HtmlNode.Parse(rawHtml);

            // assert
            var expectation = HtmlNode.Element("body", children: new[]
            {
                HtmlNode.Element("p", HtmlAttributeList.Create("style", "text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;"), new[]
                {
                    HtmlNode.Element("span", HtmlAttributeList.Create("style", "color:#000000;font-size:18pt;font-weight:bold;"), new[]
                    {
                        HtmlNode.Text("Nadpis"),
                    }),
                }),
                HtmlNode.Element("p", HtmlAttributeList.Create("style", "text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;"), new[]
                {
                    HtmlNode.Element("span", HtmlAttributeList.Create("style", "color:#000000;"), new[]
                    {
                        HtmlNode.Text("trochu textu, aby to nebolo prazdne"),
                    }),
                }),
                HtmlNode.Element("ul", HtmlAttributeList.Create("style", "margin-top:0;margin-bottom:0;"), new[]
                {
                    HtmlNode.Element("li", HtmlAttributeList.Create("style", "text-align:left;margin:0pt 0pt 0pt 0pt;list-style-type:disc;color:#000000;"), new[]
                    {
                        HtmlNode.Element("span", HtmlAttributeList.Create("style", "color:#000000;background-color:transparent;font-family:Calibri;font-size:11pt;font-weight:normal;font-style:normal;"), new[]
                        {
                            HtmlNode.Text("nejaka"),
                        }),
                    }),
                    HtmlNode.Element("li", HtmlAttributeList.Create("style", "text-align:left;margin:0pt 0pt 0pt 0pt;list-style-type:disc;color:#000000;"), new[]
                    {
                        HtmlNode.Element("span", HtmlAttributeList.Create("style", "color:#000000;background-color:transparent;font-family:Calibri;font-size:11pt;font-weight:normal;font-style:normal;"), new[]
                        {
                            HtmlNode.Text("ta"),
                        }),
                    }),
                    HtmlNode.Element("li", HtmlAttributeList.Create("style", "text-align:left;margin:0pt 0pt 0pt 0pt;list-style-type:disc;color:#000000;"), new[]
                    {
                        HtmlNode.Element("span", HtmlAttributeList.Create("style", "color:#000000;background-color:transparent;font-family:Calibri;font-size:11pt;font-weight:normal;font-style:normal;"), new[]
                        {
                            HtmlNode.Text("odrazka"),
                        }),
                    }),
                }),
                HtmlNode.Element("p", HtmlAttributeList.Create("style", "text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;"), new[]
                {
                    HtmlNode.Element("span", HtmlAttributeList.Create("style", "color:#000000;"), new[]
                    {
                        HtmlNode.Text("&nbsp;"),
                    }),
                }),
                HtmlNode.Element("p", HtmlAttributeList.Create("style", "text-align:left;text-indent:0pt;margin:0pt 0pt 0pt 0pt;"), new[]
                {
                    HtmlNode.Element("span", HtmlAttributeList.Create("style", "color:#000000;"), new[]
                    {
                        HtmlNode.Text("a na koniec zase trochu textu."),
                    }),
                }),
            });
            body.Should().BeEquivalentTo(expectation);
        }
    }
}
