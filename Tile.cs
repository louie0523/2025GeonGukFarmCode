using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Crop Crop;
    public int Grow = 0;
    public bool fullGrow = false;
    public GameObject CropParent;
    public List<GameObject> Grid = new List<GameObject>();
    public List<Grid> GridScripts = new List<Grid>();
    public Mesh FarmGroundMesh;
    public GameObject Grass;
    public GameObject Canvas;
    public Slider growSlider;
    public Image CurrentGrowStatusImage;
    public Sprite goodImage;
    public Sprite badImage;
    public Sprite GorwImage;
    public Text wateilyText;
    public bool isOpen = false;
    public bool bhouseBuy = false;
    public bool AutoFarmer = false;
    public float Destorytimer = 3f;
    public GameObject autoFamrerObj;
    public GameObject bhouseObj;
    [Range(0, 100)]
    public int wateily;
    public float glowTimer = 0f;
    public bool autoFarming = false;
    public int CurrentAniaml = 0;
    public int AnimalRand = 0;
    public int CurrentFarming = 0;
    public List<GameObject> chickens = new List<GameObject> ();
    float animalAttackTimer = 0;

    private void Start()
    {
        int num = 0;
        foreach(GameObject grid in Grid)
        {
            Grid girdsc = grid.GetComponent<Grid>();
            girdsc.ListNum = num;
            GridScripts.Add(girdsc);
            num++;
            
        }
    }

    public void ToFarm()
    {
        isOpen = true;
        Grass.SetActive(false);
        foreach(GameObject grid in Grid)
        {
            MeshFilter filter = grid.GetComponent<MeshFilter>();
            filter.mesh = FarmGroundMesh;
        }
    }

    public void SeedCrop(Crop crop)
    {
        SfxManager.instance.SoundPlay("심기");

        Crop = crop;
        Grow = 1;

        CropParent.transform.GetChild((int)Crop-1).transform.GetChild(0).gameObject.SetActive(true);
        foreach(Grid grid in GridScripts)
        {
            grid.DestroyCrop = false;
            grid.isComplete = false;
        }
        
    }

    public void ResetTile()
    {

        GorwConditon gorwConditon = GameManager.Instance.GorwConditons[(int)Crop - 1];
        int growStep = Mathf.FloorToInt(gorwConditon.GrowTime / 3f);
        int gronum = 0;
        if (Grow <= growStep)
        {
            gronum = 0;
        }
        else if (Grow < growStep * 3)
        {
            gronum = 1;
        }
        else if (Grow >= growStep)
        {
            gronum = 2;
        }
        foreach (Transform obj in CropParent.transform.GetChild((int)Crop - 1).transform.GetChild(0).gameObject.transform)
        {
            obj.gameObject.SetActive(false);
        }
        foreach (Transform obj in CropParent.transform.GetChild((int)Crop - 1).transform.GetChild(1).gameObject.transform)
        {
            obj.gameObject.SetActive(false);
        }
        foreach (Transform obj in CropParent.transform.GetChild((int)Crop - 1).transform.GetChild(2).gameObject.transform)
        {
            obj.gameObject.SetActive(false);
        }
        //CropParent.transform.GetChild((int)Crop - 1).transform.gameObject.SetActive(false);

        Crop = Crop.None;
        Grow = 0;
        CanvasAct();


    }

    private void Update()
    {
        if(Crop != Crop.None)
        {
            Growing();
            ActShape();
        }
        wateilyText.text = wateily + "%";

        if(Grow <= 0 && Crop != Crop.None)
        {
            Destorytimer -= Time.deltaTime;
            if(Destorytimer < 0)
            {
                Destorytimer = 3f;
                AllDestoryCrop();
            }
        }

        if(autoFamrerObj.activeSelf && fullGrow && Storage.Instance.itemSlot.Count < GameManager.Instance.MaxStorage && !autoFarming)
        {
            AutoFarmerAct();
        }

       AnimalSeba();

       



    }

    void AutoFarmerAct()
    {
        autoFarming = true;
        fullGrow = false;
        Animator ani = autoFamrerObj.GetComponent<Animator>();
        ani.SetTrigger("Farm");
        SfxManager.instance.SoundPlay("자동수확");
        for (int i = 0; i < 9; i++)
        {
            if (GridScripts[i].isComplete && !GridScripts[i].DestroyCrop && Storage.Instance.itemSlot.Count < GameManager.Instance.MaxStorage)
            {
                GridScripts[i].Farming(true);
            }
            else if (Storage.Instance.itemSlot.Count >= GameManager.Instance.MaxStorage)
            {
                break;
            }
        }
        autoFarming = false;
    }

    void AllDestoryCrop()
    {
        SfxManager.instance.SoundPlay("시듦");
        for (int i = 0; i < 9; i++)
        {
            DestroyCrop(i);
        }
    }

    public void CanvasAct()
    {
        if(Crop != Crop.None)
            Canvas.SetActive(true);
        else
            Canvas.SetActive(false);
    }

    void Growing()
    {
        glowTimer += Time.deltaTime;
        SliderSet();
        if (glowTimer >= 1f && !fullGrow && Crop != Crop.None)
        {
            growCheck();
        }

        GorwConditon gorwConditon = GameManager.Instance.GorwConditons[(int)Crop - 1];
        if (gorwConditon.waterlyMin <= wateily && gorwConditon.waterlyMax >= wateily && ConditnTIme() && CondtionWeather() && !fullGrow)
        {
            CurrentGrowStatusImage.sprite = goodImage;
            CurrentGrowStatusImage.color = Color.green;
        } else if(gorwConditon.GrowTime > Grow || !fullGrow)
        {
            CurrentGrowStatusImage.sprite = badImage;
            CurrentGrowStatusImage.color = Color.red;
        }
    }

    void growCheck()
    {
        //Debug.Log("체크");
        glowTimer = 0f;
        GorwConditon gorwConditon = GameManager.Instance.GorwConditons[(int)Crop - 1];
        if (gorwConditon.waterlyMin <= wateily && gorwConditon.waterlyMax >= wateily && ConditnTIme() && CondtionWeather())
        {
            if (Grow < gorwConditon.GrowTime)
            {
                if ((int)Crop <= 2 && bhouseObj.activeSelf)
                    return;

                Grow += WeatherManager.instance.WathrGrow[(int)WeatherManager.instance.weather];
                if(Grow <= 0)
                    Grow = 0;
            }
            else
                Grow = gorwConditon.GrowTime;
            ActShape();
            if (gorwConditon.GrowTime <= Grow || fullGrow)
            {
                Debug.Log($"{Crop}이 모두 자랐습니다!");
                CurrentGrowStatusImage.sprite = GorwImage;
                CurrentGrowStatusImage.color = Color.yellow;
                gridGrowComplete();
            }

        }

    }

    public void WateilyUp(int  water)
    {
        if (bhouseObj.activeSelf && water <= 0)
            return;

        wateily += water;
        if(wateily > 100)
        {
            wateily = 100;
        }
        if(wateily <= 0)
        {
            wateily = 0;
        }
    }

    bool ConditnTIme()
    {
        GorwConditon gorwConditon = GameManager.Instance.GorwConditons[(int)Crop - 1];
        bool type = gorwConditon.StartTime < gorwConditon.EndTIme ? true : false;
        if (type)
        {
            if(TimeManager.instance.CurrentTime >= gorwConditon.StartTime && TimeManager.instance.CurrentTime <= gorwConditon.EndTIme)
            {
                //Debug.Log("트루");
                return true;
            }
        } else
        {
            if(TimeManager.instance.CurrentTime >= gorwConditon.StartTime | TimeManager.instance.CurrentTime <= gorwConditon.EndTIme)
            {
                //Debug.Log("뻘즈");
                return true;
            }
        }
        Debug.Log("해당 사항 없어");
        return false;
    }

    bool CondtionWeather()
    {
        if (bhouseObj.activeSelf)
        {
            return true;
        }

        GorwConditon gorwConditon = GameManager.Instance.GorwConditons[(int)Crop - 1];
        if(WeatherManager.instance.weather == Weather.Clean && gorwConditon.Clean)
        {
            return true;
        } else if(WeatherManager.instance.weather == Weather.Cloudy && gorwConditon.Cloudy)
        {
            return true;
        }
        else if (WeatherManager.instance.weather == Weather.Rain && gorwConditon.Rain)
        {
            return true;
        }
        else if (WeatherManager.instance.weather == Weather.Storm && gorwConditon.Storm)
        {
            return true;
        }
        else if (WeatherManager.instance.weather == Weather.Hail && gorwConditon.Hail)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void ActShape()
    {
        GorwConditon gorwConditon = GameManager.Instance.GorwConditons[(int)Crop - 1];
        int growStep = Mathf.FloorToInt(gorwConditon.GrowTime / 3f);
        int num = 0;
        if(Grow <= growStep)
        {
            num = 0;
        } else if(Grow < gorwConditon.GrowTime)
        {
            num = 1;
        } else if(Grow >= gorwConditon.GrowTime)
        {
            num = 2;
        }

        for(int i = 0; i < CropParent.transform.GetChild((int)Crop - 1).transform.childCount; i++)
        {
            if(i == num)
            {
                CropParent.transform.GetChild((int)Crop - 1).transform.GetChild(i).gameObject.SetActive(true);
                for(int j = 0; j < 9; j++)
                {
                    if (!GridScripts[j].DestroyCrop)
                    {
                        CropParent.transform.GetChild((int)Crop - 1).transform.GetChild(i).gameObject.transform.GetChild(j).gameObject.SetActive(true);
                    } else
                    {
                        CropParent.transform.GetChild((int)Crop - 1).transform.GetChild(i).gameObject.transform.GetChild(j).gameObject.SetActive(false);
                    }
                }
                
            } else
            {
                //CropParent.transform.GetChild((int)Crop - 1).transform.GetChild(i).gameObject.SetActive(false);
                foreach (Transform obj in CropParent.transform.GetChild((int)Crop - 1).transform.GetChild(i).gameObject.transform)
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
    }

    public void gridGrowComplete()
    {
        fullGrow = true ;
        foreach(Grid grid in GridScripts)
        {
            grid.isComplete = true;
        }
    }

    public void HailDestroy()
    {
        if (bhouseObj.activeSelf)
        {
            return;
        }

        int num1 = Random.Range(0, 9);
        int count = 0;
        while (GridScripts[num1].DestroyCrop)
        {
            num1 = Random.Range(0, 9);

            count++;
            if(count >= 100)
            {
                return;
            }
        }
        int num2 = Random.Range(0, 9);
        count = 0;
        bool lastone = false;
        int Dcount = 0;
        for (int i = 0; i < 9; i++)
        {
            if (GridScripts[i].DestroyCrop)
            {
                Dcount++;
            }
        }
        if (Dcount >= 7)
        {
            lastone = true;
        }else
        {
            while (GridScripts[num2].DestroyCrop || num2 == num1)
            {
                num2 = Random.Range(0, 9);
                count++;
                if (count >= 100)
                {
                    break;
                }
            }
        }



        DestroyCrop(num1);
        if(!lastone)
            DestroyCrop(num2);

    }

    public void DestroyCrop(int num, bool farming = false)
    {
        GridScripts[num].isComplete = false;
        GridScripts[num].DestroyCrop = true;
        GorwConditon gorwConditon = GameManager.Instance.GorwConditons[(int)Crop-1];
        int growStep = Mathf.FloorToInt(gorwConditon.GrowTime / 3f);
        int gronum = 0;
        if (Grow <= growStep)
        {
            gronum = 0;
        }
        else if (Grow < growStep * 3)
        {
            gronum = 1;
        }
        else if (Grow >= growStep)
        {
            gronum = 2;
        }
        if(!farming)
        {
            CropParent.transform.GetChild((int)Crop - 1).transform.GetChild(gronum).gameObject.transform.GetChild(num).gameObject.SetActive(false);
        } else
        {
            GameObject ob = CropParent.transform.GetChild((int)Crop - 1).transform.GetChild(gronum).gameObject.transform.GetChild(num).gameObject;
            StartCoroutine(SetActiveObjs(ob, false, 1f));
        }
        AllDestroyCheck();
    }

    public IEnumerator SetActiveObjs(GameObject obj, bool act, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(act);
    }

    public void AllDestroyCheck()
    {
        int count = 0;
        foreach(Grid grid in GridScripts)
        {
            if (grid.DestroyCrop)
                count++;
        }

        if(count >= 9)
        {
            Debug.Log("모든 작물이 사라짐. 타일 리셋");
            ResetTile();
        }

    }

    void SliderSet()
    {

        growSlider.value = Grow / (float)GameManager.Instance.GorwConditons[(int)Crop - 1].GrowTime;
    }

     public void AnimalCheck()
    {
        if (CurrentAniaml > 0)
            return;

        CurrentFarming++;
        if(CurrentFarming == 3)
        {
            AnimalRand = 5;
        } else if(CurrentFarming >= 4)
        {
            AnimalRand += 2;
        }

        int rand = Random.Range(1, 101);

        if(rand < AnimalRand)
        {
            Debug.Log("야생 동물 두두둥장");
            int rand2 = Random.Range(1, 4);
            CurrentAniaml = rand2;
            CurrentFarming = 0;
            AnimalRand = 0;

            if(GameManager.Instance.CurrentAnimals <= 0)
            {
                SfxManager.instance.BgmPlay("야생동물");
            }
            GameManager.Instance.CurrentAnimals += rand2;

            for(int i = 0; i < CurrentAniaml; i++)
            {
                float X = Random.Range(-2.7f, 2.8f);
                float Z = Random.Range(-2.7f, 2.8f);
                chickens[i].SetActive(true);
                chickens[i].transform.position = new Vector3(transform.position.x, 0, transform.position.z) + new Vector3(X, -0.498f, Z);
            }
        }
    }

    void AnimalSeba()
    {
        if (CurrentAniaml > 0)
        {
            animalAttackTimer += Time.deltaTime;
            if(animalAttackTimer >= 1)
            {
                animalAttackTimer = 0;
                if(!bhouseObj.activeSelf)
                    Grow--;
            }
        }
    }

}
