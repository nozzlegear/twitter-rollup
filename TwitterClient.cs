using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using twitter_rollup.Models;
using System.Text;

namespace twitter_rollup
{
    class TwitterClient
    {
        readonly string BaseUrl = "https://api.twitter.com/1.1";

        public TwitterClient(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);

            Token = token;
            Base64Token = Convert.ToBase64String(bytes);
        }

        string Token { get; set; }

        string Base64Token { get; set; }

        IFlurlClient PrepareRequest(string path)
        {
            return Flurl.Url.Combine(BaseUrl, path).AllowAnyHttpStatus().WithOAuthBearerToken(Token);
        }

        public async Task<IEnumerable<Tweet>> GetTweetsForUser(string username, bool withReplies, long? sinceId = null)
        {
            using (var api = PrepareRequest("statuses/user_timeline"))
            {
                api.Url.SetQueryParams(new
                {
                    screen_name = username, 
                    since_id = sinceId, 
                    exclude_replies = true, 
                    tweet_mode = "extended" 
                });

                var request = api.GetAsync();
                var response = await request;

#if DEBUG

                Console.WriteLine(api.Url.ToString());
                Console.WriteLine("With token {0}", Token);

                var s = await request.ReceiveString();

                Console.WriteLine("Response string: {0}", s);

#endif

                response.EnsureSuccessStatusCode();

                var body = await request.ReceiveJson<IEnumerable<Tweet>>();

                return body.Reverse();
            }
        }
    }
}