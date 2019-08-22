using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/**
 * 空间位置信息组件
 * 落地:y高度在当前地面
 * 平行于斜坡:
 * 转身
 * 
 * 用法类似值对象，用来直接设置各种值
 * 
 * 在ZOX平面，左手坐标系，注意,dir8=0表示负x轴方向
 * 落地
 * 四腿贴地：身体姿势随地形起伏
 * 
 * 处理落地和贴地
 * 在ZOX平面上，dir8的0为zx负轴，顺时针
 * 
 * 1,是否四腿贴地，由外部精灵来设置
 * 2,转身速度，由外部设置
 * 
 * 1119 kk+ 使用了dirty技术，原来30个精灵1.30ms,现在为0.3ms
 * 
 * TODO 历史遗留问题，npc的方向
 * 
 * 处理过jump组件的问题
 */
/// </summary>

public class SpacialComp : SpriteComp
{
	
	//设置的最大的Y变化值，之前是0.1 ，导致陡峭坡插值有问题
	const float MAX_DELTA_Y=0.3f;
	/// <summary>
    /// 基础移动速度，用来做角速度用
	/// </summary>
	public static float BASE_MOVE_SPEED=4.8f;

	//弹簧阻尼,使用弹簧位置，来实现平滑移动
	public bool useSmooth=false;
	private Vector3 smoothVelocity=Vector3.zero;
	public float smoothTime=0.1f;

	#region 基本

	//!!注意，挂second队列是有原因的，确保执行顺序(在所有组件最后执行)
	public override void OnAdded(){
		//UGEEnter.sprsTimeMgr.AddUpdate_Second(OnUpdate);
        TTicker.AddUpdate(OnUpdate);

	}

	public override void OnRemoved(){
		//UGEEnter.sprsTimeMgr.RemoveUpdate_Second(OnUpdate);
        TTicker.RemoveUpdate(OnUpdate);
	}
	#endregion

	//注意，如果是贴地时，要执行y插值
	bool _isFlying=false;//
    /// <summary>
    /// 空中飞行时，不需要考虑贴紧地面,也不需要对Y位移进行插值
    /// </summary>
	public bool isFlying{
		get{
			return _isFlying;
		}
		set{
			_isFlying=value;
			isDirty=true;
		}
	}
	
	//!!特殊处理，防止跑步时掉入坑内。为解决美术场景collider有裂缝的不规范的问题!!地面碰撞体由于美术制作中有裂缝，因为移动时做了一下插值，确保平滑
	//注意只在pathFolow的时候使用，这样可以平滑走动，否则就直接设置
	//bool needLerpPosiY=false;//1118 kk改 不再插值兼容地形错误，地形有裂缝有美术来摆好collider

	bool _is4Legs=false;
    /// <summary>
    /// 是否四条腿，四腿要贴地
    /// </summary>
	public bool is4Legs{
		get{
			return _is4Legs;
		}
		set{
			_is4Legs=value;
			isDirty=true;
		}
	}

    /// <summary>
    /// 基础转身速度,暴露给外部供外部设置
    /// </summary>
	public float rotationSpeed = 6f;

	float realRotationSpeed=6f;//真正的转身速度

    /// <summary>
    /// 刷新一下转身速度，如果移动速度变化的话
    /// </summary>
    /// <param name="moveSpeed">移动速度</param>
	public void RefreshRotationSpeed(float moveSpeed){
		realRotationSpeed=rotationSpeed*moveSpeed/BASE_MOVE_SPEED;
	}

	//位置或者旋转是否脏了，脏了才重新计算，因为Spacial.tUpdate有0.02秒的延迟
	bool isDirty=true;

    /// <summary>
    /// 1226 KK+ 解决人可以扎进阻挡点的bug而做，如果是hero则worldPosi的精度是小数点后3位(为了与障碍格扫描算法对上)
    /// </summary>
	public bool usePercision1000=false;

	//实际玩家移动中的位置
	Vector3 _worldPosi;
	
    /// <summary>
    /// 世界坐标
    /// </summary>
	public Vector3 worldPosi{
		get{
			return _worldPosi;
		}
		set{
			//-
			if(usePercision1000){
				value.x=(int)(value.x*1000)*0.001f;
				value.z=(int)(value.z*1000)*0.001f;
			}
            //Debug.LogError("---------_worldPosi:" + _worldPosi + "   " + value + "   DIS:" + Vector3.Distance(_worldPosi,value));
			//-
			_worldPosi=value;

			//-
			isDirty=true;
		}
	}
	
