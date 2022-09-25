using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryStarVisual : MonoBehaviour
{
    internal enum StarStatistic
    {
        Steer,
        Acceleration,
        Speed,
        Mass,
        MaxSpeed,
        Turbo
    }


    [SerializeField]
    private StarStatistic thisStatistics;

    [SerializeField]
    private TMP_Text infoText;

    [SerializeField]
    private Image[] starImagesReferences = new Image[5];

    private int currentStars= -1;
    private float currentValue =-1;
    private void Awake()
    {
        CarModificationManager.OnCarUpdate -= OnUpdate;
        CarModificationManager.OnCarUpdate += OnUpdate;
    }
    private void OnDestroy()
    {
        CarModificationManager.OnCarUpdate -= OnUpdate;
    }

    public void OnUpdate(Car car)
    {
        int stars = -1;
        float value  = -1;
        switch (thisStatistics)
        {
            case StarStatistic.Steer:
                stars = car.GetSteerStars();
                break;
            case StarStatistic.Acceleration:
                stars = car.GetAccelerationStars();
                break;
            case StarStatistic.Speed:
                stars = car.GetSpeedStars();
                break;
            case StarStatistic.Mass:
                stars = car.GetMassStars();
                break;
            case StarStatistic.MaxSpeed:
                value = car.GetMaxVelocity();
                break;
            case StarStatistic.Turbo:
                value = Mathf.Round(car.GetCurrentTurbo * 10) / 10;
                break;
        }
        OnStars(stars);
        OnValue(value);
    }
    private void OnStars(int stars)
    {
        if (stars < 0)
        {
            return;
        }
        for(int i =0; i < starImagesReferences.Length; i++)
        {
            starImagesReferences[i].enabled = (i < stars);
        }
        currentStars = stars;
    }
    private void OnValue(float value)
    {
        if (value < 0)
        {
            return;
        }
        infoText.text = value.ToString(CultureInfo.InvariantCulture);
        currentValue = value;
    }
}
