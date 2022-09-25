using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableItem))]

public class CarPartItem : MonoBehaviour
{

    //[SerializeField] CarPartScriptableObject carPart;
    PickableItem pickableItem => GetComponent<PickableItem>();
    CarAccessory carAccesory;

    private void Awake()
    {
        pickableItem.collectedCallback += AddCarPart;
        carAccesory = GetComponent<CarAccessory>();
    }


    void AddCarPart(ItemCollector collector)
    {
        //collector.AddCarPart(carPart);
        collector.AddCarPart(carAccesory);

        pickableItem.StandardHide();
    }

}
