using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class TESTUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI velText;
        [SerializeField] TextMeshProUGUI massText;
        [SerializeField] private TextMeshProUGUI acccText;
        [SerializeField] Slider slider;
        [SerializeField] Car car;


        private Vector2 lastVelocity;

        private void FixedUpdate()
        {
            acccText.text = ((int)((car.GetCurrentVelocity - lastVelocity) / Time.deltaTime).magnitude).ToString();
            velText.text = ((int)car.GetCurrentSpeed).ToString(CultureInfo.InvariantCulture);
            massText.text = car.GetCurrentWeight.ToString(CultureInfo.InvariantCulture);
            slider.value = car.GetCurrentTurbo/car.GetTurboLength;

            lastVelocity = car.GetCurrentVelocity;
        }
    }
