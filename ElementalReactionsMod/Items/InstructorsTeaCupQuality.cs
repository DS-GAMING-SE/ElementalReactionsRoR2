using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using ItemQualities;
using ItemQualities.Utilities.Extensions;
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
                else if (epicItemCount > 0) highestQuality = QualityTier.Epic;
                else if (rareItemCount > 0) highestQuality = QualityTier.Rare;
                else if (uncommonItemCount > 0) highestQuality = QualityTier.Uncommon;
                if (highestQuality != QualityTier.None)
                {
                    BuffDef buff = BuffCatalog.GetBuffDef(QualityCatalog.GetBuffIndexOfQuality(Buffs.instructorsTeaCupQualityBase.buffIndex, highestQuality));
                    body.ConvertQualityBuffsToTier(QualityCatalog.FindBuffQualityGroupIndex(Buffs.instructorsTeaCupQualityBase.buffIndex), highestQuality);
                    body.AddTimedBuff(buff,
                        StaticValues.instructorsTeaCupQualityDuration + (StaticValues.instructorsTeaCupQualityDurationPerQuality * (float)highestQuality),
                        StaticValues.instructorsTeaCupQualityMaxStacks + (StaticValues.instructorsTeaCupQualityStacksPerQuality * (int)highestQuality));
                    body.SetTimedBuffDurationIfPresent(buff, StaticValues.instructorsTeaCupQualityDuration + (StaticValues.instructorsTeaCupQualityDurationPerQuality * (float)highestQuality), true);
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
                    body.RemoveAllQualityBuffs(buffs);
                }
            }
            ElementalReactionManager.onElementalReactionTriggered -= OnElementalReaction;
        }
    }
}
