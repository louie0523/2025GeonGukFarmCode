using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Seedobj
{
    public GameObject CropObj;
    public Text SeedText;
}
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Sprite SunIcon;
    public Sprite MoonIcon;

    public Image WatehrIcon;
    public Image TimeIcon;
    public Text TimeText;
    public Text GoldText;
    public Text GoldText2;
    public Text WaterText;
    public Text StorageText;

    public Text TileBuyText;
    public Text FarmText;
    public Text MessageText;
    public Text MessageText2;
    public Text WaterGetText;
    public Text FarmingText;
    public GameObject gaysan;
    public Text StorageUpgradetext;
    public GameObject SeedGetUI;
    public Text[] SeedGetTexts = new Text[5];
    public Text[] StorSeedTexts = new Text[5];
    public GameObject[] UpgradeObj = new GameObject[2];
    public GameObject StotreObjUI;

    public Text StroageMaxText;
    public Text StroageMaxText2;

    public Text GreenHouseText;
    public Text AutoFarmerText;

    public GameObject animalGet;

    private Coroutine messageCoroutine;
    private Coroutine messageCoroutine2;

    public List<Seedobj> SeedObjList = new List<Seedobj>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        SetTime();

        TileBuy();

        FarmTextSet();

        GoldText.text = GameManager.Instance.Money.ToString("N0") + "$";
        if(GameManager.Instance.isStorageIn)
            GoldText2.text = GameManager.Instance.Money.ToString("N0") + "$";

        WaterText.text = $"{GameManager.Instance.currentWater}/{GameManager.Instance.MaxWater}";

        SeedCountText();

        SeedPriceText();

        if (GameManager.Instance.player.thisGridFarimg)
            FarmingText.text = "수확 하기 [ 좌클릭 ]";
        else
            FarmingText.text = "";

        StroageMaxText.text = $"{Storage.Instance.itemSlot.Count} / {GameManager.Instance.MaxStorage}";
        StroageMaxText2.text = $"{Storage.Instance.itemSlot.Count} / {GameManager.Instance.MaxStorage}";

        if (GameManager.Instance.MaxStorage <= 40)
            StorageUpgradetext.text = $"PRICE : {GameManager.Instance.StorageUpgradePrice}";
        else
            StorageUpgradetext.text = "";

        if (!GameManager.Instance.player.CurrentGrid.Tile.bhouseBuy)
            GreenHouseText.text = "비닐하우스 구매 [ Z ]\r\n가격 : 30000";
        else if(!GameManager.Instance.player.CurrentGrid.Tile.bhouseObj.activeSelf)
            GreenHouseText.text = "비닐하우스\r\n펼치기 [ Z ]";
        else if (!GameManager.Instance.player.CurrentGrid.Tile.bhouseObj.activeSelf)
            GreenHouseText.text = "비닐하우스\r\n접기 [ Z ]";

        if (!GameManager.Instance.player.CurrentGrid.Tile.autoFamrerObj.activeSelf)
            AutoFarmerText.text = "자동 수확기 구매 [ C ]\r\n가격 : 70000";
        else
            AutoFarmerText.text = "구매해주셔서\r\n감사합니다 :)";

    }

    void SeedCountText()
    {
        int count = 0;
        foreach(Seedobj obj in SeedObjList)
        {
            if(obj.CropObj.activeSelf)
            {
                obj.SeedText.text = GameManager.Instance.player.SeedCount[count].ToString();
            }
            count++;
            
                  
        }
    }

    void SeedPriceText()
    {
        for(int i = 0;i < 5; i++)
        {
            StorSeedTexts[i].text = GameManager.Instance.SeedPrice[i].ToString();
        }
    }


    void SetTime()
    {
        TimeIcon.sprite = TimeManager.instance.CurrentTime >= 6 && TimeManager.instance.CurrentTime <= 18 ? SunIcon : MoonIcon;
        string AmPm = TimeManager.instance.CurrentTime <= 12 ? "AM" : "PM";
        TimeText.text = Mathf.FloorToInt(TimeManager.instance.CurrentTime) + $"{AmPm}";
    }

    void TileBuy()
    {
        if(!GameManager.Instance.player.isFarm && GameManager.Instance.tileAct)
        {
            TileBuyText.text = $"현재 타일({GameManager.Instance.NextTilePrice.ToString("N0")}$)을 구매한다 [ Q ]";
        } else
        {
            TileBuyText.text = $"";
        }
    }

    void FarmTextSet()
    {
        if(GameManager.Instance.player.CurrentGrid.Tile.Crop == Crop.None && GameManager.Instance.player.CurrentSeed != Crop.None && GameManager.Instance.player.isFarm)
        {
            string Crop = "옥수수";
            switch(GameManager.Instance.player.CurrentSeed)
            {
                case global::Crop.Carrot:
                    Crop = "당근";
                    break;
                case global::Crop.Cflower:
                    Crop = "양배추";
                    break;
                case global::Crop.Broccoil:
                    Crop = "브로콜리";
                    break;
                case global::Crop.GoldCarrot:
                    Crop = "황금 당근";
                    break;
            }

            FarmText.text = $"{Crop} 씨앗 심기 [ E ]";
        } else
        {
            FarmText.text = $"";
        }
    }

    public void Message(string message)
    {
        if(messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        MessageText.text = message;
        MessageText.color = new Color(MessageText.color.r, MessageText.color.g, MessageText.color.b, 1f);
        messageCoroutine = StartCoroutine(MessageFade());
    }

    IEnumerator MessageFade()
    {
        yield return new WaitForSeconds(2.5f);

        
        float fade = 1f;
        while(fade > 0f)
        {
            fade -= Time.deltaTime;
            MessageText.color = new Color(MessageText.color.r, MessageText.color.g, MessageText.color.b, fade);

            yield return null;
        }
        MessageText.text = "";
        
    }

    public void Message2(string message)
    {
        if (messageCoroutine2 != null)
            StopCoroutine(messageCoroutine2);

        MessageText2.text = message;
        MessageText2.color = new Color(MessageText2.color.r, MessageText2.color.g, MessageText2.color.b, 1f);
        messageCoroutine2 = StartCoroutine(MessageFade2());
    }

    IEnumerator MessageFade2()
    {
        yield return new WaitForSeconds(2.5f);


        float fade = 1f;
        while (fade > 0f)
        {
            fade -= Time.deltaTime;
            MessageText2.color = new Color(MessageText2.color.r, MessageText2.color.g, MessageText2.color.b, fade);

            yield return null;
        }
        MessageText2.text = "";

    }

    public IEnumerator GaySanGO(string gs, string end)
    {
        gaysan.SetActive(true);

        Text GsText = gaysan.transform.GetChild(0).GetComponent<Text>();
        Text endText = gaysan.transform.GetChild(1).GetComponent<Text>();

        GsText.text = gs;
        endText.text = end;

        yield return new WaitForSeconds(2.5f);

        gaysan.SetActive(false);
    }
}
