using System.Collections.Generic;
using twitter_rollup.Models;

namespace twitter_rollup
{
    static class TweetFormatter
    {
        public static string ToHtml(Tweet tweet)
        {
            var strings = new List<string>();

            return string.Join("\n", strings);
        }
    }
}