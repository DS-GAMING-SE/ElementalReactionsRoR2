using BepInEx.Configuration;
using ElementalReactionsMod.Elements;
using RoR2;
using RoR2.ContentManagement;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using HG;

namespace ElementalReactionsMod.Loadout
{
    public class ElementLoadoutPanelController : MonoBehaviour
    {
        public LanguageTextMeshController hoverTextDescription;

        public UILayerKey requiredUILayerKey;

        public List<Row> rows;

        private MPEventSystemLocator eventSystemLocator;

        public BodyIndex bodyIndex;
        public string bodyName;
        public ElementLoadoutComponent bodyElementLoadout;

        private bool startRan;

        private void Awake()
        {
            this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
        }

        private void Start()
        {
            rows = [Row.FromSkillSlot(this, SkillSlot.Primary),
                Row.FromSkillSlot(this, SkillSlot.Secondary),
                Row.FromSkillSlot(this, SkillSlot.Utility),
                Row.FromSkillSlot(this, SkillSlot.Special)];
            startRan = true;
        }

        private void OnEnable()
        {
            if (startRan) UpdateDisplayData();
        }
        private void Update()
        {
            if (startRan) UpdateDisplayData();
        }

        private void UpdateDisplayData()
        {
            MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
            NetworkUser networkUser = null;
            if (eventSystem != null)
            {
                LocalUser localUser = eventSystem.localUser;
                if (localUser != null)
                {
                    networkUser = localUser.currentNetworkUser;
                }
            }
            BodyIndex bodyIndex = networkUser ? networkUser.bodyIndexPreference : BodyIndex.None;
            SetDisplayData(bodyIndex);
        }
        public void SetDisplayData(BodyIndex bodyIndex)
        {
            if (bodyIndex == this.bodyIndex)
            {
                return;
            }
            this.bodyIndex = bodyIndex;
            this.bodyName = BodyCatalog.GetBodyName(bodyIndex);
            this.bodyElementLoadout = BodyCatalog.GetBodyPrefab(bodyIndex).EnsureComponent<ElementLoadoutComponent>();
            this.Rebuild();
        }

        public void Rebuild()
        {
            foreach (var row in rows)
            {
                row.UpdateCharacter();
            }
        }

        private void OnDestroy()
        {
            for (int i = this.rows.Count - 1; i >= 0; i--)
            {
                this.rows[i].Dispose();
            }
            this.rows.Clear();
        }

        public class Row : IDisposable // ----------------------------------------------------------------------------------------
        {
            public ElementLoadoutPanelController owner;

            public RectTransform rowPanelTransform;

            public RectTransform buttonContainerTransform;

            public RectTransform choiceHighlightRect;

            public Color primaryColor;

            private List<RowData> rowData = new List<RowData>();

            public SkillSlot skillSlot;

            private Row(ElementLoadoutPanelController owner, string titleToken, SkillSlot skillSlot)
            {
                this.owner = owner;
                //this.rowPanelTransform = (RectTransform)GameObject.Instantiate(LoadoutPanelController.rowPrefab, (RectTransform)owner.transform).transform;
                this.rowPanelTransform = (RectTransform)GameObject.Instantiate(
                    AssetAsyncReferenceManager<GameObject>.LoadAsset(new UnityEngine.AddressableAssets.AssetReferenceT<GameObject>
                    (RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_UI.Row_prefab)).WaitForCompletion(), (RectTransform)owner.transform).transform;
                this.buttonContainerTransform = (RectTransform)rowPanelTransform.Find("ButtonContainer");
                this.choiceHighlightRect = (RectTransform)rowPanelTransform.Find("ButtonSelectionHighlight, Checkbox");
                this.rowPanelTransform.Find("SlotLabel").GetComponent<LanguageTextMeshController>().token = titleToken;
                this.skillSlot = skillSlot;
            }

