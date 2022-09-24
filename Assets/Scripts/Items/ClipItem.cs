using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableItem))]
public class ClipItem : MonoBehaviour
{
    [SerializeField] RadioClip clip;
    PickableItem pickableItem => GetComponent<PickableItem>();

    private void Awake()
    {
        pickableItem.collectedCallback += AddClip;
    }


    void AddClip(ItemCollector collector)
    {
        collector.AddClip(clip);

        pickableItem.StandardHide();
    }
}
