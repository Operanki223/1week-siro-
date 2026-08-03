using System;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] int savePointNum = 0;
    void OnTriggerEnter(Collider other)
    {
        GameManager.instance.savePointNum = savePointNum;
    }
}
