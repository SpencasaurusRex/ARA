## ARA

ARA is a game about managing a factory by programming robots. These robots will be programmed with lua scripts, provided with functions to move around and interact with the world.

Ultimately the goal is to have the player work through streamlining a production chain while taking into consideration resource and power constraints, and upgrading robots to better perform their tasks.

Here's an example of what a script *could* look like for an ***extremely basic*** crafter robot: 
```lua
function craft()
  turnRight(2)
  pickUp("iron_ingot")
  turnLeft(2)
  craft("iron_rod")
  dropDown(2)  
end

function start()
  forward()
  for i = 1,10 do
    craft()
  end
  back()
end
```

As the complexity of the factory's needs rises, naturally so will the scripts governing the robots.

## Objectives
* Provide the player with a complex and challenging, yet rewarding sandbox.
* Be able to run hundreds of robots without a performance drop. 
* Learn and enjoy the process of game-dev

## Progress
The game is very early on in development, but currently I am able to have robots run around executing lua scripts.
The scripts cannot yet be edited in game.

I stream development of this game occassionally on my [Twitch Channel](https://twitch.tv/SpensasaurusRex)
