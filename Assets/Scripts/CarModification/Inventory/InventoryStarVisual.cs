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

    [SerializeField]
    private Image upgradeImage;

    [SerializeField]
    private Sprite upgradeSprite;

    [SerializeField]
    private Sprite downgradeSprite;

    private int currentStars = -1;
    private float currentValue = -1;
    private void Awake()
    {
        CarModificationManager.OnCarUpdate -= OnUpdate;
        CarModificationManager.OnCarUpdate += OnUpdate;
        CarModificationManager.OnAccesorySelectedAction -= OnAccessorySelected;
        CarModificationManager.OnAccesorySelectedAction += OnAccessorySelected;
        CarModificationManager.OnAccessoryDeselected -= OnDeselected;
        CarModificationManager.OnAccessoryDeselected += OnDeselected;
    }
    private void OnDestroy()
    {
        CarModificationManager.OnCarUpdate -= OnUpdate;
        CarModificationManager.OnAccesorySelectedAction -= OnAccessorySelected;
        CarModificationManager.OnAccessoryDeselected -= OnDeselected;
    }
    private void OnDeselected()
    {
        upgradeImage.enabled = false;
    }
    public void OnAccessorySelected(Car car, CarAccessory accessory, Car.CarModifierInfo info, CarAccessoryType position,CarModifier[] oldAccessoryModifiers)
    {
        if (position.Equals(CarAccessoryType.None))
        {
            OnDeselected();
            return;
        }
        for (int i = 0; i < accessory.ModifiersInPosition(position).Length; i++)
        {
            switch (accessory.ModifiersInPosition(position)[i].ParameterType)
            {
                case CarVarsType.Weight:
                    info.carMass += accessory.ModifiersInPosition(position)[i].ModificationValue * CarAttribute.CAR_VAR_MULTIPLIERS[CarVarsType.Weight];
                    break;
                case CarVarsType.SteerAngle:
                    info.steerAngle += accessory.ModifiersInPosition(position)[i].ModificationValue * CarAttribute.CAR_VAR_MULTIPLIERS[CarVarsType.SteerAngle];
                    break;
                case CarVarsType.MotorForce:
                    info.motorForce += accessory.ModifiersInPosition(position)[i].ModificationValue * CarAttribute.CAR_VAR_MULTIPLIERS[CarVarsType.MotorForce];
                    break;
                case CarVarsType.Turbo:
                    info.turboLength += accessory.ModifiersInPosition(position)[i].ModificationValue * CarAttribute.CAR_VAR_MULTIPLIERS[CarVarsType.Turbo];
                    break;
            }
        }
        if (oldAccessoryModifiers != null)
        {
            for (int i = 0; i < oldAccessoryModifiers.Length; i++)
            {
                switch (oldAccessoryModifiers[i].ParameterType)
                {
                    case CarVarsType.Weight:
                        info.carMass -= oldAccessoryModifiers[i].ModificationValue * CarAttribute.CAR_VAR_MULTIPLIERS[CarVarsType.Weight];
                        break;
                    case CarVarsType.SteerAngle:
                        info.steerAngle -= oldAccessoryModifiers[i].ModificationValue * CarAttribute.CAR_VAR_MULTIPLIERS[CarVarsType.SteerAngle];
                        break;
                    case CarVarsType.MotorForce:
                        info.motorForce -= oldAccessoryModifiers[i].ModificationValue * CarAttribute.CAR_VAR_MULTIPLIERS[CarVarsType.MotorForce];
                        break;
                    case CarVarsType.Turbo:
                        info.turboLength -= oldAccessoryModifiers[i].ModificationValue * CarAttribute.CAR_VAR_MULTIPLIERS[CarVarsType.Turbo];
                        break;
                }
            }
        }
      
        int stars = -1;
        float value = -1;
        switch (thisStatistics)
        {
            case StarStatistic.Steer:
                stars = car.GetSteerStarssAux(info);
                break;
            case StarStatistic.Acceleration:
                stars = car.GetAccelerationStarsAux(info);
                break;
            case StarStatistic.Speed:
                stars = car.GetSpeedStarssAux(info);
                break;
            case StarStatistic.Mass:
                stars = car.GetMassStarssAux(info);
                break;
            case StarStatistic.MaxSpeed:
                value = car.GetMaxVelocityAux(info);
                break;
            case StarStatistic.Turbo:
                value = Mathf.Round(info.turboLength * 10) / 10;
                break;
        }
        if (stars >= 0)
        {
            if (stars > currentStars)
            {
                upgradeImage.sprite = upgradeSprite;
                upgradeImage.enabled = true;
            }
            else if (stars < currentStars)
            {
                upgradeImage.sprite = downgradeSprite;
                upgradeImage.enabled = true;
            }
            else
            {
                OnDeselected();
            }
        }
        if (value >= 0)
        {
            if (value > currentValue)
            {
                upgradeImage.sprite = upgradeSprite;
                upgradeImage.enabled = true;
            }
            else if (value < currentValue)
            {
                upgradeImage.sprite = downgradeSprite;
                upgradeImage.enabled = true;
            }
            else
            {
                OnDeselected();
            }
        }

    }
    public void OnUpdate(Car car)
    {
        int stars = -1;
        float value = -1;
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
        for (int i = 0; i < starImagesReferences.Length; i++)
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
