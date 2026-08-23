# Changelog

## v1.1.1
- (Bug Fix) Fixed Superconduct increasing damage by 130% instead of 30%. Oops

## v1.1.0
- (Rework) Reworked a few elemental reactions to function closer to how they do in the original game *and to enable triggering reactions more often*
	- These reworks involve allowing multiple elements to persist on a target at once. You can now trigger multiple reactions at once when attacking a target with multiple elements on them
	- Burning is now its own DOT instead of using the base-game ignite effect. Burning damage now scales with elemental reaction damage multipliers, like Instructor's Tea Cup. Burning is still able to be buffed by Ignition Tank
	- Burning and Electro-Charge no longer remove their elements on being triggered. The DOT effects now last as long as the triggering elements remain
	
- (Visual) Damage numbers from reactions now use the same color as damage from items
- (Visual) The screen effect from elemental weather is now slightly more visible

- (Compatibility) Added elements to the drones from [Sandswept](https://thunderstore.io/c/riskofrain2/p/SandsweptTeam/Sandswept/) and [Starstorm 2](https://thunderstore.io/package/TeamMoonstorm/Starstorm2/) (Ally elements must be enabled in the config)

- (Mod Page) Edited the mod icon again<br>*Get rid of the circle, focus on balance over symmetry. I already solved this with the expansion icon, idk why I had to overcomplicate this*

- (Bug Fix) Noxious Thorn is now able to proc reactions when transfering elements
- (Bug Fix) Fixed cases where you would still have elements on your skills even if you selected Physical in your loadout
- (Bug Fix) Fixed the game not starting if [Sandswept's](https://thunderstore.io/c/riskofrain2/p/SandsweptTeam/Sandswept/) Cannonball Jellyfish was disabled

## v1.0.1
- (Mod Page) Edited the mod icon a bit 
<br>*I didn't like the old one because it wasn't symmetrical, now it's symmetrical and more accurate to the source material but it feels uneven. I can't decide which I dislike more)*

- (Bug Fix) Fixed incompat with Bubbet's Items (Improved TakeDamageProcess IL matching)

## v1.0.0

- First release