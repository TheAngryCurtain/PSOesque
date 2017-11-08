using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemProbability
{
    public ItemData Data;
    public int Probability;
}

[System.Serializable]
public class SerializableIntArray
{
    public int[] Values = new int[3]; // money, item, or nothing
}

public class ItemFactory : MonoBehaviour
{
    public enum eRewardType { Money, Item, Nothing };

    [Header("Prefabs")]
    [SerializeField] private GameObject[] m_ItemPrefabs;

    [Header("Source Probabilities")]
    [SerializeField] private SerializableIntArray[] m_ItemTypeCrateProbabilities = new SerializableIntArray[3]; // common, rare, very rare crates
    [SerializeField] private SerializableIntArray[] m_ItemTypeEnemyProbabilities = new SerializableIntArray[5]; // weak, regular, tough, vTough, boss enemies

    [Header("Money")]
    [SerializeField] private float[] m_BaseMoneyAmountByDiff = new float[5];

    [Header("Item Data by Source")]
    [Header("Crate")]
    [SerializeField] private ItemProbability[] m_CommonCrateItems;
    [SerializeField] private ItemProbability[] m_RareCrateItems;
    [SerializeField] private ItemProbability[] m_VeryRareCrateItems;

    [Header("Enemy")]
    [SerializeField] private ItemProbability[] m_WeakEnemyItems;
    [SerializeField] private ItemProbability[] m_RegularEnemyItems;
    [SerializeField] private ItemProbability[] m_ToughEnemyItems;
    [SerializeField] private ItemProbability[] m_VeryToughEnemyItems;
    [SerializeField] private ItemProbability[] m_BossEnemyItems;

    [Header("Misc.")]
    [SerializeField] private GameObject m_ItemSpawnParticleObj;
    [SerializeField] private GameObject m_ItemMarkerObj;

    private ParticleSystem m_ItemSpawnEffect;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.RequestItemSpawnEvent>(OnItemRequested);

