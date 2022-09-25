using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

    public class TESTUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI accText;
        [SerializeField] TextMeshProUGUI speedText;
        [SerializeField] TextMeshProUGUI massText;
        [SerializeField] TextMeshProUGUI velText;
        [SerializeField] TextMeshProUGUI steerText;
        [SerializeField] TextMeshProUGUI maxVelText;
        [SerializeField] TextMeshProUGUI turboText;
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
            massText.text = car.GetMassStars().ToString();
            accText.text = car.GetAccelerationStars().ToString();
            velText.text = car.GetSpeedStars().ToString();
            steerText.text = car.GetSteerStars().ToString();
            maxVelText.text = ((int)car.GetMaxVelocity()).ToString();
            turboText.text = (Mathf.Round(car.GetCurrentTurbo * 10)/10).ToString(CultureInfo.InvariantCulture);
        }
        

        IEnumerator UpdateUI()
         {
             while (true)
             {
                 yield return new WaitForSeconds(0.1f);
                 speedText.text = ((int)car.GetCurrentSpeed).ToString(CultureInfo.InvariantCulture);
             }
         }



    }
