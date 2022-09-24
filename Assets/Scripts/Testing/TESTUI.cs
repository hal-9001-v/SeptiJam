using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

    public class TESTUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI velText;
        [SerializeField] TextMeshProUGUI massText;
        [SerializeField] Slider slider;
        [SerializeField] Car car;
        
        private float speed;
        
        private void Start()
        {
            StartCoroutine(UpdateUI());
        }

        private void FixedUpdate()
        {
            slider.value = car.GetCurrentTurbo / car.GetTurboLength;
            massText.text = car.GetCurrentWeight.ToString(CultureInfo.InvariantCulture);
        }

        
        IEnumerator UpdateUI()
         {
             while (true)
             {
                 yield return new WaitForSeconds(0.1f);
                 velText.text = ((int)car.GetCurrentSpeed).ToString(CultureInfo.InvariantCulture);
             }
         }



    }
