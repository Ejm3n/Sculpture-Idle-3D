using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardObject : MonoBehaviour
{
    //[SerializeField] private Quaternion offset = Quaternion.identity;
    private void LateUpdate()
    {
        transform.LookAt(CinemachineBrain.Default.transform);
    }
}
