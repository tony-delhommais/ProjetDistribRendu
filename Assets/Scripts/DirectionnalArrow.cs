using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionnalArrow : MonoBehaviour
{
    [SerializeField] private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = target.transform.position;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        transform.Rotate(-90 , 180, 0);
    }
}
