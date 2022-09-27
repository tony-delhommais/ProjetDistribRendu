using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class ColorChoose : MonoBehaviour
{
    [SerializeField] private GameObject character;
    MenuScript menuScript;

    private void Awake()
    {
        menuScript = GameObject.FindGameObjectWithTag("Canca").GetComponent<MenuScript>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Button>().onClick.AddListener(ChangeColor);
    }
    void ChangeColor()
    {
        character.GetComponentInChildren<PlayerColor>().materialColor = GetComponent<Image>().color;

        if (menuScript) menuScript.targetPlayerColor = GetComponent<Image>().color;
    }
}
