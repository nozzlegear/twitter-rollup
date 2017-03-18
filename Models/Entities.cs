using System.Collections.Generic;

namespace twitter_rollup.Models
{
    class Entities
    {
        public IEnumerable<string> hashtags { get; set; }

        public IEnumerable<string> symbols { get; set; }

        public IEnumerable<UserMention> user_mentions { get; set; }

        public IEnumerable<TwitterUrl> urls { get; set; }

        public IEnumerable<Media> media { get; set; }
    }
}