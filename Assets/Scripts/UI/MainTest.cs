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
        m_shootOne.onClick.AddListener(delegate() {Thero.instance.Attack(0); });

        m_shootTwo.onClick.AddListener(delegate () { Thero.instance.Attack(1); });
    }
}