using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using LookingGlass.LookingGlassLanguage;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;

namespace ElementalReactionsMod
{
    public static class Util
    {
        public static ElementIndex GetElement(this DamageTypeCombo damageTypeCombo)
        {
            return (ElementIndex)GetDamageTypeIndex(damageTypeCombo, ElementCatalog.elementIndexDamageTypeBits);
        }

        public static void SetElement(this ref DamageTypeCombo damageTypeCombo, ElementIndex elementIndex)
        {
            for (int i = 0; i < ElementCatalog.elementIndexDamageTypeBits.Length; i++)
            {
                if (GetBit((int)elementIndex + 1, i))
                {
                    damageTypeCombo.AddModdedDamageType(ElementCatalog.elementIndexDamageTypeBits[i]);
                }
                else
                {
                    damageTypeCombo.RemoveModdedDamageType(ElementCatalog.elementIndexDamageTypeBits[i]);
                }
            }
        }

        public static bool IsElementalDamage(this DamageTypeCombo damageTypeCombo)
        {
            foreach (var item in ElementCatalog.elementIndexDamageTypeBits)
            {
                if (damageTypeCombo.HasModdedDamageType(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsElementalReactionDamage(this DamageTypeCombo damageTypeCombo)
        {
            return damageTypeCombo.HasModdedDamageType(DamageTypes.elementalReactionDamageType);
        }

        public static int GetDamageTypeIndex(DamageTypeCombo damageTypeCombo, DamageAPI.ModdedDamageType[] damageTypes)
        {
            int index = -1;
            for (int i = 0; i < damageTypes.Length; i++)
            {
                if (damageTypeCombo.HasModdedDamageType(damageTypes[i]))
                {
                    index += (int)Mathf.Pow(2, i);
                }
            }
            return index;
        }

        public static bool GetBit(int num, int bitIndex)
        {
            return (num & (1 << bitIndex)) != 0;
        }
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff, bool hidden = false)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;
            buffDef.isHidden = hidden;

            Content.AddBuffDef(buffDef);

            return buffDef;
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        internal static void RegisterLookingGlassBuff(Language lang, BuffDef buff, string name, string description)
        {
            LookingGlassLanguageAPI.SetupToken(lang, $"NAME_{buff.name}", name);
            LookingGlassLanguageAPI.SetupToken(lang, $"DESCRIPTION_{buff.name}", description);
        }
        public static BlastAttack CreateBlastAttack(DamageInfo damageInfo, float damage, float radius, float proc, ElementIndex element, bool isReaction, float force)
        {
            CharacterBody characterBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
            DamageTypeCombo damageType = default;
            damageType.SetElement(element);
            if (isReaction)
            {
                damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                damageType |= DamageType.AOE;
            }
            return CreateBlastAttack(characterBody ? characterBody : null, characterBody ? characterBody.damage * damage : damage, false, radius, BlastAttack.FalloffModel.None, proc, damageType, damageInfo.position, force);
        }
        public static BlastAttack CreateBlastAttack(DamageInfo damageInfo, float damage, float radius, float proc, DamageTypeCombo damageType, float force)
        {
            CharacterBody characterBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
            return CreateBlastAttack(characterBody ? characterBody : null, characterBody ? characterBody.damage * damage : damage, false, radius, BlastAttack.FalloffModel.None, proc, damageType, damageInfo.position, force);
        }
        public static BlastAttack CreateBlastAttack(CharacterBody attacker, float damage, bool crit, float radius, BlastAttack.FalloffModel falloff, float proc, DamageTypeCombo damageType, Vector3 position, float force)
        {
            BlastAttack blastAttack = new BlastAttack
            {
                attacker = attacker ? attacker.gameObject : null,
                inflictor = attacker ? attacker.gameObject : null,
                baseDamage = damage,
                crit = crit,
                radius = radius,
                falloffModel = falloff,
                procCoefficient = proc,
                teamIndex = attacker ? attacker.teamComponent.teamIndex : TeamIndex.None,
                damageType = damageType,
                position = position,
                baseForce = force,
                attackerFiltering = AttackerFiltering.NeverHitSelf
            };
            return blastAttack;
        }
    }
}