            public static Row FromSkillSlot(ElementLoadoutPanelController owner, SkillSlot skillSlot)
            {
                string titleToken;
                switch(skillSlot)
                {
                    case SkillSlot.Primary:
                        titleToken = "LOADOUT_SKILL_PRIMARY";
                        break;
                    case SkillSlot.Secondary:
                        titleToken = "LOADOUT_SKILL_SECONDARY";
                        break;
                    case SkillSlot.Utility:
                        titleToken = "LOADOUT_SKILL_UTILITY";
                        break;
                    case SkillSlot.Special:
                        titleToken = "LOADOUT_SKILL_SPECIAL";
                        break;
                    default:
                        titleToken ="LOADOUT_SKILL_MISC";
                        break;
                }
                Row row = new Row(owner, titleToken, skillSlot);
                for (int i = 0; i < ElementCatalog.elementCatalog.Length; i++)
                {
                    int index = i;
                    row.AddButton(owner, ElementCatalog.elementCatalog[i]).onClick.AddListener(() => 
                    {
                        Config.SetElementLoadoutConfig(owner.bodyName, ElementCatalog.elementCatalog[index], skillSlot);
                        row.OnLoadoutChanged(null, null);
                    });
                }
                return row;
            }
            public void UpdateCharacter()
            {
                CharacterBody bodyPrefabBodyComponent = BodyCatalog.GetBodyPrefabBodyComponent(owner.bodyIndex);
                if (bodyPrefabBodyComponent != null)
                {
                    this.primaryColor = bodyPrefabBodyComponent.bodyColor;
                }
                this.rowPanelTransform.Find("SlotLabel").GetComponent<HGTextMeshProUGUI>().color = this.primaryColor;
                OnLoadoutChanged(null, null);
            }
            private void SetButtonColorMultiplier(int i, float f)
            {
                MPButton button = this.rowData[i].button;
                ColorBlock colors = button.colors;
                colors.colorMultiplier = f;
                button.colors = colors;
            }

            public void OnLoadoutChanged(object obj, EventArgs args)
            {
                ElementDef[] loadout = Config.GetElementLoadoutFromConfig(owner.bodyName, out _);
                if (owner.bodyElementLoadout)
                {
                    owner.bodyElementLoadout.ApplyElementLoadout(loadout);
                }
                for (int i = 0; i < this.rowData.Count; i++)
                {
                    ColorBlock colors = this.rowData[i].button.colors;
                    colors.colorMultiplier = 0.5f;
                    this.rowData[i].button.colors = colors;
                    this.SetButtonColorMultiplier(i, 0.5f);
                    if ((int)loadout[(int)skillSlot].index == this.rowData[i].defIndex)
                    {
                        this.choiceHighlightRect.SetParent((RectTransform)this.rowData[i].button.transform, false);
                        this.SetButtonColorMultiplier(i, 1f);
                    }
                }
            }

            private HGButton AddButton(ElementLoadoutPanelController owner, ElementDef element)
            {
                HGButton component = GameObject.Instantiate(AssetAsyncReferenceManager<GameObject>.LoadAsset(new UnityEngine.AddressableAssets.AssetReferenceT<GameObject>
                    (RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_UI.LoadoutButton_prefab)).WaitForCompletion(), this.buttonContainerTransform).GetComponent<HGButton>();
                component.updateTextOnHover = true;
                component.hoverLanguageTextMeshController = owner.hoverTextDescription;
                component.hoverToken = element.keywordToken;
                component.requiredTopLayer = owner.requiredUILayerKey;
                TooltipProvider component2 = component.GetComponent<TooltipProvider>();
                component.interactable = true;
                component2.titleColor = element.color;
                component2.overrideTitleText = Language.GetString(element.nameToken);
                component2.overrideBodyText = Language.GetString(element.descriptionToken);
                ((Image)component.targetGraphic).sprite = element.skillIcon;
                this.rowData.Add(new RowData(component, (int)element.index));
                return component;
            }

            public void Dispose()
            {
                for (int i = this.rowData.Count - 1; i >= 0; i--)
                {
                    GameObject.Destroy(this.rowData[i].button.gameObject);
                }
                GameObject.Destroy(this.rowPanelTransform.gameObject);
            }
            private struct RowData
            {
                public RowData(MPButton _button, int _defIndex)
                {
                    this.button = _button;
                    this.defIndex = _defIndex;
                }

                public MPButton button;

                public int defIndex;
            }
        }
    }
}
