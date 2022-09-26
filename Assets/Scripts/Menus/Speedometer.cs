using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Speedometer : MonoBehaviour
{
    private const float MAX_SPEED_ANGLE = -180;
    private const float ZERO_SPEED_ANGLE = 80f;
    
    [SerializeField] private Transform needle;

    [SerializeField] private TextMeshProUGUI speedTmpro;
    [SerializeField] private Image turboImage;
    [SerializeField] GameObject gameUI;
    private Car _car;
    
    private float speedMax;
    private float speed;
    
    
    
    private void Awake()
    {
        _car = FindObjectOfType<Car>();

        speed = 0f;
        speedMax = 100f;
    }

    private void Start()
    {
        StartCoroutine(UpdatePhysicsText());
    }

    private void Update()
    {
        if (_car.GetCurrentSpeed > speedMax+20) speed = speedMax;
        else speed = _car.GetCurrentSpeed;
        turboImage.fillAmount = _car.GetCurrentTurbo / _car.GetTurboLength;
        needle.eulerAngles = new Vector3(0, 0, GetSpeedRotation());
    }

    IEnumerator UpdatePhysicsText()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            speedTmpro.text = ((int)_car.GetCurrentSpeed).ToString(CultureInfo.InvariantCulture);
        }
    }

    private float GetSpeedRotation()
    {
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;
        float speedNormalized = speed /  speedMax;

        return ZERO_SPEED_ANGLE - speedNormalized * totalAngleSize;
    }

    public void HideUI()
    {
        gameUI.SetActive(false);
    }

    public void ShowUI()
    {
        gameUI.SetActive(true);
    }


}
