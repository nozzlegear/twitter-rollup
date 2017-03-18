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

        static void Main(string[] args)
        {
            var app = new CommandLineApplication(false);
            app.Name = "Twitter Rollup";
            app.Description = "A command line utility which pulls in tweets from a list of usernames and sends a summary to an email address.";
            
            app.VersionOption("-v | --version", "1.0", "1.0.0");
            app.HelpOption("-? | -h | --help");

            app.OnExecute(async () => 
            {
                var arguments = new Arguments(app);
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
                        // TODO: Filter out all replies that aren't to self.

                        continue;
                    }
                       
                    // TODO: Filter out all replies that aren't to a known user.
                    userTweets.Where(t => 
                    {
                        if (!t.is_reply_status)
                        {
                            return true;
                        }

                        

                        return true;
                    });
                }

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
