﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{
    public Text scoreText;

    /*// Start is called before the first frame update
    void Start()
    {
        
    }*/

    public void UpdateText()
    {
        scoreText.text = "" + GameManager.Get().score;
    }
}
