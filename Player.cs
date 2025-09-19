using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public Crop CurrentSeed;
    public List<int> SeedCount = new List<int>();
    public int fatigue = 0;
    public float speed = 5f;
    public GameObject PlayerObj;
    public Grid CurrentGrid;
    public GameObject CurrentTileOjb;
    public bool isFarm = false;
    public bool Watring = false;
    public bool Farming = false;
    public bool thisGridFarimg = false;
    public List<GameObject> WaterCanobj = new List<GameObject>();
    public List<GameObject> FarmingTool = new List<GameObject>();
    public bool neting = false;
    public bool isAnimalForward = false;

    private Rigidbody rb;
    private Animator animator;
    public float Step_time = 0.35f;
    public float Step_curTimer = 0f;
    public Animal currentAnimal;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // 회전 방지
        PlayerObj = transform.GetChild(0).gameObject;
        animator = PlayerObj.GetComponent<Animator>();
        //SeedCountSet();
        Invoke("bgmSet", 1f);
    }

    public void bgmSet()
    {
        SfxManager.instance.BgmPlay("브금2");
    }

  

    private void Update()
    {
        Move();

        if(CurrentGrid.Tile.Crop == Crop.None && CurrentSeed != Crop.None && Input.GetKeyDown(KeyCode.E) && isFarm)
        {
            SeedTile();
        }

        if(CurrentGrid.isComplete && !CurrentGrid.DestroyCrop)
        {
            thisGridFarimg = true;
        } else
        {
            thisGridFarimg = false;
        }

        if(GameManager.Instance.Nets && isAnimalForward && currentAnimal != null)
        {
            UIManager.instance.animalGet.SetActive(true);
        } else
        {
            UIManager.instance.animalGet.SetActive(false);
        }
        

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            StartCoroutine(Storage.Instance.ShowItem());
        }

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    Storage.Instance.TestSet();
        //}

        if(Input.GetKeyDown(KeyCode.Mouse1) && GameManager.Instance.Nets)
        {
            Debug.Log("닭 잡는 중");
            NetsUse();
        }


        if (Input.GetKeyDown(KeyCode.LeftShift) && !Input.GetKeyDown(KeyCode.Mouse0))
        {
            WaterCanUse();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKeyDown(KeyCode.LeftShift))
        {
            FarmingToolUse();
        }

        if (Water.Instance.WaterUp && Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Instance.currentWater = GameManager.Instance.MaxWater;
            SfxManager.instance.SoundPlay("물채우기");
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentSeed = Crop.Corn;
        }
         else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentSeed = Crop.Carrot;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentSeed = Crop.Cflower;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CurrentSeed = Crop.Broccoil;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CurrentSeed = Crop.GoldCarrot;
        }

        if(Input.GetKeyDown(KeyCode.Z) && !CurrentGrid.Tile.bhouseBuy && GameManager.Instance.Money >= 30000 && CurrentGrid.Tile.isOpen){
            GameManager.Instance.Money -= 30000;
            CurrentGrid.Tile.bhouseBuy = true;
            SfxManager.instance.SoundPlay("구매");
        } else if(Input.GetKeyDown(KeyCode.Z) && CurrentGrid.Tile.bhouseBuy)
        {
            float distance = Vector3.Distance(transform.position, CurrentGrid.Tile.transform.position);
            if(distance < 10f)
            {
                GreenHouseUse();
            }
        }


        if (Input.GetKeyDown(KeyCode.C) && !CurrentGrid.Tile.AutoFarmer && GameManager.Instance.Money >= 70000 && CurrentGrid.Tile.isOpen)
        {

            GameManager.Instance.Money -= 70000;
            CurrentGrid.Tile.AutoFarmer = true;
            CurrentGrid.Tile.autoFamrerObj.SetActive(true);
            SfxManager.instance.SoundPlay("구매");
        }
    }

    void SeedCountSet()
    {
        for(int i = 0; i < SeedCount.Count; i++)
        {
            SeedCount[i] = 0;
        }
        for(int i = 0; i < Storage.Instance.itemSlot.Count; i++)
        {
            if ((int)Storage.Instance.itemSlot[i] <= 4)
            {
                SeedCount[(int)Storage.Instance.itemSlot[i]]++;
            }
        }
    }

    void SeedTile()
    {
        if (SeedCount[(int)CurrentSeed - 1] <= 0)
        {
            string Cropstr = "옥수수";
            switch (CurrentSeed)
            {
                case global::Crop.Carrot:
                    Cropstr = "당근";
                    break;
                case global::Crop.Cflower:
                    Cropstr = "양배추";
                    break;
                case global::Crop.Broccoil:
                    Cropstr = "브로콜리";
                    break;
                case global::Crop.GoldCarrot:
                    Cropstr = "황금 당근";
                    break;
            }
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message($"{Cropstr}의 씨앗이 부족합니다.");
            return;
        }

        Tile tile = CurrentGrid.Tile;

        SeedCount[(int)CurrentSeed - 1]--;

        tile.SeedCrop(CurrentSeed);


        tile.CanvasAct();
    }

    void Move()
    {
        if(Watring || Farming || GameManager.Instance.StorageCam.activeSelf)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(h, 0, v).normalized;
        bool walking = move.magnitude > 0f;
        animator.SetBool("Walk", walking);

        Vector3 velocity = move * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);


        Step_curTimer -= Time.deltaTime;

        if (move.magnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            PlayerObj.transform.rotation = Quaternion.Slerp(
                PlayerObj.transform.rotation, 
                targetRotation, 
                10 * Time.deltaTime);

            if (Step_curTimer < 0)
            {
                SfxManager.instance.SoundPlay("걷기2");
                Step_curTimer = Step_time;
            }
        }

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Grid"))
        {
            CurrentGrid = collision.gameObject.GetComponent<Grid>();
            SetCurrentTilePostion();
        }
        else if (collision.gameObject.CompareTag("Animal"))
        {
            currentAnimal = null;
            isAnimalForward = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Grid"))
        {
            CurrentGrid = collision.gameObject.GetComponent<Grid>();
            SetCurrentTilePostion();
        } else if (collision.gameObject.CompareTag("Animal"))
        {
            currentAnimal = collision.gameObject.GetComponent<Animal>();
            isAnimalForward = true;
        }

    }


    public void GreenHouseUse()
    {
        if (!CurrentGrid.Tile.bhouseObj.activeSelf)
        {
            SfxManager.instance.SoundPlay("장착");
            CurrentGrid.Tile.bhouseObj.SetActive(true);
        } else
        {
            SfxManager.instance.SoundPlay("해제");
            CurrentGrid.Tile.bhouseObj.SetActive(false);
        }
    }
    public void SetCurrentTilePostion()
    {
        Tile tile = CurrentGrid.Tile;

        CurrentTileOjb.transform.localPosition = tile.gameObject.transform.localPosition;

        if(tile.isOpen)
            isFarm = true;
        else
            isFarm  = false;
    }

    public void WaterCanUse()
    {
        if(GameManager.Instance.currentWater >= 1 && !Farming && !Watring && !neting)
        {
            GameManager.Instance.currentWater--;
            WateilyTile();
            StartCoroutine(waterAnimation());
            SfxManager.instance.SoundPlay("물뿌리기");
        } else if(GameManager.Instance.currentWater <= 0)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message("물뿌리기에 물이 부족합니다.");
        } else if(Farming)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message("수확 중엔 불가능합니다.");
        } else if(Watring)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message("이미 물을 뿌리고 있습니다.");
        } else if(neting)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message("포획 중엔 불가능합니다.");
        }
    }

    public IEnumerator waterAnimation()
    {
        Debug.Log("물뿌리기애니");
        for(int i = 0; i < WaterCanobj.Count; i++)
        {
            if(i == GameManager.Instance.water_Can)
                WaterCanobj[i].SetActive(true);
            else
                WaterCanobj[i].SetActive(false);
        }
        animator.SetTrigger("Water");
        Watring = true;
        yield return new WaitForSeconds(1.4f);
        Watring = false;
        for (int i = 0; i < WaterCanobj.Count; i++)
        {
                WaterCanobj[i].SetActive(false);
        }
    }

    public void FarmingToolUse()
    {
        if (thisGridFarimg && !Watring && !Farming && Storage.Instance.itemSlot.Count < GameManager.Instance.MaxStorage && !neting)
        {
            CurrentGrid.Farming();
            StartCoroutine(FarmingAnimation());
        }
        else if (Storage.Instance.itemSlot.Count >= GameManager.Instance.MaxStorage)
        {
            UIManager.instance.Message("창고가 가득 찼습니다.");
        }
        else if (Farming)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message("이미 수확 중입니다.");
        }
        else if (Watring)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message("물을 뿌리는 도중엔 불가능합니다.");
        }
        else if (neting)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message("포획 중엔 불가능합니다.");
        }
    }

    public IEnumerator FarmingAnimation()
    {
        for (int i = 0; i < FarmingTool.Count; i++)
        {
            if (i == GameManager.Instance.water_Can)
                FarmingTool[i].SetActive(true);
            else
                FarmingTool[i].SetActive(false);
        }
        animator.SetTrigger("Farming");
        Farming = true;
        yield return new WaitForSeconds(1.1f);
        Farming = false;
        for (int i = 0; i < FarmingTool.Count; i++)
        {
                FarmingTool[i].SetActive(false);
        }
    }

    public void NetsUse()
    {
        if (isAnimalForward && !Watring && !Farming  && !neting && GameManager.Instance.Nets)
        {
            Debug.Log("달강 좀 뒤져");
            StartCoroutine(getsAnimal());
            StartCoroutine(NetgetAni());
            ZZZ();
        }
        else if (Farming)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message("수확 중엔 불가능합니다.");
        }
        else if (Watring)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message("물을 뿌리는 도중엔 불가능합니다.");
        }
        else if (neting)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message("이미 포획 중입니다.");
        } else if(!GameManager.Instance.Nets)
        {
            SfxManager.instance.SoundPlay("취소");
            UIManager.instance.Message("포획 도구가 없습니다.");
        }
    }

    void ZZZ()
    {
        int rand = Random.Range(1, 101);
        if(rand <= 20)
        {
            SfxManager.instance.SoundPlay("찢");
            UIManager.instance.Message("그물이 찢어졌습니다..");
            GameManager.Instance.Nets = false;
        }
    }


    IEnumerator getsAnimal()
    {
        if (currentAnimal == null)
            yield return null;

        SfxManager.instance.SoundPlay("닭");

        Animator ani = CurrentGrid.Tile.chickens[currentAnimal.num].GetComponent<Animator>();
        SphereCollider col = CurrentGrid.Tile.chickens[currentAnimal.num].GetComponent<SphereCollider>();
        col.isTrigger = true;
        ani.SetBool("Walk", true);
        yield return new WaitForSeconds(0.4f);
        ani.SetBool("Walk", false);
        col.isTrigger = false;
        CurrentGrid.Tile.chickens[currentAnimal.num].SetActive(false);
        currentAnimal.parentTIle.CurrentAniaml--;
        GameManager.Instance.CurrentAnimals--;
        currentAnimal = null;
        isAnimalForward = false;

        if (GameManager.Instance.CurrentAnimals <= 0)
        {
            SfxManager.instance.BgmPlay("브금2");
            GameManager.Instance.CurrentAnimals = 0;
        }


    }
    public IEnumerator NetgetAni()
    {
        
        animator.SetTrigger("Nets");
        neting = true;
        yield return new WaitForSeconds(1.3f);
        neting = false;
    }

    void WateilyTile()
    {
        CurrentGrid.Tile.WateilyUp(GameManager.Instance.waterUp);
    }
}
