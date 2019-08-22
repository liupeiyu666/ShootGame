using System;
using System.Collections.Generic;

/// <summary>
/// 可重复利用Item的 List。
/// 需配合ReuseItem使用。
/// @author 程明阳
/// @date 2018/6/4 15:27:51
/// </summary>
//public class ReuseList<T> : List<T> where T : ReuseItem, new()
public class ReuseList<T> where T : ReuseItem, new()
{
    List<T> _list = new List<T>();

    public T GetFreeItem()
    {
        var returnValue = this.getExistFreeItem();
        if (returnValue == null)
        {
            this.expand();
            returnValue = this.getExistFreeItem();
        }

        returnValue.OnInit();
        _freeCount--;

        return returnValue;
    }

    public int Count
    {
        get
        {
            return _list.Count;
        }
    }

    public T this[int i]
    {
        get
        {
            return _list[i];
        }
        set
        {
            _list[i] = value;
        }
    }

    int _freeCount;

    public void Recycle(T t)
    {
        if (t != null)
        {
            if (_list.Contains(t))
            {
                if (!t.IsFree)
                {
                    t.OnRecycle();
                    _freeCount++;
                    checkDecreaseFreeItem();
                }
            }
        }
    }

    public void RecycleAt(int i)
    {
        this.Recycle(_list[i]);
    }

    /// <summary>
    /// free count小于n%，启动缩池。
    /// 一次检查只缩一半
    /// </summary>
    void checkDecreaseFreeItem()
    {
        //_freeCount记下来，避免每次都计算Count，导致的性能问题。
        if (_freeCount >= _list.Count * 0.75f && _list.Count > 4)
        {
            int decreaseCount = _list.Count / 2;
            var count = _list.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                if (_list[i].IsFree)
                {
                    _list.RemoveAt(i);

                    decreaseCount--;
                    _freeCount--;
                    if (decreaseCount <= 0)
                    {
                        return;
                    }
                }
            }
        }
    }

    T getExistFreeItem()
    {
        var count = this._list.Count;
        for (int i = 0; i < count; i++)
        {
            if (_list[i].IsFree)
            {
                return _list[i];
            }
        }

        return null;
    }

    void expand()
    {
        var addLen = _list.Count;
        if (addLen == 0)
        {
            addLen = 4;
        }

        for (int i = 0; i < addLen; i++)
        {
            _freeCount++;
            _list.Add(new T());
        }
    }
}

public class ReuseItem
{
    private bool _isFree = true;
    public bool IsFree
    {
        get { return _isFree; }
        private set { _isFree = value; }
    }

    public virtual void OnInit()
    {
        _isFree = false;
    }

    public virtual void OnRecycle()
    {
        _isFree = true;
    }
}
