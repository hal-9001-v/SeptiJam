using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableItem))]

public class CarPartItem : MonoBehaviour
{
    public float amplitud;
    private float velocidad;
    private float tiempo;
    private float tiempoAcumulado;
    private bool bajada;
    PickableItem pickableItem => GetComponent<PickableItem>();
    [SerializeField] CarAccessory carAccesory;

    private void Update()
    {
        if (bajada)
        {
            transform.position += Vector3.up * velocidad * Time.deltaTime;
        }
        else
        {
            transform.position -= Vector3.up * velocidad * Time.deltaTime;

        }

        tiempoAcumulado += Time.deltaTime;

        if (tiempoAcumulado >= tiempo)
        {
            tiempoAcumulado = 0;
            bajada = !bajada;
        }

    }

    private void Awake()
    {
        tiempo = amplitud / velocidad;
        pickableItem.collectedCallback += AddCarPart;
        carAccesory = GetComponent<CarAccessory>();
    }


    void AddCarPart(ItemCollector collector)
    {
        collector.AddCarPart(carAccesory);

        pickableItem.StandardHide();
    }

}
