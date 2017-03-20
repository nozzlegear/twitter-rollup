using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.CommandLineUtils;
using twitter_rollup.Models;

namespace twitter_rollup
{
    class Program
    {
        public static string GetAppDirectory()
        {
            return Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        }

        public static string GetFilePath(string filename = "test.html")
        {
            return Path.Combine(GetAppDirectory(), filename);
        }

        /// <summary>
        /// Takes a username and returns possible variants of it, e.g. username, @username
        /// </summary>
        static IEnumerable<string> GetUsernameVariants(string username)
        {
            return new List<string> { username, username.Replace("@", ""), "@" + username };
        }

        /// <summary>
        /// Checks if a list of usernames contains any variant of one single username.    
        /// </summary>
        static bool HasMatch(IEnumerable<string> usernames, string username)
        {
            // Not sure if usernames will have an @, or if the username will have an @, so check for both.
            var variants = GetUsernameVariants(username);

            return usernames.Any(u => variants.Any(v => v.Equals(u, StringComparison.OrdinalIgnoreCase)));
        }

        static void Main(string[] args)
        {
            var app = new CommandLineApplication(false);
            var arguments = new Arguments(app);
            app.Name = "Twitter Rollup";
            app.Description = "A command line utility which pulls in tweets from a list of usernames and sends a summary to an email address.";
            
            app.VersionOption("-v | --version", "1.0", "1.0.0");
            app.HelpOption("-? | -h | --help");

            app.OnExecute(async () => 
            {
                arguments.Parse();
                
                var twitter = new TwitterClient(arguments.TwitterToken);
                var tweets = new List<Tweet>();
                string subject = $"Twitter Rollup for {DateTime.Now.ToString("MMM dd, yyyy")}.";
                string html = $"<h1>{subject}</h1><p>Sorted by user, oldest to newest.</p>";

                foreach (var username in arguments.Usernames)
                {
                    var hasReplies = arguments.WithKnownReplies || arguments.WithReplies || arguments.WithSelfReplies;
                    var userTweets = (await twitter.GetTweetsForUser(username, hasReplies)).ToList();

                    if (!hasReplies || arguments.WithReplies)
                    {
                        tweets.AddRange(userTweets);

                        continue;
                    }

                    if (! arguments.WithKnownReplies)
                    {
                        var variants = GetUsernameVariants(username);

                        // Filter out all replies that aren't to self.
                        tweets.AddRange(userTweets.Where(t =>
                        {
                            if (t.is_quote_status || !t.is_reply_status)
                            {
                                return true;
                            }

                            return HasMatch(variants, t.in_reply_to_screen_name);
                        }));

                        continue;
                    }
                       
                    // Filter out all replies that aren't to a known user.
                    tweets.AddRange(userTweets.Where(t => 
                    {
                        if (t.is_quote_status || !t.is_reply_status)
                        {
                            return true;
                        }

                        var targetUser = t.in_reply_to_screen_name;

                        return HasMatch(arguments.Usernames, t.in_reply_to_screen_name);
                    }));
                }

                html += string.Join("\n", tweets.Select(t => TweetFormatter.ToHtml(t)));

                if (arguments.TestFlag)
                {
                    using (var file = File.OpenWrite(GetFilePath()))
                    {
                        var bytes = Encoding.UTF8.GetBytes(html);

                        await file.WriteAsync(bytes, 0, bytes.Length);
                    }

                    return 0;
                }

                return 0;
            });

            app.Execute(args);
        }
    }
}
