dotnet build
dotnet ./bin/Debug/netcoreapp1.1/twitter-rollup.dll -t $TWITTER_BEARER_TOKEN -u jessecox,crendor,akamikeb --test --known-replies
