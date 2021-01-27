using System;
using System.Collections.Generic;

namespace Summary
{
    /// <summary showdoc="true">
    /// 泛型类
    /// </summary>
    /// <typeparam name="T1">泛型描述</typeparam>
    /// <catalog>一级目录/二级目录/泛型类和方法</catalog>
    public class Summary5<T1> where T1 : class
    {

        /// <summary>
        /// 泛型字段1
        /// </summary>
        public T1 field1;

        /// <summary>
        /// 泛型属性
        /// </summary>
        public T1 Property1
        {
            get; set;
        }

        /// <summary>
        /// 泛型方法
        /// </summary>
        /// <typeparam name="T2">泛型描述</typeparam>
        /// <param name="func">含参返回委托</param>
        /// <returns>返回值</returns>
        public int Method1<T2>(Func<string[], Dictionary<T2, float>> func)
        {
            return 0;
        }

        /// <summary>
        /// 泛型方法1
        /// </summary>
        /// <typeparam name="T3">第一个泛型</typeparam>
        /// <typeparam name="T4">第二个泛型</typeparam>
        /// <param name="t1">泛型参数1</param>
        /// <param name="t2">泛型参数2</param>
        /// <returns></returns>
        public string Method2<T3, T4>(T1 t1, T3 t2) where T3 : struct where T4 : class
        {
            return string.Empty;
        }
    }

}
