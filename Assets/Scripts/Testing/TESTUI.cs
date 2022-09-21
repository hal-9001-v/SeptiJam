using System.Globalization;
using TMPro;
using UnityEngine;


    public class TESTUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI velText;
        [SerializeField] TextMeshProUGUI massText;

        [SerializeField] Car car;


        private void Update()
        {
            velText.text = car.Rigidbody.velocity.magnitude.ToString(CultureInfo.InvariantCulture);
            massText.text = car.Rigidbody.mass.ToString(CultureInfo.InvariantCulture);
        }
    }