        GameObject particleObj = (GameObject)Instantiate(m_ItemSpawnParticleObj, null);
        m_ItemSpawnEffect = particleObj.GetComponentInChildren<ParticleSystem>();
    }

    private void OnItemRequested(GameEvents.RequestItemSpawnEvent e)
    {
        // TODO get these from somewhere global
        Enums.eLevelTheme currentTheme = Enums.eLevelTheme.Forest;
        Enums.eDifficulty difficulty = Enums.eDifficulty.Easy;

        int[] sourceProbabilities = null;
        switch (e.ItemSource)
        {
            case Enums.eItemSource.Crate:
                sourceProbabilities = m_ItemTypeCrateProbabilities[(int)e.CrateType].Values;
                break;

            case Enums.eItemSource.Enemy:
                sourceProbabilities = m_ItemTypeEnemyProbabilities[(int)e.EnemyType].Values;
                break;
        }

        // determine reward
        eRewardType reward = eRewardType.Nothing;

        int[] probabilities = new int[sourceProbabilities.Length];
        for (int i = 0; i < sourceProbabilities.Length; i++)
        {
            probabilities[i] = sourceProbabilities[i];
        }
        reward = (eRewardType)Utils.WeightedRandom(probabilities);

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

    private void RewardMoney(Enums.eDifficulty diff, Enums.eItemSource source, Enums.eCrateType cType, Enums.eEnemyType eType, Vector3 spawnPos)
    {
        int difficulty = (int)diff;
        float baseAmount = m_BaseMoneyAmountByDiff[difficulty];

        int diffVariableModifier = difficulty * 10;
        int variableAmount = UnityEngine.Random.Range(-5, 5 + 1) + diffVariableModifier;
        if (variableAmount < 0 && Mathf.Abs(variableAmount) >= baseAmount)
        {
            // will end up being negative or zero
            variableAmount *= -1;
        }

        ItemData data = ScriptableObject.CreateInstance<ItemData>();
        data.m_ItemType = Enums.eItemType.Money;
        data.m_ItemValue = baseAmount + variableAmount;

        SpawnItemObject(Enums.eItemType.Money, data, spawnPos);
    }

    private void RewardItem(Enums.eLevelTheme theme, Enums.eDifficulty diff, Enums.eItemSource source, Enums.eCrateType cType, Enums.eEnemyType eType, Vector3 spawnPos)
    {
        ItemProbability[] itemProbs = null;
        if (source == Enums.eItemSource.Crate)
        {
            switch (cType)
            {
                case Enums.eCrateType.Common:
                    itemProbs = m_CommonCrateItems;
                    break;

                case Enums.eCrateType.Rare:
                    itemProbs = m_RareCrateItems;
                    break;

                case Enums.eCrateType.VeryRare:
                    itemProbs = m_VeryRareCrateItems;
                    break;
            }
        }
        else if (source == Enums.eItemSource.Enemy)
        {
            switch (eType)
            {
                case Enums.eEnemyType.Weak:
                    itemProbs = m_WeakEnemyItems;
                    break;

                case Enums.eEnemyType.Regular:
                    itemProbs = m_RegularEnemyItems;
                    break;

                case Enums.eEnemyType.Tough:
                    itemProbs = m_ToughEnemyItems;
                    break;

                case Enums.eEnemyType.ExtraTough:
                    itemProbs = m_VeryToughEnemyItems;
                    break;

                case Enums.eEnemyType.Boss:
                    itemProbs = m_BossEnemyItems;
                    break;
            }
        }

        int[] probabilities = new int[itemProbs.Length];
        for (int i = 0; i < itemProbs.Length; i++)
        {
            // theme/difficulty check
            if ((itemProbs[i].Data.m_Themes[0] != Enums.eLevelTheme.All && !itemProbs[i].Data.m_Themes.Contains(theme)) ||
                (itemProbs[i].Data.m_Difficulties[0] != Enums.eDifficulty.All && !itemProbs[i].Data.m_Difficulties.Contains(diff)))
            {
                itemProbs[i].Probability = 0;
            }

            probabilities[i] = itemProbs[i].Probability;
        }

        int dataIndex = Utils.WeightedRandom(probabilities);
        ItemData data = itemProbs[dataIndex].Data;

        SpawnItemObject(data.m_ItemType, data, spawnPos);

        //Enums.eItemType itemType = (Enums.eItemType)Utils.WeightedRandom(m_ItemTypeProbabilities);
        //switch (itemType)
        //{
        //    case Enums.eItemType.Consumable:
        //        currentItems = m_ConsumableData;
        //        break;

        //    case Enums.eItemType.StatBoost:
        //        currentItems = m_StatBoostData;
        //        break;

        //    case Enums.eItemType.Armour:
        //        currentItems = m_ArmourData;
        //        break;

        //    case Enums.eItemType.Weapon:
        //        currentItems = m_WeaponData;
        //        break;
        //}

        //// gather the probabilties of all items
        //int[] itemProbabilities = new int[currentItems.Length];
        //for (int i = 0; i < currentItems.Length; i++)
        //{
        //    itemProbabilities[i] = currentItems[i].m_ProababilityByDifficulty[(int)diff];
        //}

        //int itemDataIndex = Utils.WeightedRandom(itemProbabilities);
        //ItemData data = currentItems[itemDataIndex];

        //SpawnItemObject(itemType, data, spawnPos);
    }

    private void SpawnItemObject(Enums.eItemType type, ItemData data, Vector3 spawnPos)
    {
        GameObject itemObj = (GameObject)Instantiate(m_ItemPrefabs[(int)type], spawnPos, Quaternion.identity);
        Item item = itemObj.GetComponent<Item>();
        item.SetData(data);

        // play particles
        m_ItemSpawnEffect.transform.parent.gameObject.transform.position = spawnPos; // this is truely horrible
        m_ItemSpawnEffect.Play();

        // put down a marker
        GameObject markerObj = (GameObject)Instantiate(m_ItemMarkerObj, null);
        item.AssignMarkerObject(markerObj.transform);

        // for fun!
        Rigidbody rb = itemObj.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 7f, ForceMode.Impulse);
        rb.AddTorque(itemObj.transform.right * 2f, ForceMode.Impulse);
    }

    //private void RetrieveCrateItem(Enums.eDifficulty diff, Enums.eLevelTheme theme, Enums.eCrateType type, Vector3 spawnPos)
    //{
    //    List<ItemData> themedItems = GetItemsForTheme(theme);
    //    themedItems.RemoveAll(x => !x.m_Sources.Contains(Enums.eItemSource.Crate) && !x.m_CrateTypes.Contains(type));

    //    SelectItemToSpawn(themedItems, spawnPos);
    //}

    //private void RetrieveEnemyItem(Enums.eDifficulty diff, Enums.eLevelTheme theme, Enums.eEnemyType type, Vector3 spawnPos)
    //{
    //    List<ItemData> themedItems = GetItemsForTheme(theme);
    //    themedItems.RemoveAll(x => !x.m_Sources.Contains(Enums.eItemSource.Enemy) && !x.m_EnemyTypes.Contains(type));

    //    SelectItemToSpawn(themedItems, spawnPos);
    //}

    //private void SelectItemToSpawn(List<ItemData> candidates, Vector3 spawnPos)
    //{
    //    Enums.eItemType randType = (Enums.eItemType)(UnityEngine.Random.Range(0, 3));

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

    //private List<ItemData> GetItemsForTheme(Enums.eLevelTheme theme)
    //{
    //    List<ItemData> subset = new List<ItemData>();
    //    //subset.AddRange(m_ItemData);
    //    subset.RemoveAll(x => !x.m_Themes.Contains(theme));

    //    return subset;
    //}
}
