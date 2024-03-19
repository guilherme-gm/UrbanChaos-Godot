# Urban Chaos Godot
This project aims at recreating Urban Chaos (https://en.wikipedia.org/wiki/Urban_Chaos) game in the Godot engine.
The aim is to get a game like the original PC one.

To play it, it is required to have the original game files (e.g. from Steam or GOG) in your computer so we can
get the asset files.

What is NOT in the initial release plans:
1. Make big gameplay changes (small ones _may_ exist depending on the difficulty to port the original one)
2. Add unreleased content (e.g. bike, sewers, etc)
3. Modding support

This project gathers info from the released sources: https://github.com/dizzy2003/MuckyFoot-UrbanChaos

While the idea is to get close to the original, some things are likely to not be the same (e.g. particles)
because this project will make use of Godot's features as much as possible, while the original game had
it all coded from scratch.


## How it works
The first time you launch the game, a conversion process starts to gather the needed assets from the original game folder
and convert it to a Godot-friendly version.

After the conversion and in future launches, the game will start and you can play all the available/impemented content.

The original game may be uninstalled at this point. But when new versions of Urban Chaos Godot launches, you may need
to have the original game again (if we need to reconvert something).


## Project structure
1. **AssetTools:** Godot project to manage the original game assets (and eventually convert it). Contains some Editor tools to make it easier to analyze data.
2. **CodeGenerators:** C# Source Generators to support other projects.


## How to play
TODO:
