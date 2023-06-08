using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Summary
{
    /// <summary showdoc="true">
    /// 注释文档用例4,复杂类型测试
    /// </summary>
    /// <catalog>一级目录/二级目录/复杂类型测试</catalog>
    public class SummaryUseCase4
    {

        /// <summary>
        /// 字典类型
        /// </summary>
        public Dictionary<string, int> Dictionary1
        {
            get; set;
        }

        /// <summary>
        /// 列表类型
        /// </summary>
        public List<float> List1
        {
            get;
        }

        /// <summary>
        /// 数组类型
        /// </summary>
        public double[] arrys;

        /// <summary>
        /// 字典类型,委托类型
        /// </summary>
        /// <param name="arg1">字典参数</param>
        /// <param name="arg2">委托参数</param>
        public void Method1(Dictionary<float, int> arg1, Func<string, int> arg2)
        {

        }

        /// <summary>
        /// 不重复哈希表类型,委托类型
        /// </summary>
        /// <param name="arg1">无参委托</param>
        /// <param name="arg2">单参委托</param>
        /// <returns></returns>
        public HashSet<Queue<int[]>> Method2(Action arg1, Action<int> arg2)
        {
            return null;
        }

        /// <summary>
        /// 不重复哈希表类型,委托类型
        /// </summary>
        /// <param name="arg1">无参委托</param>
        /// <param name="arg2">单参委托</param>
        /// <returns></returns>
        public HashSet<Queue<int[]>>[] Method3(Action arg1, Action<int> arg2)
        {
            return null;
        }

        /// <summary>
        /// 究极恶心多重嵌套类型
        /// </summary>
        /// <param name="arg">键值对</param>
        /// <returns>字典</returns>
        public Dictionary<List<HashSet<LinkedList<string>>>, int[]> Method4(KeyValuePair<int, Func<float, long>> arg)
        {
            return null;
        }

    }

}
