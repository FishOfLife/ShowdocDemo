using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Summary
{
    /// <summary showdoc="true">
    /// 
    /// </summary>
    /// <catalog>一级目录/二级目录/空注释</catalog>
    public class SummaryUseCase7
    {

        /// <summary>
        /// 属性1
        /// </summary>
        public string Property
        {
            get; set;
        }

        /// <summary>
        /// 返回值为空
        /// </summary>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        /// <returns></returns>
        public int Method(int arg1, string arg2)
        {
            return 0;
        }

        /// <summary>
        /// 参数为空
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2">参数2</param>
        /// <param name="arg3"></param>
        /// <returns>返回值</returns>
        public int Method(int arg1, string arg2, float arg3)
        {
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg1">描述为空</param>
        /// <param name="arg2">描述为空</param>
        /// <param name="arg3">描述为空</param>
        /// <param name="arg4">描述为空</param>
        /// <returns>描述为空</returns>
        public int Method(int arg1, string arg2, float arg3, long arg4)
        {
            return 0;
        }

    }

}
