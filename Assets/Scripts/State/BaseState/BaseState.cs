using UnityEngine;

public class BaseState : SpriteFSMState
{
    public BaseState() {}

    #region 快捷引用

    private AnimComp _animComp;

    protected AnimComp anim
    {
        get {
            if (_animComp == null)
            {
                _animComp = sprite.GetComp<AnimComp>();
            }
            return _animComp;
        }
    }



    #endregion
}
