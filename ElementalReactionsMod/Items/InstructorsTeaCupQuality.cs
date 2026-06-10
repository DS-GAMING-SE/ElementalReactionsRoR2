using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using ItemQualities;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace ElementalReactionsMod.Items
{
    public class InstructorsTeaCupQuality : CharacterBody.ItemBehavior
    {
        public int uncommonItemCount;
        public int rareItemCount;
        public int epicItemCount;
        public int legendaryItemCount;

        private void Start()
        {
            ElementalReactionManager.onElementalReactionTriggered += OnElementalReaction;
        }

        private void OnElementalReaction(ElementalReactionDef reaction, ElementDef element1, ElementDef element2, CharacterBody victim, GameObject attacker)
        {
            if (attacker && attacker == gameObject && body)
            {
                QualityTier highestQuality = QualityTier.None;
                if (legendaryItemCount > 0) highestQuality = QualityTier.Legendary;
                if (epicItemCount > 0) highestQuality = QualityTier.Epic;
                if (rareItemCount > 0) highestQuality = QualityTier.Rare;
                if (uncommonItemCount > 0) highestQuality = QualityTier.Uncommon;
                if (highestQuality != QualityTier.None)
                {
                    BuffDef buff = BuffCatalog.GetBuffDef(QualityCatalog.GetBuffIndexOfQuality(Buffs.instructorsTeaCupQualityBase.buffIndex, highestQuality));
                    body.AddTimedBuff(buff,
                        StaticValues.instructorsTeaCupQualityDuration + (StaticValues.instructorsTeaCupQualityDurationPerQuality * (float)highestQuality),
                        StaticValues.instructorsTeaCupQualityMaxStacks + (StaticValues.instructorsTeaCupQualityStacksPerQuality * (int)highestQuality));
                    body.ExtendTimedBuffIfPresent(buff, StaticValues.instructorsTeaCupQualityMaxStacks + (StaticValues.instructorsTeaCupQualityStacksPerQuality * (int)highestQuality));
                }
            }
        }

        private void OnDisable()
        {
            if (body)
            {
                BuffQualityGroup buffs = QualityCatalog.GetBuffQualityGroup(QualityCatalog.FindBuffQualityGroupIndex(Buffs.instructorsTeaCupQualityBase.buffIndex));
                if (buffs)
                {
                    body.ClearTimedBuffs(buffs.UncommonBuffIndex);
                    body.ClearTimedBuffs(buffs.RareBuffIndex);
                    body.ClearTimedBuffs(buffs.EpicBuffIndex);
                    body.ClearTimedBuffs(buffs.LegendaryBuffIndex);
                }
            }
            ElementalReactionManager.onElementalReactionTriggered -= OnElementalReaction;
        }
    }
}
