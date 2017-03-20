using System.Collections.Generic;
using twitter_rollup.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace twitter_rollup
{
    static class TweetFormatter
    {
        public static string ToHtml(Tweet tweet)
        {
            var htmlStrings = new List<string> { "<p style='margin: 0'>", $"<a href='https://mobile.twitter.com/{tweet.user.screen_name}/status/{tweet.id_str}'>(#)</a>" };
            var name = tweet.user.name;
            var isRetweet = tweet.is_retweet;
            var entities = tweet.is_retweet ? tweet.retweeted_status.entities : tweet.entities;
            var text = tweet.is_retweet ? tweet.retweeted_status.full_text : tweet.full_text;

            foreach (var url in entities.urls)
            {
                text = text.Replace(url.url, url.expanded_url);
            }

            foreach (var mention in entities.user_mentions)
            {
                text = text.Replace(mention.screen_name, $"<a href=\"https://mobile.twitter.com/{mention.screen_name}\">{mention.screen_name}</a>");
            }

            foreach (var media in entities.media)
            {
                if (media.type == "photo")
                {
                    text = text.Replace(media.url, "");
                }
            }

            if (isRetweet)
            {
                htmlStrings.Add($"<strong>{name}</strong> retweeted <strong><a href='https://mobile.twitter.com/{tweet.retweeted_status.user.screen_name}'>{tweet.retweeted_status.user.screen_name}</a></strong>:");
            }
            else if (tweet.is_quote_status)
            {
                htmlStrings.Add($"<strong>{name}</strong> quoted <strong><a href='https://mobile.twitter.com/{tweet.quoted_status.user.screen_name}'>{tweet.quoted_status.user.screen_name}</a></strong>:");
                text += $"<blockquote>{tweet.quoted_status.full_text}</blockquote>";
            }
            else 
            {
                htmlStrings.Add($"<strong>{name}</strong>:");
            }

            htmlStrings.AddRange(new string[] {
                "</p>",
                "<p style='margin:0; margin-top:10px;'>",
                new Regex("\n", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(text, "<br/>"),
                "</p>"
            });

            foreach (var media in entities.media.Where(m => m.type == "photo"))
            {
                htmlStrings.Add($"<div style='padding-top: 10px'><a href='{media.url}'><img src='{media.media_url_https}' style='max-width: 100%; max-height:400px' /></a></div>");
            }

            return $"<div style='padding: 10px 0; border-bottom: 1px solid #ccc;'>{string.Join("", htmlStrings)}</div>";
        }
    }
}