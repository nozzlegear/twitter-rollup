using System.Collections.Generic;

namespace twitter_rollup.Models
{
    class Media
    {
        public long? id { get; set; }

        public long? id_str { get; set; }

        public IEnumerable<int> indices { get; set; }

        public string media_url { get; set; }

        public string media_url_https { get; set; }

        public string url { get; set; }

        public string display_url { get; set; }

        public string expanded_url { get; set; }

        public string type { get; set; }

        public Sizes sizes { get; set; }

        public long? source_status_id { get; set; }

        public string source_status_id_str { get; set; }

        public long? source_user_id { get; set; }

        public string source_user_id_str { get; set; }
    }
}