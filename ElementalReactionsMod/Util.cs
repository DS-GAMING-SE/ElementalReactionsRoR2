using ElementalReactionsMod.Elements;
using ElementalReactionsMod.Reactions;
using LookingGlass.LookingGlassLanguage;
using R2API;
using RoR2;
using RoR2.Navigation;
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
                if (GetBit((int)elementIndex, i))
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
            int index = 0;
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
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff, bool isCooldown = false, bool hidden = false)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = "bdElementalReactions"+buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;
            buffDef.isHidden = hidden;
            buffDef.isCooldown = isCooldown;

            Content.AddBuffDef(buffDef);

            return buffDef;
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
        public static void ManualBlastAttack(Vector3 origin, float radius, GameObject attacker, TeamIndex attackerTeam, float damage, float damageToPlayer, bool crit, DamageTypeCombo damageType, bool friendlyFire)
        {
            Collider[] colliders;
            int overlapCount = HGPhysics.OverlapSphere(out colliders, origin, radius, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Collide);
            HealthComponent[] hitHealthComponents = new HealthComponent[overlapCount];
            int hitCount = 0;
            for (int i = 0; i < overlapCount; i++)
            {
                CharacterBody characterBody = RoR2.Util.HurtBoxColliderToBody(colliders[i]);
                if (characterBody && characterBody.healthComponent && Array.IndexOf<HealthComponent>(hitHealthComponents, characterBody.healthComponent, 0, hitCount) == -1)
                {
                    if (!FriendlyFireManager.ShouldSplashHitProceed(characterBody.healthComponent, attackerTeam) && !friendlyFire)
                    {
                        continue;
                    }
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.attacker = attacker;
                    damageInfo.crit = crit;
                    damageInfo.damage = characterBody.teamComponent && characterBody.teamComponent.teamIndex == TeamIndex.Player ? damageToPlayer : damage;
                    damageInfo.force = Vector3.zero;
                    damageInfo.inflictor = attacker;
                    damageInfo.position = colliders[i].transform.position;
                    damageInfo.procCoefficient = 0f;
                    damageInfo.damageColorIndex = DamageColorIndex.Default;
                    damageInfo.damageType = damageType;
                    damageInfo.damageType.AddModdedDamageType(DamageTypes.elementalReactionDamageType);
                    damageInfo.inflictedHurtbox = colliders[i].GetComponent<HurtBox>();
                    characterBody.healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, characterBody.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, characterBody.gameObject);
                    hitHealthComponents[hitCount++] = characterBody.healthComponent;
                }
            }
            HGPhysics.ReturnResults(colliders);
        }
        public static void AddTimedBuffTimer(this CharacterBody body, BuffDef buff, int timer)
        {
            int num = 1;
            while (num <= timer)
            {
                body.AddTimedBuff(buff, num);
                num++;
            }
        }

        public static bool GetRandomNode(Vector3 origin, out Vector3 destination, float minDistance, float maxDistance)
        {
            NodeGraph nodeGraph = SceneInfo.instance.GetNodeGraph(MapNodeGroup.GraphType.Ground);
            NodeGraph.NodeIndex nodeIndex = NodeGraph.NodeIndex.invalid;
            List<NodeGraph.NodeIndex> list;

            list = nodeGraph.FindNodesInRange(origin, minDistance, maxDistance, HullMask.Human);
            if (list.Count > 0)
            {
                nodeIndex = list[UnityEngine.Random.Range(0, list.Count)];
            }
            if (nodeIndex == NodeGraph.NodeIndex.invalid)
            {
                list ??= new();
                nodeGraph.GetActiveNodesForHullMask(HullMask.Human, list);
                if (list.Count > 0)
                {
                    nodeIndex = list[UnityEngine.Random.Range(0, Mathf.Max(1, list.Count))];
                }
            }
            if (list.Count <= 0)
            {
                destination = origin;
                return false;
            }

            nodeGraph.GetNodePosition(nodeIndex, out Vector3 vector3);
            destination = vector3;
            return true;
        }
    }
}
