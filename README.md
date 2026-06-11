# <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/texElementalReactionExpansionIcon.png?raw=true" width="64"> Elemental Reactions

Adds the Elemental Reaction system from Genshin Impact to the game. In the character select screen, you can select what elements you want each of your skills to be. Hit enemies to apply the element to them. **Applying multiple elements at once will cause a reaction, which can have a variety of effects based on the elements its made of.**

This mod relies on characters having Damage Sources implemented. **Modded survivors who can't use items like Luminous Shot or Breaching Fin won't be able to use the elements either.**

*picture of element select menu*

## Reactions
There are seven elements, most of which have a unique reaction when combined.

| Pyro | Hydro | Electro | Cryo | Anemo | Geo | Dendro |
| ---- | ----- | ------- | ---- | ----- | --- | ------ |
| <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texPyroSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texHydroSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texElectroSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texCryoSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texAnemoSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texGeoSkillIcon.png?raw=true" width="64"> | <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Common/Skill%20Icons/texDendroSkillIcon.png?raw=true" width="64"> |

explain reactions

The elements are not a tool exclusive to you. **Enemies can also deal elemental damage and trigger reactions against you**. Enemies have the elements built into themselves and their abilities based on what would logically make sense. Lesser Wisps are embodiments of Pyro, Golems deal Geo damage with their strikes and Electro damage with their lasers, etc. This includes enemies from certain enemy mods, which you can find listed in the Supported Mods section.

## Items

This mod also adds a few new items that all interact with the elements.

| Icon | Item |
| ---- | ----------- |
| <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Items/InstructorsTeaCup/texInstructorsTeaCupItemIcon.png?raw=true" width="128"> | **Common - Instructor's Tea Cup**<br>*Deal bonus damage from elemental reactions.*<br><br>Increases elemental reaction damage by 20% (+20% per stack). |
| <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Items/Moonwheel/texMoonWheelItemIcon.png?raw=true" width="128"> | **Legendary - Moon Wheel**<br>*Upgrades the Hydro reactions between Electro, Dendro, and Geo into powerful Lunar Reactions*<br><br>Upgrades the Electro-Charge, Bloom, and Hydro-Crystallize reactions into Lunar Reactions that can critically strike. Increases Lunar Reaction damage by 0% (+50% per stack).<br><br>Lunar-Charged: Continuously strike the target with lightning, dealing 4x500% damage.<br>Lunar-Bloom: Create a Dendro Core and gain a Verdant Dew, up to 3. Dealing Dendro skill damage with 3 Verdant Dews will consume them and increase the damage dealt by 150%.<br>Lunar-Crystallize: Create three Moondrifts. For every 3 times this reaction is triggered, the Moondrifts will deal 3x300% damage. |
| <img src="https://github.com/DS-GAMING-SE/ElementalReactionsRoR2/blob/master/ElementalReactionsUnityProject/Assets/ElementalReactionsAssets/Items/Delusion/texDelusionItemIcon.png?raw=true" width="128"> | **Lunar - Delusion**<br>*Resonates with a new element on pickup. Activating your Special skill will make any skill damage fire attacks of the Delusion's elemental damage...* **BUT at the cost of your health**.<br><br>delusion desc. |

## Supported Mods
- [LookingGlass](https://thunderstore.io/package/DropPod/LookingGlass/)
- [RiskOfOptions](https://thunderstore.io/package/Rune580/Risk_Of_Options/)
- [Quality](https://thunderstore.io/package/Goorakh/Quality/)
	- Instructor's Tea Cup and Moon Wheel have quality variants
	
Enemies added by the following mods will be able to use the elements
- [EnemiesReturns](https://thunderstore.io/package/Risky_Sleeps/EnemiesReturns/)
- [Starstorm 2](https://thunderstore.io/package/TeamMoonstorm/Starstorm2/)
- [BootlegBestiary](https://thunderstore.io/package/Skeletogne/BootlegBestiary/)