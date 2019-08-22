using System; 
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Script pool v1.
/// @author Wade		
/// 把stack修改为queue，为了测试createPlayer的时候，new cls的消耗
/// 
/// 这里没有设置上线 - 如果未来需要，就就修改成单独的类
/// </summary>
public class ScriptPoolV1
{
	Dictionary<string, Stack<ScriptPoolItem>> ScriptAllDict = new Dictionary<string, Stack<ScriptPoolItem>>();

    public ScriptPoolV1(){}

	/// <summary>
	/// 默认添加，list<item cls>, num数量
	/// </summary>
	/// <param name="num">创建个数</param>
	/// <typeparam name="T">类(继承于ScriptPoolItem)</typeparam>
	public void Add<T>(int num) where T:ScriptPoolItem, new(){
		for(int j=0; j<num; j++){
			T cls = new T();
			Recycle( typeof(T).ToString(), cls);
		}
	}

	/// <summary>
	/// 回收-类
	/// </summary>
	/// <param name="name">类名(应该用type，而不是传入...因为项目原因暂时采用传入的方式).</param>
	/// <param name="cls">Cls.</param>
	public void Recycle(string name, ScriptPoolItem cls){
		if(name == ""){
			return ;
		}
		Stack<ScriptPoolItem> tempStack;
		cls.OnBeforePushToPool();			//进入的时候
		if(ScriptAllDict.TryGetValue(name, out tempStack)){
			tempStack.Push( cls );		//Enqueue
		}else{
			tempStack = new Stack<ScriptPoolItem>();
			tempStack.Push( cls );
			ScriptAllDict.Add(name, tempStack);
		}
	}
	
	/// <summary>
	/// 从池中获取
	/// </summary>
	/// <param name="name">类名</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	// TODO nullOrEmpty的时候，直接报错算了
	public T Get<T>(string name) where T:ScriptPoolItem, new(){
		T result;
		if(name == string.Empty){
			result = new T();
			result.OnAfterPopFromPool();		//拿出的时候
			return result;
		}
		Stack<ScriptPoolItem> tempStack;
		if(ScriptAllDict.TryGetValue(name, out tempStack)){
			result = (tempStack.Count == 0) ? new T() : tempStack.Pop() as T;		//Dequeue
		}else{
			result = new T();
		}
		result.scriptName = name;
		result.OnAfterPopFromPool();			//拿出的时候
		return result;
	}

	/// <summary>
	/// Clears all.
	/// </summary>
	public void ClearAll(){
		ScriptAllDict.Clear();
	}

	/// <summary>
	/// 清理-指定的类
	/// </summary>
	/// <param name="name">Name.</param>
	public void Clear(string name){
		Stack<ScriptPoolItem> tempStack;
		if(ScriptAllDict.TryGetValue(name, out tempStack)){
			tempStack.Clear();
		}
	}
    /// <summary>
    /// 获取 制定类 存了多少个
    /// </summary>
    public int GetCount(string name)
    {
        Stack<ScriptPoolItem> tempStack;
        if (ScriptAllDict.TryGetValue(name, out tempStack))
        {
            return tempStack.Count;
        }
        return 0;
    }
	/// <summary>
	/// 打印当前脚本池信息
	/// </summary>
	public void Print(){

	} 
}

