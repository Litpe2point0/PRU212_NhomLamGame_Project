using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerOfDeathSummon : MonoBehaviour
{
    [Header("Summon Parameter")]
    [SerializeField] private GameObject[] portalPrefabs;
    public void Summon()
    {
        for (int i = 0; i < portalPrefabs.Length; i++)
        {
            portalPrefabs[i].SetActive(true);
            portalPrefabs[i].GetComponent<Portal>().SetActivate();
        }
    }
}