    /// <summary>
    /// 在格子的位置,曾经在Vector3自动转换Vector2时出过问题，y丢失，千万注意！
    /// </summary>
	public Vector2 gridPosiV2{
		get{
			return new Vector2((int)(_worldPosi.x),(int)(_worldPosi.z));
		}
	}
	
    /// <summary>
    /// 在格子的位置
    /// </summary>
	public Vector3 gridPosiV3{
		get{
			return new Vector3((int)(_worldPosi.x),0,(int)(_worldPosi.z));
		}
	}

    /// <summary>
    /// 注意dir8月zox的坐标系是有偏差的
    /// </summary>
	public int dir8{
		set{
			rotY = GetAngle(value);
		}
		get{
			return GetDir8(sprite.transform.eulerAngles.y);
		}
	}

	/// <summary>
	/// 提取当前的groundY,因为spaicalComp是在每帧最后才算groundY（节约性能，因为raycast很耗)
	/// 这个确保随时随地能取到正确高度
	/// </summary>
	/// <value>The ground y slow.</value>
	public float groundY_slow{
		get{
			return MapManager.instance.GetHeight(_worldPosi.x,_worldPosi.z);//13是groundLayer,这里是高频调用，直接用常量减少函数调用
		}
	}
	
	//TODO rotY回头整理好,避免与dir8有冲突
	//x,z设置为0
	float _rotY;
    /// <summary>
    /// 绕y轴的旋转角度
    /// </summary>
	public float rotY{
		set{
			_rotY=value;
			sprite.transform.eulerAngles=new Vector3(0,value,0);
			//-
			isDirty=true;
		}
		get{
			return _rotY;
		}
	}
	
    /// <summary>
    /// 与摄像机的距离
    /// </summary>
    public float cameraDistance {
		get{
			var c=Camera.main;
			if(c==null){
				return 100.0f;
			}else{
	       		return Vector3.Distance(c.transform.position, _worldPosi);
			}
		}
    }
    /// <summary>
    ///
    /// </summary>
    private Vector3 m_faceToDir= Vector3.zero;

    public Vector3 faceToDir
    {
        get { return m_faceToDir; }
    }


    //注意这个耗时0.02ms,多了就会消耗1ms
	//!!注意设置位置和朝向，在这里统一设置，这样才不会有抖动
	void OnUpdate(){
//		//因为如果是怪死了，执行物理效果的时候，要停掉SpacialComp的更新
//		if(enabled==false){
//			return;
//		}
		if(isDirty==false){//没有脏，就没必要,下面的运算会消耗0.02ms
			return;
		}

		//1:设置位置
		//贴地计算,主要是操作Y值
		if(isFlying==false){//如果是在落地，则要执行落地计算

			//if (!Logic.GetInstance ().bHideMode) {//11.27 kk- 解决最小化后从地底冒出的问题,直接关闭这个，tsprite取高度不会有太大的性能损耗
			float groundY=MapManager.instance.GetHeight(_worldPosi.x,_worldPosi.z);//13是groundLayer,这里是高频调用，直接用常量减少函数调用
			//}

			//0409 不再进行插值
//			//贴地的时候，y要进行插值
//			if(needLerpPosiY){//需要对Y进行插值
//				var currY=m_baseSprite.transform.position.y;//当前真实的y坐标
//				float deltaY = groundY -currY ;
//	           	deltaY=Mathf.Clamp(deltaY,-MAX_DELTA_Y,MAX_DELTA_Y);
//				currY+=deltaY;
//				
//				_worldPosi.y=currY;
//			}else{//不需要对Y进行插值
//				_worldPosi.y=groundY;
//			}

			_worldPosi.y=groundY;
		}else{//飞行中的Y,由外部直接设置
			
		}
		
		//设置位置
		if(useSmooth){
			sprite.transform.position =Vector3.SmoothDamp(sprite.transform.position,_worldPosi,ref smoothVelocity,smoothTime);
		}else{
			
			sprite.transform.position = _worldPosi;
		}
		
		//2:设置朝向,(这样飞行的时候就是平行于地面)
		sprite.transform.eulerAngles = new Vector3(0, _rotY, 0);
		
		//3:设置贴地(注意3和2的执行顺序，必须这样)
		if(is4Legs){
			NormalSelf();
		}

		//刷新完毕
		isDirty=false;
	}
	
	
    /// <summary>
    ///  补丁，如果刚加载就隐藏，会导致位置错误，因为spacial需要update来驱动位置
    ///  这个专门为sprite.bVisible,不带插值，直接设置坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
	public void ResetGridPosi(int x,int z){
		worldPosi=new Vector3(x+0.5f,0,z+0.5f);
		if(isFlying==false){//不是飞行中，才贴地呢
			_worldPosi.y= MapManager.instance.GetHeight(x,z);//13是groundLayer,这里是高频调用，直接用常量减少函数调用
			sprite.transform.position=_worldPosi;
		}
	}

