using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarVisualSpot : MonoBehaviour
{
    [SerializeField]
    private CarAccessoryType accesoryType;

    private float scaleFactor;

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
        scaleFactor = transform.localScale.magnitude;
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
            //transform.rotation = accessory.AccesoryInformation.OriginalTransform.rotation;
            
            transform.localScale = accessory.AccesoryInformation.OriginalTransform.localScale*scaleFactor;
            //Bounds customBounds = myMeshFilter.mesh.bounds;
            //customBounds.size = new Vector3(1, 1, 1);
            //myMeshFilter.mesh.bounds = customBounds;
            ChangeOrientation(accessory.GetOrientationInPosition(type));
        }
  
    }
    private void ChangeOrientation(AccessoryOrientation forward)
    {
        switch (forward)
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
    private void OnDestroy()
    {
        CarModificationManager.OnCarModification -= OnCarChanged;
    }
}
