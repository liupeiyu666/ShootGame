using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedUtilities.Cameras;
using AdvancedUtilities.Cameras.Components;
using UnityEngine;
/// <summary>
/// 输入控制器
/// </summary>
public class GameInput:MonoBehaviour {
    #region 单例

    private GameInput()
    {
    }

    private static GameInput _instance;

    public static GameInput instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameInput();
            }

            return _instance;
        }
    }

    #endregion

    public ETCJoystick m_leftJoyStick;

    public ETCJoystick m_rightJoySitck;

    public BasicCameraController m_cameraController;

    private Action<Vector3> m_cameraOffsetAct;
	// Use this for initialization
	void Start () {
	    m_leftJoyStick.onMove.AddListener(OnLeftJoyStickMove);
	    m_leftJoyStick.onMoveEnd.AddListener(OnLeftJoystickMoveEnd);

	    m_rightJoySitck.onMove.AddListener(OnRightJoyStickMove);
	    m_rightJoySitck.onMoveEnd.AddListener(OnRightJoystickMoveEnd);

	    m_cameraOffsetAct= m_cameraController.GetCameraComponent<CameraOffsetComponent>().OnOffset;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private bool m_isLeftMove, m_isRightMove;
    void OnLeftJoyStickMove(Vector2 joystickMove)
    {
        m_isLeftMove = true;
        Vector3 t_dir = GetWorldXOZDirFromJoystickMove(Camera.main.transform, joystickMove);
        //只负责位置移动
        Thero.instance.PositionMove(t_dir);
        if (!m_isRightMove)
        {
            Thero.instance.RotationMove(t_dir);
        }
    }
    void OnLeftJoystickMoveEnd()
    {
        m_isLeftMove = false;
    }

    void OnRightJoyStickMove(Vector2 joystickMove)
    {
        
        m_isRightMove = true;

        Vector2 t_tempMove = new Vector2(joystickMove.x, -joystickMove.y);
        Vector3 t_dir = GetWorldXOZDirFromJoystickMove(Camera.main.transform, t_tempMove);
        if (m_isLeftMove)
        {
            Thero.instance.RotationMove(t_dir);
        }
        
        //设置摄像机偏移
        if (m_cameraOffsetAct!=null)
        {
            m_cameraOffsetAct(t_dir);
        }
    }
    void OnRightJoystickMoveEnd()
    {
        m_isRightMove = false;
    }
    public Vector3 GetWorldXOZDirFromJoystickMove(Transform camTrm, Vector2 joystickMove)
    {

        Vector3 xozDir = Vector3.zero;
        if (camTrm != null)
        {
            float v = joystickMove.y;
            float h = joystickMove.x;
            // calculate camera relative direction to move:
            //算出摄像机在xoz平面上的forward,摄像机的射线的指向
            Vector3 forward = Vector3.Scale(camTrm.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 right = camTrm.right;
            xozDir = v * forward + h * right;
        }
        return xozDir;
    }

}
