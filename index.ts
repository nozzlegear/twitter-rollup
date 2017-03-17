#!/usr/bin/env node

import * as fs from "fs";
import * as util from "util";
import * as Twitter from "twitter";
import * as Bluebird from "bluebird";
import { createTransport } from "nodemailer";

const client = new Twitter({
    consumer_key: process.env.TWITTER_CONSUMER_KEY,
    consumer_secret: process.env.TWITTER_CONSUMER_SECRET,
    bearer_token: process.env.TWITTER_BEARER_TOKEN,
})

function inspect(arg1: string | any, arg2?) {
    if (arg2) {
        console.log(arg1, util.inspect(arg2, { colors: true }));
    } else {
        console.log(util.inspect(arg1, { colors: true }));
    }
}

const getTweets = (username, sinceId: number) => new Bluebird<Twitter.Tweet[]>((res, rej) => {
    client.get("statuses/user_timeline", { screen_name: username, since_id: sinceId || undefined, exclude_replies: true, tweet_mode: "extended" }, (err, tweets, resp) => {
        if (err) {
            inspect(`Error getting tweets for ${username}.`, { error: err, resp: resp.toJSON() });
        }

        // Don't break the app, just return 0 tweets.
        res(tweets && tweets.reverse() || []);
    })
});

function toHtml(tweet: Twitter.Tweet) {
    const strings: string[] = [];
    const username = tweet.user.name;
    const isRetweet = !!tweet.retweeted_status;
    const urls = (isRetweet ? tweet.retweeted_status.entities.urls : tweet.entities.urls) || [];
    const media = (isRetweet ? tweet.retweeted_status.entities.media : tweet.entities.media) || [];
    const mentions = (isRetweet ? tweet.retweeted_status.entities.user_mentions : tweet.entities.user_mentions) || [];
    let text = isRetweet ? tweet.retweeted_status.full_text : tweet.full_text;

    text = mentions.reduce((text, mention, index, array) => {
        const indices = mention.indices;
        const wrapperStart = `<a href='https://twitter.com/${mention.screen_name}'>`;
        const mentionText = text.substring(indices[0], indices[1]);
        const wrapperEnd = `</a>`;

        text = text.substring(0, indices[0]) + (wrapperStart + mentionText + wrapperEnd) + text.substring(indices[1]);

        // All following index's should be `value + wrapperStart.length + wrapperEnd.length`;
        for (let i = index + 1; i < array.length; i++) {
            const nextIndex = array[i].indices;

            nextIndex[0] = nextIndex[0] + wrapperStart.length + wrapperEnd.length;
            nextIndex[1] = nextIndex[1] + wrapperStart.length + wrapperEnd.length;
        }

        return text;
    }, text);
    text = urls.reduce((text, url) => text = text.replace(url.url, tweet.is_quote_status && url.expanded_url.indexOf(tweet.quoted_status_id_str) > -1 ? "" : `<a href='${url.url}'>${url.expanded_url}</a>`), text);
    text = media.reduce((text, media) => media.type === "photo" ? text.replace(media.url, "") : text, text);

    strings.push("<p style='margin: 0'>");
    strings.push(`<a href='https://twitter.com/statuses/${tweet.id_str}'>(#)</a>`);

    if (isRetweet) {
        strings.push(`<strong>${username}</strong> retweeted <strong><a href='https://twitter.com/${tweet.retweeted_status.user.screen_name}'>@${tweet.retweeted_status.user.screen_name}</a></strong>:`)
    } else if (tweet.is_quote_status && tweet.quoted_status) {
        strings.push(`<strong>${username}</strong> quoted <strong><a href='https://twitter.com/${tweet.quoted_status.user.screen_name}'>@${tweet.quoted_status.user.screen_name}</a></strong>:`);
        text = text + `<blockquote>"${tweet.quoted_status.full_text}"</blockquote>`;
    } else {
        strings.push(`<strong>${username}</strong>:`)
    }

    strings.push("</p>");
    strings.push("<p style='margin:0; margin-top:10px;'>")
    strings.push(text.replace(/\n/ig, "<br/>"));
    strings.push("</p>");

    media.forEach(media => {
        if (media.type === "photo") {
            strings.push(`<div style='padding-top: 10px'><a href='${media.url}'><img src='${media.media_url_https}' style='max-width: 100%; max-height:400px' /></a></div>`)
        }
    })

    return `<div style='padding: 10px 0; border-bottom: 1px solid #ccc;'>${strings.join(" ")}</div>`;
}

