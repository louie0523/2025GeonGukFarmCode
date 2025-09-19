using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageEnter : MonoBehaviour
{
    public static StorageEnter Instance;

    public bool StorageEnterTrue = false;
    public BoxCollider boxCollider;


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

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StorageEnterTrue = true;
            UIManager.instance.StorageText.text = $"창고에 들어간다 [ F ]";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StorageEnterTrue = false;
            UIManager.instance.StorageText.text = $"";
        }
    }
}
