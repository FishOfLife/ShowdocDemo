using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Summary
{
    /// <summary showdoc="true">
    /// 注释文档用例3,方法测试
    /// </summary>
    /// <catalog>一级目录/二级目录/方法测试</catalog>
    public class SummaryUseCase3
    {

        /// <summary>
        /// 无参无返
        /// </summary>
        public void Init()
        {

        }

        /// <summary>
        /// 单参无返
        /// </summary>
        /// <param name="data">参数1</param>
        public void Init(object data)
        {

        }

        /// <summary>
        /// 多参无返
        /// </summary>
        /// <param name="data">参数1</param>
        /// <param name="config">参数2</param>
        /// <param name="time">参数3</param>
        public void Init(object data, object config, long time)
        {

        }

        /// <summary>
        /// 多参有返
        /// </summary>
        /// <param name="data">参数1</param>
        /// <param name="config">参数2</param>
        /// <param name="time">参数3</param>
        /// <param name="path">参数4</param>
        /// <returns></returns>
        public bool Init(object data, object config, long time, string path)
        {
            return true;
        }

    }

}
