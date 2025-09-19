using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class isFarmGridVecter
{
    public int x, z;

    public isFarmGridVecter(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}

public enum Crop
{
    None = 0,
    Corn = 1,
    Carrot = 2,
    Cflower = 3,
    Broccoil = 4,
    GoldCarrot = 5,
}


[System.Serializable]
public class GorwConditon
{
    [Range(0, 100)]
    public int waterlyMin, waterlyMax;
    public bool Clean = true;
    public bool Cloudy = false;
    public bool Rain = false;
    public bool Storm = false;
    public bool Hail = false;
    [Range(0, 24)]
    public int StartTime;
    [Range(0, 24)]
    public int EndTIme;
    public int GrowTime;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player player;

    public float Money = 3000;
    public int tool = 0;
    public int water_Can = 0;
    public int currentWater = 0;
    public int waterUp = 20;
    public int MaxWater = 3;
    public int Farmingtool = 0;
    public int MaxFarmingCrop = 1;
    public int MaxStorage = 20;
    public int SeedRand = 20;
    public int StorageUpgradePrice = 50000;
    public List<GorwConditon> GorwConditons = new List<GorwConditon>();
    public List<Tile> tiles = new List<Tile>();
    public GameObject TileParent;
    public int Day = 1;
    public bool Nets = false;
    public bool isStorageIn = false;
    public List<int> CropPriceMin = new List<int>()
    {
        2000, 6000, 12000, 16000, 20000
    };
    public List<int> CropPriceMax = new List<int>()
    {
        4000, 12000, 24000, 32000, 40000
    };
    public List<int> CurrentCropPrice = new List<int>()
    {
        2000, 6000, 12000, 16000, 20000
    };
    public List<int> ListCropPrice = new List<int>()
    {
        2000, 6000, 12000, 16000, 20000
    };
    public List<int> SeedPrice = new List<int>()
    {
        3000, 10000, 16000, 28000, 30000
    };

    public float NextTilePrice = 20000;
    public bool isFristTile = true;
    [SerializeField]
    public List<isFarmGridVecter> isFarmGrids = new List<isFarmGridVecter>();
    public bool tileAct = false;
    public int CurrentAnimals;
  

    public GameObject StorageCam;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {

        if (isFristTile)
            tileAct = true;
        else
            tileAct = isBuyTrueTIle();
        

        if(!player.isFarm && Money >= NextTilePrice && tileAct && Input.GetKeyDown(KeyCode.Q))
        {
            SfxManager.instance.SoundPlay("땅구매");
            BuyTile();
        }

        EnterTheStorage();


        
        UIManager.instance.UpgradeObj[2].SetActive(Nets);


    }

    public void SetCropPrice()
    {
        Debug.Log("작물 가격이 재조정됩니다.");
        for(int i = 0; i < 5; i++)
        {
            int rand = Random.Range(-1000, 2001);
            ListCropPrice[i] = rand / CurrentCropPrice[i];
            CurrentCropPrice[i] += rand;
            if (CurrentCropPrice[i] > CropPriceMax[i])
            {
                CurrentCropPrice[i] = CropPriceMax[i];
            } else if (CurrentCropPrice[i] <  CropPriceMin[i])
            {
                CurrentCropPrice[i] = CropPriceMin[i];
            }
        }
    }

    bool isBuyTrueTIle()
    {
        if(isFarmGrids.Count < 1)
            return false;

        for(int i = 0; i <  isFarmGrids.Count; i++)
        {
            Transform transform = player.CurrentGrid.Tile.transform;
            if(transform.localPosition.x == isFarmGrids[i].x && transform.localPosition.z == isFarmGrids[i].z)
            {
                //Debug.Log("확장 가능한 타일");
                return true;

            }
        }

        return false;
    }

    

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        SetCropPrice();
    }

    public void BuyTile()
    {
        if (isFristTile)
            isFristTile = false;

        Money -= NextTilePrice;
        NextTilePrice *= 1.5f;
        player.CurrentGrid.Tile.ToFarm();
        player.SetCurrentTilePostion();
        Transform tile = player.CurrentGrid.Tile.transform;
        isFarmGrids.Add(new isFarmGridVecter((int)tile.localPosition.x - 6, (int)tile.localPosition.z));
        isFarmGrids.Add(new isFarmGridVecter((int)tile.localPosition.x + 6, (int)tile.localPosition.z));
        isFarmGrids.Add(new isFarmGridVecter((int)tile.localPosition.x, (int)tile.localPosition.z - 6));
        isFarmGrids.Add(new isFarmGridVecter((int)tile.localPosition.x, (int)tile.localPosition.z + 6));
        EqulatListSet();
    }

    void EqulatListSet()
    {
        isFarmGrids = isFarmGrids.GroupBy(g => new { g.x, g.z }).Select(g => g.First()).ToList();
    }

    public void EnterTheStorage()
    {
        if(Input.GetKeyDown(KeyCode.F) && !isStorageIn && StorageEnter.Instance.StorageEnterTrue)
        {
            StorageCam.SetActive(true);
            StartCoroutine(Storage.Instance.ShowItem());
            isStorageIn = true;
        } else if(Input.GetKeyDown(KeyCode.F) && isStorageIn)
        {
            StorageCam.SetActive(false);
            isStorageIn = false;
        }
    }

    public void ExitStorage()
    {
        StorageCam.SetActive(false);
        isStorageIn = false;
    }

    public void SeedBuy(int num)
    {
        if(Money < SeedPrice[num])
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message2("돈이 부족합니다.");
            return;
        }

        if(Storage.Instance.itemSlot.Count >= MaxStorage)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message2("창고에 공간이 부족합니다.");
            return;
        }

        Money -= SeedPrice[num];
        Storage.Instance.itemSlot.Add((Item)num);
        SfxManager.instance.SoundPlay("구매");
        StartCoroutine(Storage.Instance.ShowItem());
    }

    public void ObjUpgrade(int num)
    {
        if(num == 0) { 
            if(Money < 25000)
            {
                SfxManager.instance.SoundPlay("취소");
                UIManager.instance.Message2("돈이 부족합니다.");
                return;
            }

            if(water_Can != 0)
            {
                SfxManager.instance.SoundPlay("취소");
                UIManager.instance.Message2("이미 최대 레벨입니다.");
                return;
            }

            Money -= 25000;
            water_Can = 1;
            MaxWater = 20;
            waterUp = 20;
            SfxManager.instance.SoundPlay("구매");
            UIManager.instance.UpgradeObj[num].SetActive(true);
        } else if(num == 1)
        {
            if (Money < 25000)
            {
                SfxManager.instance.SoundPlay("취소");
                UIManager.instance.Message2("돈이 부족합니다.");
                return;
            }

            if (Farmingtool != 0)
            {
                SfxManager.instance.SoundPlay("취소");
                UIManager.instance.Message2("이미 최대 레벨입니다.");
                return;
            }

            Money -= 25000;
            Farmingtool = 1;
            MaxFarmingCrop = 3;
            SfxManager.instance.SoundPlay("구매");
            UIManager.instance.UpgradeObj[num].SetActive(true);
        } else
        {
            if (Money < 12500)
            {
                SfxManager.instance.SoundPlay("취소");
                UIManager.instance.Message2("돈이 부족합니다.");
                return;
            }

            if (Nets)
            {
                SfxManager.instance.SoundPlay("취소");
                UIManager.instance.Message2("이미 소지하고 있습니다.");
                return;
            }

            Money -= 12500;
            Nets = true;
            SfxManager.instance.SoundPlay("구매");
        }
    }
}
