using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ElementalReactionsMod.Util;
using ElementalReactionsMod;
using ElementalReactionsMod.Elements;

namespace ElementalReactionsMod.Reactions
{
    public class BloomController : MonoBehaviour, IOnIncomingDamageServerReceiver
    {
        ElementalReactionPooledObject pool;
        private float timer;
        public void Start()
        {
            pool = GetComponent<ElementalReactionPooledObject>();
        }
        public void OnIncomingDamageServer(DamageInfo damageInfo)
        {
            ElementIndex element = damageInfo.damageType.GetElement();
            if (element == DefaultElementDefs.pyroElement.index)
            {
                Chat.AddMessage("burgeon");
                if (pool)
                {
                    pool.ReturnObject();
                }
                else
                {
                    GameObject.Destroy(base.gameObject);
                }
            }
            else if (element == DefaultElementDefs.electroElement.index)
            {
                Chat.AddMessage("hyperbloom");
                if (pool)
                {
                    pool.ReturnObject();
                }
                else
                {
                    GameObject.Destroy(base.gameObject);
                }
            }
            damageInfo.rejected = true;
        }
        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer > StaticValues.bloomDuration)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
