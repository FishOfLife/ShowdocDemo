using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Summary
{

    /// <summary showdoc="true">
    /// 注释文档用例5,泛型+重载测试
    /// </summary>
    /// <catalog>一级目录/二级目录/泛型和重载测试</catalog>
    public class SummaryUseCase5<T1, T2, T3>
    {

        /// <summary>
        /// 类泛型T1类型的属性
        /// </summary>
        public T1 Property1
        {
            get; set;
        }

        /// <summary>
        /// 字典
        /// </summary>
        public Dictionary<T2, T3> dictionary;

        /// <summary>
        /// 包含类泛型和方法泛型的方法
        /// </summary>
        /// <typeparam name="T4">方法泛型1</typeparam>
        /// <typeparam name="T5">方法泛型2</typeparam>
        /// <param name="t1">参数1</param>
        /// <param name="t2">参数2</param>
        /// <param name="t3">参数3</param>
        /// <param name="t4">参数4</param>
        /// <param name="t5">参数5</param>
        /// <returns>返回类泛型T1的实例</returns>
        public T1 Method1<T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return default(T1);
        }

        /// <summary>
        /// 究极无敌恶心之嵌套类型+泛型+方法重载
        /// </summary>
        /// <typeparam name="T4">方法泛型</typeparam>
        /// <param name="t3s">类泛型T3数组</param>
        /// <param name="dic">字典</param>
        /// <returns></returns>
        public Dictionary<T1, T2> Method1<T4>(T3[] t3s, Dictionary<Func<T1, T2>, Action<T3, T4>> dic)
        {
            return null;
        }

    }

}
