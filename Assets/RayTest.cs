using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tgame.Game.Table;
using ZTool.Table;

public class RayTest : MonoBehaviour {

    // Use this for initialization
    public LayerMask m_layerMask;

    public List<int> m_test=new List<int>();
    void Start ()
    {
        TableManager.instance.StartLoad();
       
        
        int t_mask = 0;
	    //for (int i = 0; i < m_test.Count; i++)
	    //{
	    //    t_mask|=1 << m_test[i];

	    //}

     //   m_layerMask = t_mask;
        // m_layerMask |= 12;

    }
	
	// Update is called once per frame
	void Update () {
      
        //transform.Rotate(transform.up,30f);
        //Ray ray = new Ray();
        //ray.origin = transform.position;
        //ray.direction = transform.forward;

        //RaycastHit hit;
        //Debug.DrawLine(ray.origin, ray.origin+ ray.direction.normalized * 2, Color.blue, 2f);
        //if (Physics.Raycast(ray, out hit, 1000, m_layerMask))
        //{
        //    Debug.LogError(hit.transform.name);
        //}
    }
}
