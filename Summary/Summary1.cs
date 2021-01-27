using System;

namespace Summary
{

    public enum State
    {
        StateA = 0,
        StateB = 1
    }

    public struct StructBody
    {
        public int value;
    }

    /// <summary showdoc="true">
    /// 类描述
    /// </summary>
    /// <catalog>一级目录/二级目录/标题</catalog>
    public class Summary1
    {

        /// <summary>
        /// 方法描述
        /// </summary>
        /// <param name="arg1">整数</param>
        /// <param name="arg2">单精度浮点数</param>
        /// <param name="arg3">字符串</param>
        /// <param name="arg4">双精度浮点数</param>
        /// <param name="arg5">长整型</param>
        /// <returns>返回值</returns>
        public string Method1(int arg1, float arg2, string arg3, double arg4, long arg5)
        {
            return arg3;
        }

        /// <summary>
        /// 方法描述2
        /// </summary>
        /// <param name="state">枚举类型</param>
        /// <param name="action">引用类型</param>
        /// <param name="structBody">结构体</param>
        /// <returns>返回整型</returns>
        public int Method2(State state, Action action, StructBody structBody)
        {
            return 0;
        }

    }

}
