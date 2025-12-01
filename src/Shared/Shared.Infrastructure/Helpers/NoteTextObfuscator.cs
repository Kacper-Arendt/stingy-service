using System.Text;
using System.Text.RegularExpressions;

namespace Shared.Infrastructure.Helpers;

using HtmlAgilityPack;

public static partial class NoteTextObfuscator
{
    private static readonly Random Random = new();

    public static string Obfuscate(string html, string imageSrcReplacement)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        ObfuscateNode(doc.DocumentNode, imageSrcReplacement);

        return doc.DocumentNode.InnerHtml;
    }

    private static void ObfuscateNode(HtmlNode node, string imageSrcReplacement)
    {
        if (node.NodeType == HtmlNodeType.Text)
            node.InnerHtml = ObfuscateText(node.InnerHtml);
        else if (node.Name == "img")
            node.SetAttributeValue("src", imageSrcReplacement);
        else
        {
            foreach (var child in node.ChildNodes)
                ObfuscateNode(child, imageSrcReplacement);
        }
    }

    private static string ObfuscateText(string text)
    {
        return MyRegex().Replace(text, match =>
        {
            var word = match.Value;
            var sb = new StringBuilder(word.Length);
            foreach (var randomChar in word.Select(c => (char)Random.Next(
                         char.IsUpper(c) ? 'A' : 'a',
                         char.IsUpper(c) ? 'Z' + 1 : 'z' + 1
                     )))
            {
                sb.Append(randomChar);
            }

            return sb.ToString();
        });
    }

    [GeneratedRegex(@"\w+")]
    private static partial Regex MyRegex();
}