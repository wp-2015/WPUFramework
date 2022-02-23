using System;
using System.Reflection;

namespace WPUFramework
{
    /// <summary>
    /// Mono静态方法
    /// </summary>
    public class MonoStaticMethod : ILRuntimeMethod
    {
        private readonly MethodInfo methodInfo;

        private readonly object[] param;

        public MonoStaticMethod(Type type, string methodName)
        {
            this.methodInfo = type.GetMethod(methodName);
            this.param = new object[this.methodInfo.GetParameters().Length];
        }

        public override void Invoke()
        {
            this.methodInfo.Invoke(null, param);
        }

        public override void Invoke(object a)
        {
            this.param[0] = a;
            this.methodInfo.Invoke(null, param);
        }

        public override void Invoke(object a, object b)
        {
            this.param[0] = a;
            this.param[1] = b;
            this.methodInfo.Invoke(null, param);
        }

        public override void Invoke(object a, object b, object c)
        {
            this.param[0] = a;
            this.param[1] = b;
            this.param[2] = c;
            this.methodInfo.Invoke(null, param);
        }
    }
}