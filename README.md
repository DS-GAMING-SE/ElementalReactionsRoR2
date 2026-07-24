# <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/texElementalReactionExpansionIcon.png?raw=true" width="64"> Elemental Reactions

Adds the Elemental Reaction system from Genshin Impact to the game. In the character select screen, you can select what elements you want each of your skills to be. Hit enemies to apply the element to them. **Having multiple elements applied at once will cause a reaction, which can have a variety of effects based on the elements its made of.**

There are many effects that elemental reactions can have, including damage increases, dealing damage, debuffs, conditional effects, and creating new objects to work around. This mod is meant to add new variation and strategy to your future runs. Experiment with different combinations of elements. See what kinds of elemental selections work best with your favorite survivors. At its simplest, this mod adds a bunch of flashy effects to your attacks, so you can still enjoy it without putting too much thought into it.

This mod relies on characters having Damage Sources implemented. **Modded survivors who can't use items like Luminous Shot or Breaching Fin won't be able to use the elements either.**

<img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/tree/master/ReadmeImages/ElementSelect.jpg?raw=true">

## Reactions
There are seven elements, most of which will trigger a unique reaction when combined.

| Pyro | Hydro | Electro | Cryo | Anemo | Geo | Dendro |
| ---- | ----- | ------- | ---- | ----- | --- | ------ |
| <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texPyroSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texHydroSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texElectroSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texCryoSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texAnemoSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texGeoSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texDendroSkillIcon.png?raw=true" width="64"> |

<img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/tree/master/ReadmeImages/Overload.jpg?raw=true">
<img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/tree/master/ReadmeImages/Crystallize.jpg?raw=true">
<img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/tree/master/ReadmeImages/LunarBloom.jpg?raw=true">

The elements are not exclusive to you. **Enemies can also deal elemental damage and trigger reactions against you**. Most enemies have elements built into themselves and their abilities. Lesser Wisps are embodiments of Pyro, Golems deal Geo damage with their strikes and Electro damage with their lasers, etc. This includes enemies from certain enemy mods listed in the Supported Mods section. Certain kinds of elites and elite abilities can deal elemental damage too.

## Items

This mod also adds a few new items that interact with the elements.

| Icon | Item |
| ---- | ----------- |
| <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Items/InstructorsTeaCup/texInstructorsTeaCupItemIcon.png?raw=true" width="128"> | **Common - Instructor's Tea Cup**<br>*Deal bonus damage from elemental reactions.*<br><br>Increases elemental reaction damage by 20% (+20% per stack). |
| <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Items/Moonwheel/texMoonWheelItemIcon.png?raw=true" width="128"> | **Legendary - Moon Wheel**<br>*Upgrades the Hydro reactions between Electro, Dendro, and Geo into powerful Lunar Reactions*<br><br>Upgrades the Electro-Charge, Bloom, and Hydro-Crystallize reactions into Lunar Reactions that can critically strike. Increases Lunar Reaction damage by 0% (+50% per stack).<br><br>Lunar-Charged: Continuously strike the target with lightning, dealing 4x600% damage.<br>Lunar-Bloom: Create a Dendro Core and gain a Verdant Dew, up to 3. Dealing more than 400% damage while having 3 Verdant Dews will consume them and increase the damage dealt by 200%.<br>Lunar-Crystallize: Create a Moondrift. Creating 3 Moondrifts will fire them at nearby enemies, dealing 3x400% damage.<br><br>Unlocked after completing the **Elemental Mastery** achievement. |
| <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Items/Delusion/texDelusionItemIcon.png?raw=true" width="128"> | **Lunar - Delusion**<br>*Resonates with a new element on pickup. Activating your Special skill will make any skill damage fire attacks of the Delusion's elemental damage...* **BUT at the cost of your health**.<br><br>On pickup, resonate with a random element you don't have. Activating your Special skill will activate the Delusion for 10s. While active, damaging enemies with any skill will periodically fire an attack of the Delusion's element, dealing 600% base damage (+600% per same-element stack), while also draining your health by **5%** (+5% per same-element stack). While the Delusion is active, **healing received is reduced by 20%** (+20% per stack). A separate attack will be fired for each Delusion you have of a unique element. |

## Supported Mods
- [LookingGlass](https://thunderstore.io/package/DropPod/LookingGlass/)
- [RiskOfOptions](https://thunderstore.io/package/Rune580/Risk_Of_Options/)
- [Quality](https://thunderstore.io/package/Goorakh/Quality/)
	- Instructor's Tea Cup and Moon Wheel have quality variants
- [ZetAspects](https://thunderstore.io/c/riskofrain2/p/William758/ZetAspects/)
	- ZetAspects elite aspects (like normal aspects) grant elemental power
- [Starstorm 2](https://thunderstore.io/package/TeamMoonstorm/Starstorm2/)
    - Storms have unique interactions with the elements (Beta content hasn't been considered)
    - Item(s) from this mod are usable in Wandering Chef recipes
	- Survivors have item displays
- [Sandswept](https://thunderstore.io/c/riskofrain2/p/SandsweptTeam/Sandswept/)
    - Item(s) from this mod are usable in Wandering Chef recipes
	
Enemies added by the following mods will be able to use the elements
- [EnemiesReturns](https://thunderstore.io/package/Risky_Sleeps/EnemiesReturns/)
- [Starstorm 2](https://thunderstore.io/package/TeamMoonstorm/Starstorm2/)
- [BootlegBestiary](https://thunderstore.io/package/Skeletogne/BootlegBestiary/)
- [Sandswept](https://thunderstore.io/c/riskofrain2/p/SandsweptTeam/Sandswept/)