using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMpaGenerater : MonoBehaviour
{
    public GameObject TilePrefab;
    public float Xsize = 6;
    public float Zsize = 6;
    
    public int x = 9;
    public int z = 9;


    private void Start()
    {
        MapGenerate();
    }

    void MapGenerate()
    {
        int midx = Mathf.FloorToInt(x / 2f);
        int midz = Mathf.FloorToInt(z / 2f);

        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < z; j++)
            {
                float posX = (i - midx) * Xsize;
                float posZ = (j - midz) * Zsize;


                GameObject tile = Instantiate(TilePrefab, new Vector3(posX, 0, posZ), Quaternion.identity, this.transform);
                GameManager.Instance.tiles.Add(tile.GetComponent<Tile>());
            }
        }
    }

}
