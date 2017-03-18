using System.Collections.Generic;

namespace twitter_rollup.Models
{
    class TwitterUrl
    {
        public string url { get; set; }

        public string expanded_url { get; set; }

        public string display_url { get; set; }

        public IEnumerable<int> indices { get; set; }
    }
}