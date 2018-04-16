#! /bin/pwsh

$version = $(cat "package.json" | jq .version).Trim("`"")
$imageName = "nozzlegear/twitter-rollup"

docker build -t "$imageName" -t "$imageName`:$version" .
docker push "$imageName"
