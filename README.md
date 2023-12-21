# CTModsLC
Mods for Lethal Company made by Toemmsen96 and Chrigi2486

The newest stable .dll for the corresponding mods will be in the main directory

## Mods

### TestMod
A mod for testing random things  
This mod won't be continously updated  
Currently it increases your sprint speed and provides chat commands like  
/spawn scrap \<name\> (amount=\<amount\>) (position={random, @me, @\<playername\>})  
or  
/spawn special gun (amount=\<amount\>) (position={random, @me, @\<playername\>})

### GoOutWithABangMod
Spawns a mine on player and triggers it when they die, unless they die to explosion or unknown  

### KarmaForBeingAnnoyingMod
Blows up the player who uses any of the annoying scrap that makes sound (clown horn, blow horn, ...)
This only kills the player using the item, as they deserve it.  
By default it has a 10% chance of activating.
This version spawns an explosion only client sided so everyone else is confused what the player died from.

#### Config
On first startup a .cfg file gets generated:  
In the config file Chrigi.KarmaForBeingAnnoyingMod.cfg in BepInEx/config you can change the following parameters:  
General Damage Range -> set damage range of the explosion  
General Delay -> set delay the explosion has until it gets executed  
ON OFF switch -> set if mod is on or off  
UseOnRemote -> set if mod applies to remote item  
General Kill Range -> set kill range of explosion  
General Probability -> set probability for non listed items  
Remote Probability -> set probability for remote item  
Airhorn Probability -> set probability for Airhorn item  
Clownhorn Probability -> set probability for Clownhorn item  
Cashregister Probability -> set probability for Cashregister item  
Hairdryer Probability -> set probability for Hairdryer item 

