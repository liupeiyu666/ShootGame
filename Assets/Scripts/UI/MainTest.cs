using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MainTest:MonoBehaviour
{
    public Button m_shootOne;
    public Button m_shootTwo;
    void Start()
    {
        m_shootOne.onClick.AddListener(delegate() {THero.instance.Attack(1); });

        m_shootTwo.onClick.AddListener(delegate () { THero.instance.Attack(2); });

    }
    void Update()
    {

    }

}