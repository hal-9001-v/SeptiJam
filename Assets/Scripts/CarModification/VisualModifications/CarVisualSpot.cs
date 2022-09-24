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

    private  Vector3[] orientations = { new Vector3 (1, 0, 0 ), new Vector3(0, 1, 0), new Vector3(0, 0, 1), new Vector3(-1, 0, 0), new Vector3(0, -1, 0), new Vector3(0, 0, -1), };


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
            //transform.rotation = accessory.AccesoryInformation.OriginalTransform.rotation;
            
            transform.localScale = accessory.AccesoryInformation.OriginalTransform.localScale;
            //Bounds customBounds = myMeshFilter.mesh.bounds;
            //customBounds.size = new Vector3(1, 1, 1);
            //myMeshFilter.mesh.bounds = customBounds;
            ChangeOrientation(accessory.GetOrientationInPosition(type));
        }
  
    }
    private void ChangeOrientation(AccessoryOrientation forward)
    {
        //transform.forward =  orientations[(int)forward];
        transform.forward = transform.TransformDirection(orientations[(int)forward]);
        //transform.forward = transform.TransformDirection(orientations[(int)forward+1]);
        //transform.forward = transform.TransformDirection(orientations[(int)forward+2]);
        //transform.localRotation.SetLookRotation(orientations[(int)forward]);
    }
    private void OnDestroy()
    {
        CarModificationManager.OnCarModification -= OnCarChanged;
    }
}
