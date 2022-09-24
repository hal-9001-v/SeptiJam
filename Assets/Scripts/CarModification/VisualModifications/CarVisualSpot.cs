using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarVisualSpot : MonoBehaviour
{
    [SerializeField]
    private CarAccessoryType accesoryType;

    //References
    private MeshRenderer myMeshRenderer;
    private MeshFilter myMeshFilter;

    //We will have defaults??

    private void Awake()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        myMeshFilter = GetComponent<MeshFilter>();
        CarModificationManager.OnCarModification -= OnCarChanged;
        CarModificationManager.OnCarModification += OnCarChanged;
    }
    private void OnCarChanged(CarAccessoryType type, CarAccessory accessory)
    {
        if(accesoryType != type)
        {
            return;
        }
        if(accessory == null)
        {
            myMeshRenderer.enabled = false;
            myMeshFilter.mesh = null;
        }
        else
        {
            myMeshRenderer.enabled = true;
            myMeshRenderer.materials = accessory.AccesoryInformation.MeshMaterials;
            myMeshFilter.mesh = accessory.AccesoryInformation.AccessoryMesh;
            Bounds customBounds = myMeshFilter.mesh.bounds;
            customBounds.size = new Vector3(1, 1, 1);
            myMeshFilter.mesh.bounds = customBounds;
        }
  
    }
    private void ChangeOrientation(CarAccessoryInPositionInfo accessoryInfo)
    {
        switch (accessoryInfo.ForwardInPosition)
        {
            case AccessoryOrientation.XPositive:
                break;
            case AccessoryOrientation.YPositive:
                break;
            case AccessoryOrientation.ZPositive:
                break;
            case AccessoryOrientation.XNegative:
                break;
            case AccessoryOrientation.YNegative:
                break;
            case AccessoryOrientation.ZNegative:
                break;
        }
    }
}
