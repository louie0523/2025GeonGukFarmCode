using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Grid : MonoBehaviour
{
    public Tile Tile;
    public int ListNum = 0;
    public bool isFarm = false;
    public bool isComplete = false;
    public bool DestroyCrop = false;

    private void Start()
    {
        Tile = transform.parent.GetComponent<Tile>();
    }

    public void Farming(bool auto = false)
    {
        Tile.CurrentFarming++;
        
        SfxManager.instance.SoundPlay("��Ȯ");
        Debug.Log($"{Tile.Crop}�� ��Ȯ�մϴ�.");
        int rand = Random.Range(1, 101);
        int myRand = auto ? GameManager.Instance.SeedRand : 40;
        if (rand < myRand)
        {
            Debug.Log("���� ȹ��");
            Storage.Instance.itemSlot.Add((Item)(int)Tile.Crop - 1);
        }
        CropGet(Tile.Crop);
        Tile.DestroyCrop(ListNum, true);
        if(GameManager.Instance.StorageCam.activeSelf)
        StartCoroutine(Storage.Instance.ShowItem());
        Tile.AnimalCheck();
    }

    void CropGet(Crop crop)
    {
        int Seed = Random.Range(1, GameManager.Instance.MaxFarmingCrop + 1);
        Debug.Log($"���� �۹� : {Seed}");
        for (int i = 0; i < Seed; i++)
        {
            if(Storage.Instance.itemSlot.Count < GameManager.Instance.MaxStorage)
            {
                Storage.Instance.itemSlot.Add((Item)((int)crop) + 4);
                Debug.Log($"{(int)crop + 4} 1�� ȹ��");
            }
        }
        if (GameManager.Instance.StorageCam.activeSelf)
            StartCoroutine(Storage.Instance.ShowItem());
    }

}
