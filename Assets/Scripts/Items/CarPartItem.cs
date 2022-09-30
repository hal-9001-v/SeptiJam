using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickableItem))]

public class CarPartItem : MonoBehaviour
{
    [Header("Settings")]
    public float amplitud;
    public float velocidad;
    private float tiempo;
    private float tiempoAcumulado;
    private bool bajada;

    [Header("Settings")]
    [SerializeField] SoundInfo pickedSound;

    PickableItem pickableItem => GetComponent<PickableItem>();
    [SerializeField] CarAccessory carAccesory;

    void Start()
    {
        pickedSound.Initialize(gameObject);

        tiempo = amplitud / velocidad;
        pickableItem.collectedCallback += AddCarPart;

        carAccesory = GetComponent<CarAccessory>();


    }

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


    void AddCarPart(ItemCollector collector)
    {
        collector.AddCarPart(carAccesory);

        pickableItem.StandardHide();
        pickedSound.Play();
    }

}
