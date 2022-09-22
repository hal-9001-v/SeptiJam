using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class TESTUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI velText;
        [SerializeField] TextMeshProUGUI massText;
        [SerializeField] Slider slider;
        [SerializeField] Car car;
    

        private void Update()
        {
            velText.text = car.GetCurrentSpeed.ToString(CultureInfo.InvariantCulture);
            massText.text = car.GetCurrentWeight.ToString(CultureInfo.InvariantCulture);
            slider.value = car.GetCurrentTurbo;
        }
    }
