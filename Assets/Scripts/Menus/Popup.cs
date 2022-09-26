using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
public enum CarParts
{
    anchor,
    barrel,
    beachball,
    bowlingball,
    butane,
    coconut,
    colacao,
    fan,
    flemish,
    floater,
    hamsterball,
    hooka,
    jagger,
    laptop,
    madrid,
    mercadona,
    microwave,
    monster,
    nokia,
    oliveoil,
    paellera,
    playcontrol,
    potato,
    rtx,
    rubberchicken,
    rudder,
    shovel,
    surftable,
    tofu,
    wine

}


public class Popup : MonoBehaviour
{
    public Sprite[] images;
    public Sprite[] engLishimages;
    public Image popupImage;
    public string name;


    private void Awake()
    {
        DisablePopup();
    }

    public void EnablePopup(CarAccessory accessory)
    {
        Language lang = FindObjectOfType<LanguageNotifier>().CurrentLanguage;
        if(lang == Language.English)
        {
            ChangePopupEng(accessory.AccesoryInformation.carpart);
            name = accessory.AccesoryInformation.englishName;
        }
        else
        {
            ChangePopup(accessory.AccesoryInformation.carpart);
            name = accessory.AccesoryInformation.spanishName;
        }

        popupImage.gameObject.SetActive(true);


    }

    public void DisablePopup()
    {
        popupImage.gameObject.SetActive(false);
    }
    
    public void ChangePopup(CarParts car)
    {
        Debug.Log(car);
        popupImage.sprite = images[(int)car+1];

    }
    
     public void ChangePopupEng(CarParts car)
    {
        popupImage.sprite = engLishimages[(int)car+1];
    }
    
    
}
