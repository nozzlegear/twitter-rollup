using System.Collections.Generic;

namespace twitter_rollup.Models
{
    class UserMention
    {
        public string screen_name { get; set; }

        public string name { get; set; }

        public long? id { get; set; }

        public string id_str { get; set; }

        public IEnumerable<int> indices { get; set; }
    }
}