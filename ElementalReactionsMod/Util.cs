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
using UnityEngine.Networking;

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

        public static bool CanUseElements(CharacterBody characterBody)
        {
            bool player = characterBody && (characterBody.isPlayerControlled || (characterBody.inventory && characterBody.inventory.GetItemCountEffective(Loadout.ElementLoadoutComponent.damageIsFromPlayerItem) > 0));
            return !characterBody || !characterBody.teamComponent ||
                ((characterBody.teamComponent.teamIndex == TeamIndex.Player && player && Config.CanSurvivorsUseElements().Value) ||
                (characterBody.teamComponent.teamIndex == TeamIndex.Player && !player && Config.CanAlliesUseElements().Value) ||
                (characterBody.teamComponent.teamIndex != TeamIndex.Player && Config.CanEnemiesUseElements().Value));
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
        internal static T AddNewBuff<T>(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff, bool isCooldown = false, bool hidden = false) where T : BuffDef
        {
            T buffDef = ScriptableObject.CreateInstance<T>();
            buffDef.name = "bdElementalReactions" + buffName;
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
            return CreateBlastAttack(damageInfo.attacker, characterBody ? characterBody.teamComponent.teamIndex : TeamComponent.GetObjectTeam(damageInfo.attacker), characterBody ? characterBody.damage * damage : damage * Run.instance.teamlessDamageCoefficient, false, radius, BlastAttack.FalloffModel.None, proc, damageType, damageInfo.position, force);
        }
        public static BlastAttack CreateBlastAttack(DamageInfo damageInfo, float damage, float radius, float proc, DamageTypeCombo damageType, float force)
        {
            CharacterBody characterBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
            return CreateBlastAttack(damageInfo.attacker, characterBody ? characterBody.teamComponent.teamIndex : TeamComponent.GetObjectTeam(damageInfo.attacker), characterBody ? characterBody.damage * damage : damage * Run.instance.teamlessDamageCoefficient, false, radius, BlastAttack.FalloffModel.None, proc, damageType, damageInfo.position, force);
        }
        public static BlastAttack CreateBlastAttack(GameObject attacker, TeamIndex team, float damage, bool crit, float radius, BlastAttack.FalloffModel falloff, float proc, DamageTypeCombo damageType, Vector3 position, float force)
        {
            BlastAttack blastAttack = new BlastAttack
            {
                attacker = attacker,
                inflictor = attacker,
                baseDamage = damage,
                crit = crit,
                radius = radius,
                falloffModel = falloff,
                procCoefficient = proc,
                teamIndex = team,
                damageType = damageType,
                position = position,
                baseForce = force,
                attackerFiltering = AttackerFiltering.NeverHitSelf
            };
            return blastAttack;
        }
        public static void ManualBlastAttack(Vector3 origin, float radius, GameObject attacker, TeamIndex attackerTeam, float damage, bool crit, DamageTypeCombo damageType, bool friendlyFire, bool playerResist, Action<CharacterBody> onHit = null)
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
                    float finalDamage = damage;
                    if (characterBody.teamComponent)
                    {
                        if (characterBody.teamComponent.teamIndex == attackerTeam)
                        {
                            finalDamage *= (Config.FriendlyFireBloomResistance().Value / 100f);
                        }
                        if (playerResist && characterBody.teamComponent.teamIndex == TeamIndex.Player)
                        {
                            finalDamage *= (Config.PlayerBloomResistance().Value / 100f);
                        }
                    }
                    DamageInfo damageInfo = new DamageInfo();
                    damageInfo.attacker = attacker;
                    damageInfo.crit = crit;
                    damageInfo.damage = finalDamage;
                    damageInfo.force = Vector3.zero;
                    damageInfo.inflictor = attacker;
                    damageInfo.position = colliders[i].transform.position;
                    damageInfo.procCoefficient = StaticValues.genericReactionProcCoefficient;
                    damageInfo.damageColorIndex = DamageColorIndex.Item;
                    damageInfo.damageType = damageType;
                    damageInfo.inflictedHurtbox = colliders[i].GetComponent<HurtBox>();
                    characterBody.healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, characterBody.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, characterBody.gameObject);
                    onHit?.Invoke(characterBody);
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
        public static float ReduceTimedBuffDuration(this CharacterBody characterBody, BuffDef buff, float reduction)
        {
            float newDuration = 0;
            if (characterBody.GetBuffCount(buff) <= 0 || !NetworkServer.active)
            {
                return newDuration;
            }
            for (int i = 0; i < characterBody.timedBuffs.Count; i++)
            {
                if (characterBody.timedBuffs[i].buffIndex == buff.buffIndex)
                {
                    if (characterBody.timedBuffs[i].timer <= reduction)
                    {
                        characterBody.timedBuffs.RemoveAt(i);
                        characterBody.RemoveBuff(buff.buffIndex);
                    }
                    else
                    {
                        characterBody.timedBuffs[i].timer -= reduction;
                        newDuration = Mathf.Max(newDuration, characterBody.timedBuffs[i].timer);
                    }
                }
            }
            if (newDuration == 0 && characterBody.GetBuffCount(buff) > 0)
            {
                return float.MaxValue;
            }
            return newDuration;
        }

        public static bool GetRandomNode(Vector3 origin, out Vector3 destination, float minDistance, float maxDistance, bool alwaysFindNode = true)
        {
            NodeGraph nodeGraph = SceneInfo.instance.GetNodeGraph(MapNodeGroup.GraphType.Ground);
            NodeGraph.NodeIndex nodeIndex = NodeGraph.NodeIndex.invalid;
            List<NodeGraph.NodeIndex> list;

            list = nodeGraph.FindNodesInRange(origin, minDistance, maxDistance, HullMask.Human);
            if (list.Count > 0)
            {
                nodeIndex = list[UnityEngine.Random.Range(0, list.Count)];
            }
            else if (alwaysFindNode)
            {
                if (Physics.Raycast(origin, Vector3.down, out var hit, float.PositiveInfinity, LayerIndex.world.intVal, QueryTriggerInteraction.UseGlobal))
                {
                    list = nodeGraph.FindNodesInRange(hit.point, minDistance, maxDistance, HullMask.Human);
                    if (list.Count > 0)
                    {
                        nodeIndex = list[UnityEngine.Random.Range(0, list.Count)];
                    }
                }
                else
                {
                    nodeIndex = nodeGraph.FindClosestNode(origin, HullClassification.Human);
                }
            }

            if (nodeIndex != NodeGraph.NodeIndex.invalid)
            {
                nodeGraph.GetNodePosition(nodeIndex, out Vector3 vector3);
                destination = vector3;
                return true;
            }
            destination = origin;
            return false;
        }

        public static int GetItemCountWithQuality(this Inventory inventory, ItemDef item)
        {
            int count = 0;
            if (!ElementalReactionsPlugin.qualityModExists)
            {
                count = inventory.GetItemCountEffective(item);
            }
            else
            {
                count = QualitySupport.GetItemCountTotalQuality(inventory, item);
            }
            if (item == Items.Items.moonWheel) count += inventory.GetItemCountEffective(Items.MoonWheel.hiddenMoonWheel);
            return count;
        }

        public static void RemoveDot(DotController dotController, DotController.DotIndex dot)
        {
            if (dotController && dot != DotController.DotIndex.None)
            {
                for (int i = 0; i < dotController.dotStackList.Count; i++)
                {
                    if (dotController.dotStackList[i].dotIndex == dot)
                    {
                        dotController.RemoveDotStackAtServer(i);
                    }
                }
            }
        }
        public static Quaternion RandomForwardRotation()
        {
            return Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up);
        }
    }
}
