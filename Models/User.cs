namespace twitter_rollup.Models
{
    class User
    {
        public long? id { get; set; }

        public string name { get; set; }

        public string screen_name { get; set; }

        public string location { get; set; }

        public string description { get; set; }

        public string url { get; set; }
    }
}