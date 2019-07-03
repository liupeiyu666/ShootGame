using UnityEngine;
using System.Collections;

public class VectorUtils 
{
	//世界坐标转换成网格坐标
	public static Vector3 ConvertToGridPosi( Vector3 tpos)
	{
		tpos.x = (int)tpos.x;
		tpos.z = (int)tpos.z;
		tpos += new Vector3(0.5f, 0, 0.5f);
		
		return tpos;
	}

	///<summary>
	/// 注意这个是在xoz平面内的
	/// 计算当前位置是否在目标半径内
	/// curpox, curpoy 当前x,y ,  despox, despoy目标x,y  radius 半径
	/// </summary>
	//TODO 这个方法可以通过比较平方而不是求平方根优化
	public static bool IsInRadius( int curpox, int curpoy, int despox, int despoy, int radius )
	{
		Vector3 curvp = new Vector3( curpox, 0, curpoy );
		Vector3 desvp = new Vector3( despox, 0, despoy );
		float distance = Vector3.Distance( curvp, desvp );
		if( radius >= distance )
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 设置xyz
	/// </summary>
	/// <param name="trm">transform</param>
	/// <param name="x">x坐标</param>
	/// <param name="y">y坐标</param>
	/// <param name="z">z坐标</param>
	public static void SetLocalPosi(Transform trm,float x,float y,float z){
		var p=Vector3.zero;
		p.x=x;
		p.y=y;
		p.z=z;
		trm.localPosition=p;
	}
	
	/// <summary>
	/// 设置Y
	/// </summary>
	/// <param name="trm">transform</param>
	/// <param name="y">y坐标</param>
	public static void SetLocalPosiY(Transform trm,float y){
		var p=trm.localPosition;
		p.y=y;
		trm.localPosition=p;
	}
	
	/// <summary>
	/// 设置位置
	/// </summary>
	/// <param name="trm">transform</param>
	/// <param name="x">x坐标</param>
	/// <param name="y">y坐标</param>
	/// <param name="z">z坐标</param>
	public static void SetPosi(Transform trm,float x,float y,float z){
		var p=Vector3.zero;
		p.x=x;
		p.y=y;
		p.z=z;
		trm.position=p;
	}

	#region 技能计算公式
	/// <summary>
	/// 获取选中角度
	/// </summary>
	/// <param name="curPosi">当前点</param>
	/// <param name="aimPosi">目标点</param>
	public static float GetRotAngle(Vector3 curPosi, Vector3 aimPosi)
	{
		Vector3 dir = aimPosi - curPosi;
		dir.y=0;
		dir.Normalize();
		
		float fRotY = 0f;
		if(dir != Vector3.zero){
			Quaternion quat = Quaternion.LookRotation(dir);
			fRotY = quat.eulerAngles.y;
		}
		
		return fRotY;
	}
	//	/**
	//	 * 求出弧度内平均点.
	//	 * @param currPosition 圆心
	//	 * @param targetPosition 目标点
	//	 * @param angle 每2点间的角度
	//	 * @param lineCount 线
	//	 * @param r	半径
	//	 * @return
	//	 */
	//	public static List<Point2> getPoints(Point2 currPosition, Point2 targetPosition, int angle, int lineCount, int r) {
	//		List<Point2> ps = new List<Point2>();
	//		int angleCount = angle * (lineCount - 1);
	//		int angle2 = angleCount / 2;
	//		int angleTarget = getAngle(currPosition, targetPosition);
	//
	//		int startAngle = (angleTarget - angle2) % 360;;	//Math.Abs(angleTarget - angle2) % 360;
	//		Point2 startPoint = getPointByCircumference(currPosition, r, startAngle);
	//		ps.Add(startPoint);
	//		for (int i = 1; i < lineCount; i++) {
	//			int tempAngle = (startAngle + angle * i) % 360;	//Math.Abs(startAngle + angle * i) % 360;
	//			Point2 tempPoint = getPointByCircumference(currPosition, r, tempAngle);
	//			ps.Add(tempPoint);
	//		}
	//		return ps;
	//	}
	
//	/// <summary>
//	/// 根据圆心.半径.角度 求圆周上的点
//	/// </summary>
//	/// <param name="o">圆心</param>
//	/// <param name="r">半径</param>
//	/// <param name="angle">角度</param>
//	/// <returns></returns>
//	public static Point2 GetPointByCircumference(Point2 o, int r, int angle) {
//		double a = 2d * angle * Math.PI / 360d;
//		return new Point2((short) Math.Round(r * Math.Cos(a) + o.x), (short) (Math.Round(r * Math.Sin(a) + o.y)));
//	}
	
//	// TODO LY 搞清楚为何这么用的
//	/// <summary>
//	/// 2点求夹角度
//	/// </summary>
//	/// <param name="o">圆心</param>
//	/// <param name="targetPosition">圆周上一点.</param>
//	/// <returns></returns>
//	public static int getAngle(Point2 fromP, Point2 toP) {
//		int x = Math.Abs(toP.x - fromP.x);
//		int y = Math.Abs(toP.y - fromP.y);
//		double z = Math.Sqrt(x * x + y * y);
//		int angle1 = (int)Math.Round((float) (Math.Asin(y / z) / Math.PI * 180));
//		
//		x = toP.x - fromP.x;
//		y = toP.y - fromP.y;
//		byte xx = (byte) ((x > 0) ? ((y > 0) ? 1 : ((y < 0) ? 4 : 1)) : ((x < 0) ? ((y > 0) ? 2 : ((y < 0) ? 3 : 3)) : ((y > 0) ? 2 : ((y < 0) ? 4 : 0))));
//		if (xx == 2) {
//			angle1 = 90 + (90 - angle1);
//		} else if (xx == 3) {
//			angle1 = 180 + angle1;
//		} else if (xx == 4) {
//			angle1 = 270 + (90 - angle1);
//		}
//		return angle1;
//	}
	#endregion

	/// <summary>
	/// 取得在平面的距离，因为游戏是无地形的，所有要使用无地形的方式取得地图
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public static float GetXOZDistance(Vector3 a,Vector3 b)
	{
		a.y=0;
		b.y=0;
		return Vector3.Distance(a,b);
	}
	
	
	/// <summary>
	/// 判断两个vector3的各个元素经过int强转后是否相等(只比较 x z值)
	/// </summary>
	/// <returns>如果相等返回true</returns>
	public static bool IsGridPositionEqual(Vector3 a,Vector3 b)
	{
		return (int)a.x==(int)b.x && (int)a.z==(int)b.z;
	}
	
	/// <summary>
	/// node转化为vector2
	/// </summary>
	/// <returns>转化后的vector2</returns>
	public static Vector2 NodeToVector2(int x,int z){
		Vector2 v;
		v.x=x;
		v.y=z;
		return v;
		
	}
	
//	/// <summary>
//	/// node转化为vector3
//	/// </summary>
//	/// <returns>转化后的vector3</returns>
//	public static Vector3 NodeToVector3(int x,int z){
//		Vector3 v;
//		v.x=x/10+0.5f;
//		v.y=0;
//		v.z=z/10+0.5f;
//		return v;
//		
//	}
	
	/// <summary>
	/// vector3转化为vector2
	/// </summary>
	/// <returns>转化后的vector2</returns>
	public static Vector2 Vector3ToVector2(Vector3 v3){
		Vector2 v2;
		v2.x=v3.x;
		v2.y=v3.z;
		return v2;
	}

}
