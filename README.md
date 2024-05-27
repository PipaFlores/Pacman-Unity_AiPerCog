# Experimental Pacman - AI Personality and Cognition (AiPerCog) Research Project

## Content

This repository contains the Unity (version 2022.3.20f1) project of our Pacman implementation. It is designed to work along with a [remote server with MySQL database](https://version.helsinki.fi/hipercog/behavlets/Web-Pacman) to collect gameplay data. The resulting data will be encoded and analyzed with [Pacman Behavlets Encoding System](https://version.helsinki.fi/hipercog/behavlets/encoder-pacman)

Relevant code is in Assets/Scripts

## Game Design

This is a non-standard implementation of Pacman, designed for research in gamer behavior. The game is built upon the educational example developed by [Zigurous](https://github.com/zigurous/unity-pacman-tutorial) with tweaked and new elements based on the [Pacman Dossier by Jamey Pittman](https://pacman.holenet.info/). However, it does not include several features from the original game (mainly in ghost behavior and pacman reduced speed when eating pellet)
Added features include login and welcome scenes, difficulty progression, quadrant based scattering behavior

## Data Collection

Data collection scripts (Assets/Scripts/DataManagement/*) gather and sends raw gameplay data to remote server. Server address is defined in MainManager.cs scripts.


## Contact Information

Pablo Flores - pablo.flores@helsinki.fi
University of Helsinki

