using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 移动控制
/// </summary>
public class MoveComp : SpriteComp
{
    #region 基本


    public override void OnAdded()
    {
    }

    public override void OnRemoved()
    {
        // TTicker.RemoveUpdate(OnUpdate);
    }
    #endregion

    /// <summary>
    /// 位置移动,p_times次数，0表示第一次，1表示切边之后
    /// </summary>
    /// <param name="p_dir"></param>
    public void PositionMove(Vector3 p_dir, int p_times = 0)
    {

        Vector3 t_pos = sprite.m_spacialComp.worldPosi + p_dir * m_opreateData.m_moveSpeed * Time.deltaTime;
        float t_height = MapManager.instance.GetHeight(t_pos);

        if (t_height >= 0)
        {
            t_pos.y = t_height;
            sprite.m_spacialComp.worldPosi = t_pos;
        }
        else
        {
            Vector3 t_result = GetSplitMoveDir(p_dir);
            if (t_result != Vector3.zero)
            {
                if (p_times == 0)
                {
                    PositionMove(t_result, 1);
                }
            }
        }
    }
    Vector3 GetSplitMoveDir(Vector3 p_dir)
    {
        Vector3 t_normal = MapManager.instance.GetHitNormal(sprite.transform.position + Vector3.up * 0.2f, p_dir);
        if (VectorUtils.IsVectorEquals(t_normal, Vector3.zero, 4))
        {
            return Vector3.zero;
        }
        Vector3 firstLeft = Vector3.Cross(p_dir, t_normal);
        if (VectorUtils.IsVectorEquals(firstLeft, Vector3.zero, 4))
        {
            //垂直了，做个偏移
            firstLeft = Vector3.Cross(p_dir + sprite.transform.right * 0.1f, t_normal);
        }
        Vector3 t_dir = Vector3.Cross(t_normal, firstLeft).normalized;
        return t_dir;
    }

    public void RotationMove(Vector3 p_dir)
    {
        sprite.m_spacialComp.FaceToDir(p_dir, true);
    }
    #region 快捷引用

    private OperateData _opreateData;

    private OperateData m_opreateData
    {
        get
        {
            if (_opreateData == null)
            {
                _opreateData = sprite.m_data.GetData<OperateData>();
            }

            return _opreateData;
        }
    }


    #endregion
}
