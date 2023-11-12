using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundMissile : MonoBehaviour
{
    public Transform target = null;
    public GameObject prefab = null;
    public float targetToMissileDist = 10f;

    public float rotateSpeed = 20f;
    public int HoverAroundMissileCount = 5;

    int selectedAngelIndex = 0;

    List<Vector3> rotList = new List<Vector3>();
    List<GameObject> objList = new List<GameObject>();

    /*
     * *
     */

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            rotList.Clear();
            int deltaAngle = 360 / HoverAroundMissileCount;
            for (int i = 0; i < HoverAroundMissileCount; i++)
            {
                selectedAngelIndex++;
                if (selectedAngelIndex > HoverAroundMissileCount)
                    selectedAngelIndex = 1;

                float angle = deltaAngle * selectedAngelIndex;
                Vector3 rotVec = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));
                rotList.Add(rotVec);
                
                GameObject bullet = Instantiate(prefab, transform.position, Quaternion.identity);
                objList.Add(bullet);

                bullet.transform.position = rotVec * targetToMissileDist;
            }
        }
        for (int i = 0; i < objList.Count; i++)
        {
            objList[i].transform.RotateAround(transform.position,Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}
