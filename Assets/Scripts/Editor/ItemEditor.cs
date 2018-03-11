using UnityEngine;
using UnityEditor;

public class ItemEditor : EditorWindow
{
    private static int itemID = 0;

    private string newItemName = "New Item";
    private Texture2D newItemIcon;
    private string newItemDesc = "New Description";
    private Enums.eItemType newItemType;
    private int newItemValue = 0;

    // consumable
    private Enums.eConsumableType newConsumableType;
    private Enums.eConsumableStatType newRecoveryStatType;
    private int recoveryAmount;
    private Enums.eStatType newStatBoostType;
    private int statBoostAmount;
    private Enums.eStatusEffect newEffectType;
    private int weaponUpgradeAmount;

    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    [MenuItem("Tools/Item Editor")]
    static void Init()
    {
        ItemEditor window = (ItemEditor)EditorWindow.GetWindow(typeof(ItemEditor));
        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 400f, 600f);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Create Item", EditorStyles.boldLabel);
        newItemIcon = (Texture2D)EditorGUILayout.ObjectField("Icon", newItemIcon, typeof(Texture2D), false);
        newItemName = EditorGUILayout.TextField("Name: ", newItemName);

        EditorGUILayout.PrefixLabel("Description");
        newItemDesc = EditorGUILayout.TextArea(newItemDesc, GUILayout.MinHeight(60f));
        newItemValue = EditorGUILayout.IntField("Worth: ", newItemValue);

        GUILayout.Label("Item Type", EditorStyles.boldLabel);
        newItemType = (Enums.eItemType)EditorGUILayout.EnumPopup("Type: ", newItemType);

        GUILayout.Label("Sub-Type", EditorStyles.boldLabel);
        switch (newItemType)
        {
            #region Consumables
            case Enums.eItemType.Consumable:
                {
                    newConsumableType = (Enums.eConsumableType)EditorGUILayout.EnumPopup("Consumable: ", newConsumableType);
                    switch (newConsumableType)
                    {
                        case Enums.eConsumableType.Recovery:
                            {
                                newRecoveryStatType = (Enums.eConsumableStatType)EditorGUILayout.EnumPopup("Stat Type: ", newRecoveryStatType);
                                recoveryAmount = EditorGUILayout.IntField("Amount: ", recoveryAmount);
                            }
                            break;

                        case Enums.eConsumableType.StatUpgrade:
                            {
                                newStatBoostType = (Enums.eStatType)EditorGUILayout.EnumPopup("Boost Stat: ", newStatBoostType);
                                statBoostAmount = EditorGUILayout.IntField("Amount: ", statBoostAmount);
                            }
                            break;

                        case Enums.eConsumableType.StatusEffect:
                            {
                                newEffectType = (Enums.eStatusEffect)EditorGUILayout.EnumPopup("Effect: ", newEffectType);
                            }
                            break;

                        case Enums.eConsumableType.WeaponUpgrade:
                            {
                                weaponUpgradeAmount = EditorGUILayout.IntField("Amount: ", weaponUpgradeAmount);
                            }
                            break;
                    }
                }
                break;
            #endregion

            #region Stat Boost
            case Enums.eItemType.StatBoost:
                {

                }
                break;
            #endregion

            #region Armour
            case Enums.eItemType.Armour:
                {

                }
                break;
            #endregion

            #region Weapon
            case Enums.eItemType.Weapon:
                {

                }
                break;
            #endregion

            #region Rare
            case Enums.eItemType.Rare:
                {

                }
                break;
            #endregion

            case Enums.eItemType.Money:
            default:
                break;
        }

        if (GUILayout.Button("Create Item"))
        {

        }

        //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //myBool = EditorGUILayout.Toggle("Toggle", myBool);
        //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        //EditorGUILayout.EndToggleGroup();
    }
}