const sendRoundup = (html: string) => new Bluebird((res, rej) => {
    const message = {
        content: {
            from: {
                name: "Twitter Roundup",
                email: `twitter-roundup@nozzlegear.com`,
            },
            subject: `Twitter roundup for ${new Date().toLocaleDateString("en-US", { day: "numeric", month: "short", year: "numeric" })}.`,
            html: html,
        },
        recipients: [{
            address: {
                email: "nozzlegear@outlook.com",
            }
        }]
    }

    //Send the roundup email
    const transporter = createTransport({ transport: 'sparkpost', sparkPostApiKey: process.env.SPARKPOST_API_KEY } as any);

    transporter.sendMail(message, (error, info) => {
        if (error) {
            rej(error);

            return;
        }

        res(info);
    });
});

async function start() {
    const fileLocation = "./tweet-history.json";
    let history: { [username: string]: { "lastTweetId": number } };

    if (fs.existsSync(fileLocation)) {
        history = JSON.parse(fs.readFileSync(fileLocation).toString());
    } else {
        history = {};
    }

    const usernames = [
        "jessecox", 
        "crendor", 
        "jkcompletesit", 
        "facianea", 
        "pokekellz",
        "akamikeb", 
        "explainxkcd",
        "patio11", 
        "wesbos", 
        "mpjme",
        "dan_abramov",
        "nolanlawson",
        "shanselman",
        "JenMsft",
    ];
    const tweets = await Bluebird.reduce(usernames, async (result, username) => {
        const userHistory = history[username];
        const tweets = await getTweets(username, userHistory && userHistory.lastTweetId);

        if (tweets.length > 0) {
            result[username] = tweets;
            history[username] = { lastTweetId: tweets[tweets.length - 1].id };
        }

        return result;
    }, {} as { [username: string]: Twitter.Tweet[] });

    const html = "<h1>Daily Twitter Roundup</h1><p>Sorted by user, oldest to newest.</p>" + Object.getOwnPropertyNames(tweets).reduce((html, username) => {
        const userTweets = tweets[username];

        if (userTweets.length === 0) {
            return html + `<div>No tweets for @${username}. Was there a problem with the program?</div>`;
        }

        return html + userTweets.map(tweet => toHtml(tweet)).join("\n");
    }, "");

    try {
        const send = await sendRoundup(html);
    } catch (e) {
        inspect("Error sending roundup email", e);
    }

    fs.writeFileSync(fileLocation, JSON.stringify(history));
}

function scheduleRoundup() {
    const now = new Date();

    // Schedule the script to run at noon UTC on the next day.
    now.setUTCDate(now.getUTCDate() + 1);
    now.setUTCHours(12);
    now.setUTCMinutes(0);

    const countdown = now.getTime() - Date.now();
    const timeout = setTimeout(async function () {
        try {
            await start();
        } catch (e) {
            inspect("Error running Twitter Rollup", e);
        }

        scheduleRoundup();
    }, countdown);

    console.log(`Scheduled next roundup to run at ${now.toUTCString()} (${countdown} milliseconds from now.)`);
}

if (process.argv.find(arg => arg === "--now")) {
    start().catch(e => inspect("Error running Twitter Rollup", e));
} else {
    scheduleRoundup();
}
