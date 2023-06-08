using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Summary
{
    /// <summary showdoc="true">
    /// 注释文档用例1,属性测试
    /// </summary>
    /// <catalog>一级目录/二级目录/属性测试</catalog>
    public class SummaryUseCase1
    {

        /// <summary>
        /// 可读写属性
        /// </summary>
        public string Value1
        {
            get; set;
        }

        /// <summary>
        /// 可读属性
        /// </summary>
        public int Value2
        {
            get;
        }

        /// <summary>
        /// 对外可读对内可写属性
        /// </summary>
        public float Value3
        {
            get;
            private set;
        }

    }

}
