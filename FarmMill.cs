using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotationVecter
{
    X,
    Y,
    Z,
};

public class FarmMill : MonoBehaviour
{
    public RotationVecter rotationVecter;
    public GameObject TargetObj;
    public float FullRotationTime = 3f;
    public float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        float value = (timer / FullRotationTime) / FullRotationTime;

        float rotation = value * 360f;

        Vector3 vector3 = rotationVecter == RotationVecter.X ? new Vector3(rotation, 0, 0) : rotationVecter == RotationVecter.Y ? new Vector3(0, rotation, 0) : new Vector3(0, 0, rotation);
        TargetObj.transform.localRotation = Quaternion.Euler(vector3);
    }
}
