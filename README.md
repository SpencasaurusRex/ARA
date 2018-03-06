## ARA

ARA is a long term project endeavoring to make a game about managing a factory by programming individual robots. These robots will be programmed by the player via lua scripts, with provided functions to move around and interact with the world.

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
I aim for this game to provide the player with a complex and challenging, yet rewarding sandbox. Due to the difficult and time consuming nature of the game I aim to make, I don't expect the game to be very popular, and only target a small niche audience.

Enhanced performance is one of my biggest concerns with this project. I want the player to be able to be running hundreds of robots with at least 50 ticks/second. The feasibility of that goal has yet to be tested.

The movement system, rendering system, and interpreters must work harmoniously in order to achieve this goal, requiring me to think about these systems in ways that go way beyond my previous experience.

My two main objectives with this project are to learn more about performance and optimizations and to make a game that I myself would love to play. As I am a big proponent of Open Source, this game will remain open source for a majority of it's development. Depending on its success towards the end of development, I ***might*** opt to move continued development to a private repo, keeping the source up to that point public.

## Progress
The game is in the earliest stage of development, without a working prototype as of yet.

I stream development of this game occassionally on my [Twitch Channel](https://twitch.tv/SpensasaurusRex)
