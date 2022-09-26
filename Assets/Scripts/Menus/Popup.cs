using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        CarParts carpart;
        

        
    }

    public void DisablePopup()
    {
        popupImage.gameObject.SetActive(false);
    }
    
    public void ChangePopup(CarParts car)
    {
        switch (car){
                case CarParts.anchor:
                    popupImage.sprite = images[0];
                break;
                case CarParts.barrel:
                    popupImage.sprite = images[0];
                break;
                case CarParts.beachball:
                    popupImage.sprite = images[0];
                break;
                case CarParts.bowlingball:
                    popupImage.sprite = images[0];
                break;
                case CarParts.butane:
                    popupImage.sprite = images[0];
                break;
                case CarParts.coconut:
                    popupImage.sprite = images[0];
                break;
                case CarParts.colacao:
                    popupImage.sprite = images[0];
                break;
                case CarParts.fan:
                    popupImage.sprite = images[0];
                break;
                case CarParts.flemish:
                    popupImage.sprite = images[0];
                break;
                case CarParts.floater:
                    popupImage.sprite = images[0];
                break;
                case CarParts.hamsterball:
                    popupImage.sprite = images[0];
                break;
                case CarParts.hooka:
                    popupImage.sprite = images[0];
                break;
                case CarParts.jagger:
                    popupImage.sprite = images[0];
                break;
                case CarParts.laptop:
                    popupImage.sprite = images[0];
                break;
                case CarParts.madrid:
                    popupImage.sprite = images[0];
                break;
                case CarParts.mercadona:
                    popupImage.sprite = images[0];
                break;
                case CarParts.microwave:
                    popupImage.sprite = images[0];
                break;
                case CarParts.monster:
                    popupImage.sprite = images[0];
                break;
                case CarParts.nokia:
                    popupImage.sprite = images[0];
                break;
                case CarParts.oliveoil:
                    popupImage.sprite = images[0];
                break;
                case CarParts.paellera:
                    popupImage.sprite = images[0];
                break;
                case CarParts.playcontrol:
                    popupImage.sprite = images[0];
                break;
                case CarParts.potato:
                    popupImage.sprite = images[0];
                break;
                case CarParts.rtx:
                    popupImage.sprite = images[0];
                break;
                case CarParts.rubberchicken:
                    popupImage.sprite = images[0];
                    break;
                case CarParts.rudder:
                    popupImage.sprite = images[0];
                break;
                case CarParts.shovel:
                    popupImage.sprite = images[0];
                break;
                case CarParts.surftable:
                    popupImage.sprite = images[0];
                break;
                case CarParts.tofu:
                    popupImage.sprite = images[0];
                break;
                case CarParts.wine:
                    popupImage.sprite = images[0];
                break;
        }
    }
    
     public void ChangePopupEng(CarParts car)
    {
        switch (car){
                case CarParts.anchor:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.barrel:
                    popupImage.sprite = images[0];
                break;
                case CarParts.beachball:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.bowlingball:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.butane:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.coconut:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.colacao:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.fan:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.flemish:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.floater:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.hamsterball:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.hooka:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.jagger:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.laptop:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.madrid:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.mercadona:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.microwave:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.monster:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.nokia:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.oliveoil:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.paellera:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.playcontrol:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.potato:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.rtx:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.rubberchicken:
                    popupImage.sprite = engLishimages[0];
                    break;
                case CarParts.rudder:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.shovel:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.surftable:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.tofu:
                    popupImage.sprite = engLishimages[0];
                break;
                case CarParts.wine:
                    popupImage.sprite = engLishimages[0];
                break;
        }
    }
    
    
}
