using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Summary
{
    /// <summary showdoc="true">
    /// 注释文档用例8,脚踏八条船之类对应多文档
    /// </summary>
    public class SummaryUseCase8
    {

        /// <summary>
        /// 第一条船
        /// </summary>
        /// <catalog>一级目录/二级目录/第一条船</catalog>
        public char Property1
        {
            get; set;
        }

        /// <summary>
        /// 第二条船
        /// </summary>
        /// <catalog>一级目录/二级目录/第二条船</catalog>
        public byte Property2
        {
            get;
        }

        /// <summary>
        /// 第三条船
        /// </summary>
        /// <catalog>一级目录/二级目录/第三条船</catalog>
        public short Property3
        {
            get;
        }

        /// <summary>
        /// 第四条船
        /// </summary>
        /// <catalog>一级目录/二级目录/第四条船</catalog>
        public int field1;

        /// <summary>
        /// 第五条船
        /// </summary>
        /// <catalog>一级目录/二级目录/第五条船</catalog>
        public float field2;

        /// <summary>
        /// 第六条船
        /// </summary>
        /// <returns>返回</returns>
        /// <catalog>一级目录/二级目录/第六条船</catalog>
        public double Method1()
        {
            return 0;
        }

        /// <summary>
        /// 第七条船
        /// </summary>
        /// <returns>返回</returns>
        /// <catalog>一级目录/二级目录/第七条船</catalog>
        public long Method2()
        {
            return 0;
        }

        /// <summary>
        /// 第八条船
        /// </summary>
        /// <returns>返回</returns>
        /// <catalog>一级目录/二级目录/第八条船</catalog>
        public string Method3()
        {
            return null;
        }

    }

}
