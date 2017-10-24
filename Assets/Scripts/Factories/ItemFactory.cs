using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eItemType { Consumable, Armour, Weapon };

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private GameObject[] m_ItemPrefabs;
    [SerializeField] private ItemData[] m_ItemData;

    private void Awake()
    {
        VSEventManager.Instance.AddListener<GameEvents.RequestItemSpawnEvent>(OnItemRequested);
    }

    private void OnItemRequested(GameEvents.RequestItemSpawnEvent e)
    {
        // TODO
        // get the theme from somewhere. Either GameManager or DungeonBuilder

        eLevelTheme currentTheme = eLevelTheme.Forest;
        eItemSource sourceType = e.ItemSource;
        if (sourceType == eItemSource.Crate)
        {
            RetrieveCrateItem(currentTheme, e.CrateType, e.SpawnPosition);
        }
        else if (sourceType == eItemSource.Enemy)
        {
            RetrieveEnemyItem(currentTheme, e.EnemyType, e.SpawnPosition);
        }
    }

    private void RetrieveCrateItem(eLevelTheme theme, eCrateType type, Vector3 spawnPos)
    {
        List<ItemData> themedItems = GetItemsForTheme(theme);
        themedItems.RemoveAll(x => !x.m_Sources.Contains(eItemSource.Crate) && !x.m_CrateTypes.Contains(type));

        SelectItemToSpawn(themedItems, spawnPos);
    }

    private void RetrieveEnemyItem(eLevelTheme theme, eEnemyType type, Vector3 spawnPos)
    {
        List<ItemData> themedItems = GetItemsForTheme(theme);
        themedItems.RemoveAll(x => !x.m_Sources.Contains(eItemSource.Enemy) && !x.m_EnemyTypes.Contains(type));

        SelectItemToSpawn(themedItems, spawnPos);
    }

    private void SelectItemToSpawn(List<ItemData> candidates, Vector3 spawnPos)
    {
        eItemType randType = (eItemType)(UnityEngine.Random.Range(0, 3));

        eRarity randRarity;
        float rarityChance = UnityEngine.Random.Range(0f, 1f);
        if (rarityChance < 0.53f)
        {
            randRarity = eRarity.Common;
        }
        else if (rarityChance < 0.78f)
        {
            randRarity = eRarity.Uncommon;
        }
        else if (rarityChance < 0.9f)
        {
            randRarity = eRarity.Special;
        }
        else if (rarityChance < 0.96f)
        {
            randRarity = eRarity.Unique;
        }
        else
        {
            randRarity = eRarity.OneOfAKind;
        }

        candidates.RemoveAll(x => x.m_ItemType != randType && x.m_ItemRarity != randRarity);

        if (candidates.Count > 0)
        {
            int finalIndex = UnityEngine.Random.Range(0, candidates.Count);
            ItemData data = candidates[finalIndex];

            GameObject itemObj = (GameObject)Instantiate(m_ItemPrefabs[(int)randType], spawnPos, Quaternion.identity);
            Item item = itemObj.GetComponent<Item>();
            item.SetData(data);
        }
        else
        {
            Debug.Log("Drop Money?");
        }
    }

    private List<ItemData> GetItemsForTheme(eLevelTheme theme)
    {
        List<ItemData> subset = new List<ItemData>();
        subset.AddRange(m_ItemData);
        subset.RemoveAll(x => !x.m_Themes.Contains(theme));

        return subset;
    }
}
