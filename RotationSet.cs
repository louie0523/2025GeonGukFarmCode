using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSet : MonoBehaviour
{
    public Vector3 offset;


    private void Start()
    {
        this.transform.localRotation = Quaternion.Euler(offset);
    }
}
