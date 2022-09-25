using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableItem))]

public class CarPartItem : MonoBehaviour
{
    PickableItem pickableItem => GetComponent<PickableItem>();
    [SerializeField] CarAccessory carAccesory;

    private void Awake()
    {
        pickableItem.collectedCallback += AddCarPart;
        carAccesory = GetComponent<CarAccessory>();
    }


    void AddCarPart(ItemCollector collector)
    {
        collector.AddCarPart(carAccesory);

        pickableItem.StandardHide();
    }

}
