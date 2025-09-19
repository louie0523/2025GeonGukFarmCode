using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public enum Item
{
    CornSeed,
    CarrotSeed,
    CfloweSeed,
    BroccliSeed,
    GoldCarrotSeed,
    Corn,
    Carrot,
    Cflower,
    Broccoil,
    GoldCarrot,
};


public class Storage : MonoBehaviour
{
    public static Storage Instance;
    public List<Item> itemSlot = new List<Item>();
    public List<GameObject> ItemObjectList = new List<GameObject>();
    public GameObject ItemThrowPoint;
    public GameObject RealObj;

    public int testItem = 30;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.isStorageIn)
        {
            RealObj.SetActive(true);
        } else
        {
            RealObj.SetActive(false);
        }
    }

    public void TestSet()
    {
        itemSlot.Clear();
        for (int i = 0; i < testItem; i++)
        {
            int rand = Random.Range(0, 10);
            itemSlot.Add((Item)rand);
        }
    }

    public IEnumerator ShowItem()
    {
        foreach(Transform obj in ItemThrowPoint.transform)
        {
            Destroy(obj.gameObject);
        }

        for(int i = 0; i < itemSlot.Count; i++)
        {
            int x = Random.Range(-5, 6); int z = Random.Range(-5, 6);
            GameObject itemOb = Instantiate(ItemObjectList[(int)itemSlot[i]], ItemThrowPoint.transform.position + new Vector3Int(x, 0, z), Quaternion.identity, ItemThrowPoint.transform);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SellAllCrop()
    {
        List<Item> Crop = new List<Item>();

        for (int i = itemSlot.Count - 1; i >= 0; i--)
        {
            if ((int)itemSlot[i] > 4)
            {
                Crop.Add(itemSlot[i]);
                itemSlot.RemoveAt(i);
            }
        }
        if (Crop.Count <= 0)
        {
            SfxManager.instance.SoundPlay("���");
            UIManager.instance.Message2("���� â�� �۹��� �����մϴ�.");
            Debug.Log("���� â�� �۹��� �����մϴ�.");
            return;
        }

        string groptext = "";

        int SellMoeny = 0;
        int[] CropGsCount = new int[5]
        {
            0,0,0,0,0
        };
        for (int i = 0; i < Crop.Count; i++)
        {
            SellMoeny += GameManager.Instance.CurrentCropPrice[(int)Crop[i] - 5];
            CropGsCount[(int)Crop[i] - 5]++;
        }

        for (int i = 0; i < CropGsCount.Length; i++)
        {
            if (CropGsCount[i] > 0)
            {
                switch (i)
                {
                    case 0:
                        groptext += $"������ X {CropGsCount[i]}\n";
                        break;
                    case 1:
                        groptext += $"��� X {CropGsCount[i]}\n";
                        break;
                    case 2:
                        groptext += $"����� X {CropGsCount[i]}\n";
                        break;
                    case 3:
                        groptext += $"����ݸ� X {CropGsCount[i]}\n";
                        break;
                    case 4:
                        groptext += $"Ȳ�ݴ�� X {CropGsCount[i]}\n";
                        break;
                }
            }
        }

        string endtext = $"���� : {SellMoeny}";

        SfxManager.instance.SoundPlay("�Ǹ�");

        GameManager.Instance.Money += SellMoeny;
        StartCoroutine(ShowItem());
        StartCoroutine(UIManager.instance.GaySanGO(groptext, endtext));


    }

    public void SeedGetUIOpen()
    {
        if (UIManager.instance.SeedGetUI.activeSelf)
        {
            UIManager.instance.SeedGetUI.SetActive(false);

        } else
        {
            UIManager.instance.SeedGetUI.SetActive(true);
            UIManager.instance.StotreObjUI.SetActive(false);
            SeedTextSet();
        }
    }

    public void ShopOpen()
    {
        if (UIManager.instance.StotreObjUI.activeSelf)
        {
            UIManager.instance.StotreObjUI.SetActive(false);

        }
        else
        {
            UIManager.instance.StotreObjUI.SetActive(true);
            UIManager.instance.SeedGetUI.SetActive(false);

        }
    }

    public void SeedTextSet()
    {
        int[] Seeds = new int[5];

        for (int i = 0; i < itemSlot.Count; i++)
        {
            if ((int)itemSlot[i] <= 4)
            {
                Seeds[(int)itemSlot[i]]++;
            }
        }



        for (int i = 0; i < 5; i++)
        {
            UIManager.instance.SeedGetTexts[i].text = Seeds[i].ToString();
        }
    }

    public void SeedGet(int num)
    {
        List<Item> Crop = new List<Item>();

        for (int i = itemSlot.Count - 1; i >= 0; i--)
        {
            if ((int)itemSlot[i] == num)
            {
                Crop.Add(itemSlot[i]);
            }
        }

        if (Crop.Count <= 0)
        {
            SfxManager.instance.SoundPlay("���");
            UIManager.instance.Message2("���� â�� �ش� ������ �����մϴ�.");
            return;
        }

        for (int i = itemSlot.Count - 1; i >= 0; i--)
        {
            if ((int)itemSlot[i] == num)
            {
                itemSlot.RemoveAt(i);
                break;
            }
        }
        SfxManager.instance.SoundPlay("���");
        GameManager.Instance.player.SeedCount[num]++;
        StartCoroutine(ShowItem());
        SeedTextSet();
    }

    public void StorageUpgrade()
    {
        if(GameManager.Instance.MaxStorage == 80)
        {
            SfxManager.instance.SoundPlay("���");
            UIManager.instance.Message2("�̹� â�� �ְ� �������� ���׷��̵� �Ǿ����ϴ�.");
            return;
        }

        if(GameManager.Instance.Money >= GameManager.Instance.StorageUpgradePrice)
        {
            UIManager.instance.Message2("â�� ���׷��̵� �Ͽ����ϴ�!");
            GameManager.Instance.Money -= GameManager.Instance.StorageUpgradePrice;
            GameManager.Instance.MaxStorage = GameManager.Instance.MaxStorage  == 20 ? 40 : 80;
            SfxManager.instance.SoundPlay("����");
        } else
        {
            SfxManager.instance.SoundPlay("���");
            UIManager.instance.Message2("���� �����մϴ�.");
        }
    }

}
