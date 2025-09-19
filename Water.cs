using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public static Water Instance;

    public BoxCollider boxCollider;
    public bool WaterUp = false;

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
        if(other.gameObject.CompareTag("Player"))
        {
            WaterUp = true;
            UIManager.instance.WaterGetText.text = $"물을 보충한다 [ F ]";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            WaterUp = false;
            UIManager.instance.WaterGetText.text = $"";
        }
    }
}
