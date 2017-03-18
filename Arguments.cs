using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;

namespace twitter_rollup
{
    class Arguments
    {
        public Arguments(CommandLineApplication app, bool throwOnMissingArgumnets = true)
        {
            var tokenOption = app.Option("-t | --token", "Twitter access token (required)", CommandOptionType.SingleValue);
            var usersOption = app.Option("-u | --usernames", "Comma-separated list of Twitter usernames (required)", CommandOptionType.SingleValue);
            
            // Gather flags
            WithReplies = app.Option("-r | --replies", "Include replies in rollup email. (default: false)", CommandOptionType.NoValue).HasValue();
            WithKnownReplies = app.Option("-k | --known-replies", "Include replies from usernames passed to -u or --usernames. Always true when --replies is used. (default: false)", CommandOptionType.NoValue).HasValue();
            WithSelfReplies = app.Option("-s | --self-replies", "Include tweets that are the user replying to themselves. Always true when --replies or --known-replies are set. (default: false)", CommandOptionType.NoValue).HasValue();
            TestFlag = app.Option("--test", $"Skip the rollup email and output its HTML to {Program.GetFilePath()} (default: false)", CommandOptionType.NoValue).HasValue();


            if (!tokenOption.HasValue() || string.IsNullOrEmpty(tokenOption.Value())) 
            {
                throw new ArgumentNullException($"Twitter access token (-t or --token) is required.");
            }

            if (!usersOption.HasValue() || string.IsNullOrEmpty(usersOption.Value()))
            {
                throw new ArgumentNullException($"Comma-separated list of Twitter usernames (-u or --usernames) is required.");
            }

            if (!TestFlag)
            {
                // TODO: Gather and validate SMTP options
            }

            Usernames = usersOption.Value().Split(',').Select(s => s.Trim());
            TwitterToken = tokenOption.Value();
        }

        public IEnumerable<string> Usernames { get; private set; }

        public string TwitterToken { get; private set; }

        public bool WithReplies { get; private set; }

        public bool WithKnownReplies { get; private set; }

        public bool WithSelfReplies { get; private set; }

        public bool TestFlag { get; private set; }
    }
}