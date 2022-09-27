using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class ColorChoose : MonoBehaviour
{
    [SerializeField] private GameObject character;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Button>().onClick.AddListener(ChangeColor);
    }
    void ChangeColor()
    {
        character.GetComponentInChildren<Renderer>().material.color = GetComponent<Image>().color; 
    }
}