	public void ResetPosi(float x,float z){
		worldPosi=new Vector3(x,0,z);
		if(isFlying==false){//不是飞行中，才贴地呢
			_worldPosi.y=MapManager.instance.GetHeight(x,z);//13是groundLayer,这里是高频调用，直接用常量减少函数调用
			sprite.transform.position=_worldPosi;
		}
	}

    public void Refresh()
    {
        isDirty = true;
    }

    /// <summary>
        //面向目标,不插值时会立刻生效，但是在Update里还要继续被设置
        //PathFollowerComp和FlyComp会执行这个,PathFollowComp每帧都执行这个
    /// </summary>
    /// <param name="aimPosi"></param>
    /// <param name="needLerp"></param>
	public void FaceTo(Vector3 aimPosi,bool needLerp=false)
    {
        Vector3 dir = aimPosi - worldPosi ;
		dir.y=0;
        dir.Normalize();
        m_faceToDir = dir;

        if (dir!=Vector3.zero){
			if(needLerp){//需要插值，则不会立刻设置值
				var quat = Quaternion.Slerp(sprite.transform.rotation, Quaternion.LookRotation(dir), realRotationSpeed * Time.deltaTime);
				_rotY=quat.eulerAngles.y;
			}else{//不需要插值的时候，直接设置到正确值(与现有贴地系统不会冲突，这个是个瞬时动作)
				var quat =Quaternion.LookRotation(dir);
				_rotY=quat.eulerAngles.y;
				sprite.transform.eulerAngles = new Vector3(0, _rotY, 0);
			}
		}

		//-
		isDirty=true;
    }

	/// <summary>
	/// 旋转朝向（朝向跑动的方向）
	/// </summary>
	public void FaceToDir(Vector3 dir, bool needLerp = false)
	{
		dir.Normalize();
	    m_faceToDir = dir;
        if (dir != Vector3.zero)
		{
			if (needLerp )
			{
				rotY = Quaternion.Slerp(sprite.transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime).eulerAngles.y;//5是转身速度
			}
			else
			{
				rotY = Quaternion.LookRotation(dir).eulerAngles.y;
			}

		}
	}

	//四腿动物贴地
	void NormalSelf()
	{	
		float front = 1.0f;
		float back = 1.0f;
		RaycastHit hitA;
	 
        Ray ray=new Ray();
		Vector3 fdir = sprite.transform.forward;
		fdir.y =0;
        ray.origin = sprite.transform.position+ fdir*front;
		//ray.origin.y=1000;//kk +
		ray.origin= new Vector3(ray.origin.x, 1000,ray.origin.z);//kk -
		
        ray.direction =-Vector3.up;
		LayerMask nl = 1 << EngineLayerEnum.Ground;
        if (Physics.Raycast(ray, out hitA,1500.0f,nl))
		{
			RaycastHit hitB;
			ray.origin = sprite.transform.position- fdir*back;
			//ray.origin.y=1000;
			ray.origin= new Vector3(ray.origin.x, 1000.0f,ray.origin.z);
			
		    if (Physics.Raycast(ray,out hitB,1500.0f,nl))
			{
				var p1=hitA.point;
				var p2=hitB.point;
				sprite.transform.forward = p1 - p2;
			}
		}
	}

	//在ZOX平面，左手坐标系，0，表示-x方向
	static float GetAngle(int p_dir8){
		switch(p_dir8){
			case 0:return 6*45.0f;
			case 1:return 7*45.0f;
			case 2:return 0*45.0f;
			case 3:return 1*45.0f;
			case 4:return 2*45.0f;
			case 5:return 3*45.0f;
			case 6:return 4*45.0f;
			case 7:return 5*45.0f;
			default:
				Debug.LogError("未知的方向");
				break;
		}
		return 0;
	}
	//
	static byte  GetDir8(float angle){
		int p_dir =(int)(angle / 45);
		p_dir =  8 -  p_dir;
		p_dir = p_dir % 8;
		p_dir += 2;
		p_dir = p_dir %8;
		return (byte)p_dir;
	}
}