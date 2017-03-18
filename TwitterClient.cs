using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using twitter_rollup.Models;

namespace twitter_rollup
{
    class TwitterClient
    {
        readonly string BaseUrl = "https://api.twitter.com";

        public TwitterClient(string token)
        {
            Token = token;
        }

        string Token { get; set; }

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

                response.EnsureSuccessStatusCode();

                var body = await request.ReceiveJson<IEnumerable<Tweet>>();

                return body.Reverse();
            }
        }
    }
}