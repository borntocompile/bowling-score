# bowling-score

## What is it?
`MFG.Bowling` is a scoring system for the sport of Bowling.  
It calculates all the complicated rules of bowling so you don't have to.  
All you need to do is create a player and inform it of what the player scored that "turn".  
The API will inform you when a player has completed all thier frames, when all your players have invoked this event your game is complete.

## Requirements
Support for Unity 2018.x  
The code will run in older versions of Unity, however the assembly definition will not.

## API

You can create a new player by newing the class `Player`  
```c#
var player = new Player(name: "Hazza")
```

A players score card is made up of a collection of Frames.  
Each frame is made up of a collection Rolls. 

To apply a score to the Player, simply use
```c#
player.Roll(score);
```

The `Player.OnFrameComplete` event is available for when a player completes a frame.
```c#
player.OnFrameComplete += frame =>
{
    // Set next players turn.
};
```

### Example
An example script is included, it can be found at `MFG.Bowling.Example`  
Attach the Example MonoBehaviour to a GameObject in your scene and run the game.   
Check the console for the logs of what happened in the game.  
A `perfectGame` option is available to simulate every turn as a strike.

