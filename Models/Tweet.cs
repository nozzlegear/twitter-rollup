namespace twitter_rollup.Models
{
    class Tweet
    {
        public long id { get; set; }

        public string id_str { get; set; }

        public string full_text { get; set; }

        public string in_reply_to_screen_name { get; set; }

        private long? _in_reply_to_status_id { get; set; }

        public long? in_reply_to_status_id 
        { 
            get 
            {
                return _in_reply_to_status_id;
            } 
            set 
            {
                _in_reply_to_status_id = value;
                is_reply_status = value.HasValue;
            } 
        }

        public string in_reply_to_status_id_str { get; set; }

        public long? in_reply_to_user_id { get; set; }

        public long? in_reply_to_user_id_str { get; set; }

        public bool is_reply_status { get; private set; }

        public bool is_quote_status { get; private set; }

        public Tweet retweeted_status { get; set; }

        public Tweet quoted_status { get; set; }

        private long? _quoted_status_id { get; set; }

        public long? quoted_status_id 
        { 
            get
            {
                return _quoted_status_id;
            }
            set
            {
                _quoted_status_id = value;
                is_quote_status = value.HasValue;
            }
        }

        public string quoted_status_id_str { get; set; }

        public Entities entities { get; set; }

        public User user { get; set; }
    }
}