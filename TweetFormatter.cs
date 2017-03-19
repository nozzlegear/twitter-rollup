using System.Collections.Generic;
using twitter_rollup.Models;
using System.Linq;

namespace twitter_rollup
{
    static class TweetFormatter
    {
        public static string ToHtml(Tweet tweet)
        {
            var htmlStrings = new List<string>();
            var name = tweet.user.name;
            var isRetweet = tweet.is_retweet;
            var entities = tweet.is_retweet ? tweet.retweeted_status.entities : tweet.entities;
            var text = string.Empty;


            return string.Join("\n", htmlStrings);
        }
    }
}