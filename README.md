# Procedural-And-Pathfinding

This is a game project that uses perlin noise to generate grid based levels and A* pathfinding to let the player and enemies traverse those levels.
The game is a infinite Semi-turnbased stealth game.
In the game you play as a thief who has broken into a museum and is trying to steal artefacts.
The player has to plan their moves to avoid being spotted and chased by the guards and then return to exit to move onto the next level.

Controls:

Left click:
On open tiles will generate a path to that tile.
On a Obstructed and highlighted tile next to the player character will start clearing it.
(It takes three attempts to clear a obstructed tile)

Space:
Will move the player character along their selected path.
(the playr cannot stop moving until they reach the final tile in the path)

Left ctrl + left click:
Holding left ctrl while clicking allow the player to extend their selected path.
(usefull for designing a specific path to travel)
