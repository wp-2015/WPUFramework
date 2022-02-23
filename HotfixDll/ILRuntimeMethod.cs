
namespace WPUFramework
{
	/// <summary>
	/// 调用热更新层方法基类
	/// </summary>
	public abstract class ILRuntimeMethod
	{
		public abstract void Invoke();
		public abstract void Invoke(object a);
		public abstract void Invoke(object a, object b);
		public abstract void Invoke(object a, object b, object c);
	}
}