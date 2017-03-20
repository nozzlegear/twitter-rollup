using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;

namespace twitter_rollup
{
    class Arguments
    {
        CommandOption TokenOption { get; set; }

        CommandOption UsersOption { get; set; }

        CommandOption WithKnownRepliesOption { get; set; }

        CommandOption WithSelfRepliesOption { get; set; }

        CommandOption WithRepliesOption { get; set; }

        CommandOption TestOption { get; set; }

        public Arguments(CommandLineApplication app)
        {
            TokenOption = app.Option("-t | --token", "Twitter access token (required)", CommandOptionType.SingleValue);
            UsersOption = app.Option("-u | --usernames", "Comma-separated list of Twitter usernames (required)", CommandOptionType.SingleValue);
            
            // Gather flags
            WithRepliesOption = app.Option("-r | --replies", "Include replies in rollup email. (default: false)", CommandOptionType.NoValue);
            WithKnownRepliesOption = app.Option("-k | --known-replies", "Include replies from usernames passed to -u or --usernames. Always true when --replies is used. (default: false)", CommandOptionType.NoValue);
            WithSelfRepliesOption = app.Option("-s | --self-replies", "Include tweets that are the user replying to themselves. Always true when --replies or --known-replies are set. (default: false)", CommandOptionType.NoValue);
            TestOption = app.Option("--test", $"Skip the rollup email and output its HTML to {Program.GetFilePath()} (default: false)", CommandOptionType.NoValue);
        }

        public void Parse(bool throwOnMissingArgumnets = true)
        {
            if (!TokenOption.HasValue() || string.IsNullOrEmpty(TokenOption.Value())) 
            {
                throw new ArgumentNullException($"Twitter access token (-t or --token) is required.");
            }

            if (!UsersOption.HasValue() || string.IsNullOrEmpty(UsersOption.Value()))
            {
                throw new ArgumentNullException($"Comma-separated list of Twitter usernames (-u or --usernames) is required.");
            }

            WithReplies = WithRepliesOption.HasValue();
            WithKnownReplies = WithKnownRepliesOption.HasValue();
            WithSelfReplies = WithSelfRepliesOption.HasValue();
            TestFlag = TestOption.HasValue();
            
            if (!TestFlag)
            {
                // TODO: Gather and validate SMTP options
            }

            Usernames = UsersOption.Value().Split(',').Select(s => s.Trim());
            TwitterToken = TokenOption.Value();
        }

        public IEnumerable<string> Usernames { get; private set; }

        public string TwitterToken { get; private set; }

        public bool WithReplies { get; private set; }

        public bool WithKnownReplies { get; private set; }

        public bool WithSelfReplies { get; private set; }

        public bool TestFlag { get; private set; }
    }
}