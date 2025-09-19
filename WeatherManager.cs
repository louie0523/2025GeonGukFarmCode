using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum Weather
{
    Clean,
    Cloudy,
    Rain,
    Storm,
    Hail,
};


public class WeatherManager : MonoBehaviour
{
    public static WeatherManager instance;

    public List<string> WeatherNames = new List<string>()
    {
        "¸¼À½","Èå¸²","ºñ","ÆøÇ³","¿ì¹Ú"
    };
    public List<int> WathrWateily = new List<int>()
    {
        -1, -1, +1, -10, -15
    };
    public List<int> WathrGrow = new List<int>()
    {
        2,1,2,1,1
    };
    public List<Sprite> WeatherSprites = new List<Sprite>();
    public Weather weather;
    public float timer = 0f;
    
    

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

    private void Start()
    {
        SetWeather();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer >= 5f)
        {
            timer = 0f;
            foreach(Tile tile in GameManager.Instance.tiles)
            {
                tile.WateilyUp(WathrWateily[(int)weather]);
            }
            if(weather == Weather.Hail)
            {
                int rand = Random.Range(1, 101);
                if (rand <= 2)
                    HailCrop();
            }
        }
    }

    public void HailCrop()
    {
        List<Tile> OpenTIles = GameManager.Instance.tiles.Where(tile => tile.isOpen && tile.Crop != Crop.None).ToList();
        
        Debug.Log(OpenTIles.Count);
        if(OpenTIles.Count <= 0)
            return;

        int rand = Random.Range(0, OpenTIles.Count);

        Tile CurrentTIle = OpenTIles[rand];

        CurrentTIle.HailDestroy();


    }

    public void SetWeather()
    {
        int rand = Random.Range(0, 5);
        weather = (Weather)rand;
        UIManager.instance.WatehrIcon.sprite = WeatherSprites[rand];
        Debug.Log($"¿À´ÃÀÇ ³¯¾¾´Â {WeatherNames[rand]}ÀÔ´Ï´Ù.");
    }
}
