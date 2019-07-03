using System.Collections;
using System.Collections.Generic;
using Engine.Effect;
using TMPro;
using UnityEngine;
/// <summary>
/// @author Lpy
/// 临时测试的角色
/// </summary>
public class Thero : MonoBehaviour {
    #region 单例

    private Thero()
    {
    }

    public static Thero instance;

    public Transform m_shootTrans;
    #endregion

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    CheckLerp();
	    if (Input.GetKeyDown(KeyCode.T))
	    {
	        Attack(0);
	    }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Attack(1);
        }
    }

    #region 移动

    public float m_MoveSpeed = 2;
    public float m_RotatSpeed = 5;
    /// <summary>
    /// 位置移动
    /// </summary>
    /// <param name="p_dir"></param>
    public void PositionMove(Vector3 p_dir,int p_times=0)
    {
        Vector3 t_pos = transform.position + p_dir* m_MoveSpeed*Time.deltaTime;
        float t_height= MapManager.instance.GetHeight(t_pos);
       
        if (t_height>=0)
        {
          //  Debug.LogError(t_height + " ===== " + (t_height>=0) + "   " + Time.frameCount);
            t_pos.y = t_height;
            transform.position = t_pos;
        }
        else
        {
            Vector3 t_result = GetSplitMoveDir(p_dir);
            if (t_result != Vector3.zero)
            {
                if (p_times==0)
                {
                    PositionMove(t_result,1);
                }
            }
        }
    }

    Vector3 GetSplitMoveDir(Vector3 p_dir)
    {
        Vector3 t_normal= MapManager.instance.GetHitNormal(transform.position + Vector3.up * 0.2f, p_dir);
        if (VectorUtils.IsVectorEquals(t_normal, Vector3.zero,4))
        {
            return Vector3.zero;
        }
        Vector3 firstLeft = Vector3.Cross(p_dir, t_normal);
        if (VectorUtils.IsVectorEquals(firstLeft, Vector3.zero,4))
        {
            //垂直了，做个偏移
            firstLeft= Vector3.Cross(p_dir+ transform.right*0.1f, t_normal);
        }
        Vector3 t_dir = Vector3.Cross(t_normal, firstLeft).normalized;
        return t_dir;
    }

    private Vector3 m_targetDir;

    private bool m_needLerp;
    /// <summary>
    /// 方向移动
    /// </summary>
    /// <param name="p_dir"></param>
    public void RotationMove(Vector3 p_dir)
    {
        m_targetDir = p_dir;
        m_needLerp = true;
      
    }

    void CheckLerp()
    {
        if (m_needLerp)
        {
            Quaternion t_targetRot = Quaternion.LookRotation(m_targetDir);
            if (Mathf.Abs(transform.eulerAngles.y- t_targetRot.eulerAngles.y) >0.01f)
            {
                float rotY = Quaternion.Slerp(transform.rotation, t_targetRot, m_RotatSpeed * Time.deltaTime).eulerAngles.y;
                transform.eulerAngles=new Vector3(0, rotY, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, t_targetRot.eulerAngles.y, 0);
                m_needLerp = false;
            }
        }
    }

    #endregion

    #region 攻击

    public void Attack(int p_index)
    {
        //1.直接播放特效即可
        EffectShareData t_data= new EffectShareData();
        t_data.m_bornTrans = m_shootTrans;
        EffectManager.instance.CreatEffectByController(p_index, t_data);
    }


    #endregion
}
