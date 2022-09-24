using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableItem))]

public class CarPartItem : MonoBehaviour
{

    //[SerializeField] CarPartScriptableObject carPart;
    PickableItem pickableItem => GetComponent<PickableItem>();

    private void Awake()
    {
        pickableItem.collectedCallback += AddCarPart;
    }


    void AddCarPart(ItemCollector collector)
    {
        //collector.AddCarPart(carPart);
        collector.AddCarPart();

        pickableItem.StandardHide();
    }

}
