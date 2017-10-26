using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eItemType { Consumable, StatBoost, Armour, Weapon, Rare, Money };
public enum eDifficulty { Easy, Medium, Hard, VeryHard, Hardest }

public class ItemFactory : MonoBehaviour
{
    public enum eRewardType { Money, Item, Nothing };

    [Header("Prefabs")]
    [SerializeField] private GameObject[] m_ItemPrefabs;

    [Header("Item Data")]
    [SerializeField] private ItemData[] m_ConsumableData;
    [SerializeField] private ItemData[] m_StatBoostData;
    [SerializeField] private ItemData[] m_ArmourData;
    [SerializeField] private ItemData[] m_WeaponData;

    [Header("Probabilities")]
    [SerializeField] private int[] m_ItemTypeProbabilities = new int[4]; // only consumable, stat boost, armour, and weapon
    [SerializeField] private int[] m_ItemTypeFromCrateProbabilities = new int[3]; // the chance of money, item, or nothing from crates
    [SerializeField] private int[] m_ItemTypeFromEnemyProbabilities = new int[3]; // the chance of money, item, or nothing from enemies

    [Header("Other")]
    [SerializeField] private float[] m_BaseMoneyAmountByDiff = new float[5];

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.RequestItemSpawnEvent>(OnItemRequested);
    }

    private void OnItemRequested(GameEvents.RequestItemSpawnEvent e)
    {
        // TODO get these from somewhere global
        eLevelTheme currentTheme = eLevelTheme.Forest;
        eDifficulty difficulty = eDifficulty.Easy;

        eRewardType reward = eRewardType.Nothing;
        switch (e.ItemSource)
        {
            case eItemSource.Crate:
                reward = (eRewardType)Utils.WeightedRandom(m_ItemTypeFromCrateProbabilities);
                break;

            case eItemSource.Enemy:
                reward = (eRewardType)Utils.WeightedRandom(m_ItemTypeFromEnemyProbabilities);
                break;
        }

        switch (reward)
        {
            case eRewardType.Money:
                RewardMoney(difficulty, e.ItemSource, e.CrateType, e.EnemyType, e.SpawnPosition);
                break;

            case eRewardType.Item:
                RewardItem(currentTheme, difficulty, e.ItemSource, e.CrateType, e.EnemyType, e.SpawnPosition);
                break;

            default:
            case eRewardType.Nothing:
                break;
        }
    }

    private void RewardMoney(eDifficulty diff, eItemSource source, eCrateType cType, eEnemyType eType, Vector3 spawnPos)
    {
        int difficulty = (int)diff;
        float baseAmount = m_BaseMoneyAmountByDiff[difficulty];

        float diffVariableModifier = (difficulty + 1) * 10f;
        float variableAmount = UnityEngine.Random.Range(-5f, 5f) * diffVariableModifier;

        ItemData data = new ItemData();
        data.m_ItemType = eItemType.Money;
        data.m_ItemValue = baseAmount + variableAmount;

        SpawnItemObject(eItemType.Money, data, spawnPos);
    }

    private void RewardItem(eLevelTheme theme, eDifficulty diff, eItemSource source, eCrateType cType, eEnemyType eType, Vector3 spawnPos)
    {
        ItemData[] currentItems = null;
        eItemType itemType = (eItemType)Utils.WeightedRandom(m_ItemTypeProbabilities);
        switch (itemType)
        {
            case eItemType.Consumable:
                currentItems = m_ConsumableData;
                break;

            case eItemType.StatBoost:
                currentItems = m_StatBoostData;
                break;

            case eItemType.Armour:
                currentItems = m_ArmourData;
                break;

            case eItemType.Weapon:
                currentItems = m_WeaponData;
                break;
        }

        // gather the probabilties of all items
        int[] itemProbabilities = new int[currentItems.Length];
        for (int i = 0; i < currentItems.Length; i++)
        {
            itemProbabilities[i] = currentItems[i].m_ProababilityByDifficulty[(int)diff];
        }

        int itemDataIndex = Utils.WeightedRandom(itemProbabilities);
        ItemData data = currentItems[itemDataIndex];

        SpawnItemObject(itemType, data, spawnPos);
    }

    private void SpawnItemObject(eItemType type, ItemData data, Vector3 spawnPos)
    {
        GameObject itemObj = (GameObject)Instantiate(m_ItemPrefabs[(int)type], spawnPos, Quaternion.identity);
        Item item = itemObj.GetComponent<Item>();
        item.SetData(data);

        // for fun!
        Rigidbody rb = itemObj.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 7f, ForceMode.Impulse);
        rb.AddTorque(itemObj.transform.right * 2f, ForceMode.Impulse);
    }

    //private void RetrieveCrateItem(eDifficulty diff, eLevelTheme theme, eCrateType type, Vector3 spawnPos)
    //{
    //    List<ItemData> themedItems = GetItemsForTheme(theme);
    //    themedItems.RemoveAll(x => !x.m_Sources.Contains(eItemSource.Crate) && !x.m_CrateTypes.Contains(type));

    //    SelectItemToSpawn(themedItems, spawnPos);
    //}

    //private void RetrieveEnemyItem(eDifficulty diff, eLevelTheme theme, eEnemyType type, Vector3 spawnPos)
    //{
    //    List<ItemData> themedItems = GetItemsForTheme(theme);
    //    themedItems.RemoveAll(x => !x.m_Sources.Contains(eItemSource.Enemy) && !x.m_EnemyTypes.Contains(type));

    //    SelectItemToSpawn(themedItems, spawnPos);
    //}

    //private void SelectItemToSpawn(List<ItemData> candidates, Vector3 spawnPos)
    //{
    //    eItemType randType = (eItemType)(UnityEngine.Random.Range(0, 3));

    //    eRarity randRarity;
    //    float rarityChance = UnityEngine.Random.Range(0f, 1f);
    //    if (rarityChance < 0.53f)
    //    {
    //        randRarity = eRarity.Common;
    //    }
    //    else if (rarityChance < 0.78f)
    //    {
    //        randRarity = eRarity.Uncommon;
    //    }
    //    else if (rarityChance < 0.9f)
    //    {
    //        randRarity = eRarity.Special;
    //    }
    //    else if (rarityChance < 0.96f)
    //    {
    //        randRarity = eRarity.Unique;
    //    }
    //    else
    //    {
    //        randRarity = eRarity.OneOfAKind;
    //    }

    //    candidates.RemoveAll(x => x.m_ItemType != randType && x.m_ItemRarity != randRarity);

    //    if (candidates.Count > 0)
    //    {
    //        int finalIndex = UnityEngine.Random.Range(0, candidates.Count);
    //        ItemData data = candidates[finalIndex];

    //        GameObject itemObj = (GameObject)Instantiate(m_ItemPrefabs[(int)randType], spawnPos, Quaternion.identity);
    //        Item item = itemObj.GetComponent<Item>();
    //        item.SetData(data);

    //        // for fun!
    //        Rigidbody rb = itemObj.GetComponent<Rigidbody>();
    //        rb.AddForce(Vector3.up * 7f, ForceMode.Impulse);
    //        rb.AddTorque(itemObj.transform.right * 2f, ForceMode.Impulse);
    //    }
    //    else
    //    {
    //        Debug.Log("Drop Money?");
    //    }
    //}

    //private List<ItemData> GetItemsForTheme(eLevelTheme theme)
    //{
    //    List<ItemData> subset = new List<ItemData>();
    //    //subset.AddRange(m_ItemData);
    //    subset.RemoveAll(x => !x.m_Themes.Contains(theme));

    //    return subset;
    //}
}
