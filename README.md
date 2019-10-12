# C# Discord Bot Template
[![Build Status](https://travis-ci.com/jamesw-64/CSharp-Discord-Bot-Template.svg?branch=master)](https://travis-ci.com/jamesw-64/DiscordBot)

###### (Notice: previous repo URL: https://github.com/jamesw-64/DiscordBot)

A template for a C# Console Application Discord bot that makes use of the [Discord.Net Library](https://discord.foxbot.me/docs/index.html). Designed to run on a dedicated server, it comes with a 'Log' method that will print to the console while simultainiously saving the output to a log file found in the progam's executable directory. 

Should be able to run and be developed on any system that can run [Mono](https://www.mono-project.com/) C# programs.

Just clone straight from the directory and you're good to go!

## Stopcodes
Here are the stopcodes that are used by the template when exiting prematurely:
 - Stopcode 1: Did not find token.txt, program exited
 - Stopcode 2: Invalid user input, program exited
 - Stopcode 3: token updated at user prompt, program restarted
 - Stopcode 4: General, unhandled error

You are free to disregard these stopcodes or change them. It's just a little something extra.
