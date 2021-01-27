using System;
using System.Collections.Generic;

namespace Summary
{
    /// <summary showdoc="true">
    /// 类描述1
    /// </summary>
    /// <catalog>一级目录/二级目录/特殊参数方法</catalog>
    public class Summary2
    {

        /// <summary>
        /// 复杂参数方法
        /// </summary>
        /// <param name="func">返回值的回调</param>
        /// <param name="dictionary">字典</param>
        public void Method1(Func<string, int> func, Dictionary<int, StructBody> dictionary)
        {

        }

        /// <summary>
        /// 无参方法
        /// </summary>
        /// <returns>返回整型</returns>
        public int Method2()
        {
            return 0;
        }

    }

}
