# Experimental Pacman - AI Personality and Cognition (AiPerCog) Research Project

## Content

This repository contains the Unity (version 2022.3.20f1) project of our Pacman implementation. It is designed to work along with a [remote server with MySQL database](https://version.helsinki.fi/hipercog/behavlets/Web-Pacman) to collect gameplay data. The resulting data will be encoded and analyzed with [Pacman Behavlets Encoding System](https://version.helsinki.fi/hipercog/behavlets/encoder-pacman)

Relevant code is in Assets/Scripts

## Game Design

This is a non-standard implementation of Pacman, designed for research in gamer behavior. The game is built upon the educational example developed by [Zigurous](https://github.com/zigurous/unity-pacman-tutorial) with tweaked and new elements based on the [Pacman Dossier by Jamey Pittman](https://pacman.holenet.info/). However, simplifying for our research purposes, it is not an exact replica of the original game.

Map space is the standard of 26x29 tiles (walkable map, not considering UI space). Each tile is a 1x1 square, measured in Unity's x,y reference system. Pacman and ghosts speeds are proportional to the maximum speed of 9.47 tiles/sec [(75.76 pixels/seconds)](https://pacman.holenet.info/#LvlSpecs). While the pixels/sec might vary across clients, movement relative to the map remains constant and accurate to the original game, since map size and speed is defined by Unity measures.

Pacman and ghost speed only varies with level progression and by eating the power pills (energizers), unlike the original game, where pacman is slowed down when eating dots and ghosts when traversing the tunnels.


### Difficulty progression and Ghost behavior
Difficulty increases by modifying the speed, the frightened duration, and ghosts' home departure. Ghost Behavior is one of the main differences with the original game, details below.

Speed:

- Pacman speed is 5% higher than the ghosts except when eating a powerpill, where the difference in speeds is of 40% Progression in speed follows closely the original game's values, as described in the Pacman Dossier. Maximum speed, both for Pacman and Ghosts, is reached at level 5. However, at level 21+ ghosts' speed become 5% higher than Pacman

Fright time:
- Fright time starts at 6 seconds and it becomes shorter each level. However, it does not follow a logical progression. For example, in level 9 the fright time is 1 second, in level 10 is of 5 seconds, and in level 11 returns to 1 second. This brings unpredictability on assessing the risk of hunting ghosts.
- In original pacman, the number of flashes that frightened ghosts have is related to the timing and the moment where they turn. During the first 11 levels, ghost are going to flash 5 times before turning. This gives some guide to predict and watch for the turn in behavior. In this implementation, flashing of the ghosts is simplified and it always flicker three times, starting from half the frightened duration, before turning back to normal. This can be modified easily, by adding the level values in the levelData array (GameManager.cs) and modifying the loadleveldata() method to alter the "flickercount" variable in GhostFrightened.cs script.

Ghosts' home departure:
- Ghosts departure from home is simplified. Every ghost has a pre-defined timer to leave home. This timer decreases linearly with level progression, reaching its minimum at lvl 13. 
- Same rule applies for the time the ghosts spend at home after getting eaten.

Ghost behavior:
- After departure, each ghost scatters to patrol an assigned quadrant, following the original game's behavior.
- During the game they alternate between chase/scatter behavior. As difficulty increases, the sooner and longer they are in chasing behavior.
- Movement is guided by a grid of nodes positioned at each intersection. Ghost wont change path in the middle of a passage/tunnel
- When chasing Pacman, the targetting system is similar for every ghost. They chase pacman directly using a heuristic distance metric.
- Blinky is not faster than the other ghosts and there is no ["Cruise Elroy" speed boost](https://pacman.holenet.info/#CH4_Blinky) speed changes.



## Data Collection

Data collection scripts (Assets/Scripts/DataManagement/*) gather and sends raw gameplay data to remote server. Server address is defined in MainManager.cs scripts.


## Contact Information

Pablo Flores - pablo.flores@helsinki.fi
University of Helsinki

