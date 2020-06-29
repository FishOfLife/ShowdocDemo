using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShowdocCode
{
    interface IAction
    {

        /**
        * showdoc
        * @catalog 人类行为/基本行为
        * @title 抓握
        * @description 描述内容
        * @param force 必选 float 抓握力度 
        * @param time 必选 float 时间
        * @return  无
        * @return_param name string 抓取对象
        * @remark 这里是备注信息
        */
        string Grasp(float force, float time);

        /**
        * showdoc
        * @catalog 人类行为/基本行为
        * @title 哭
        * @description 描述内容
        * @param decibel 必选 float 哭声大小
        * @return  无
        * @remark 这里是备注信息
        */
        void Cry(float decibel);

        /**
        * showdoc
        * @catalog 人类行为/一般行为
        * @title 奔跑
        * @description 描述内容
        * @param time 必选 float 时间 
        * @param speed 必选 float 速度  
        * @return  无
        * @return_param distance float 奔跑距离
        * @remark 这里是备注信息
        */
        float Run(float time, float speed);

        /**
        * showdoc
        * @catalog 人类行为/一般行为
        * @title 跳跃
        * @description 描述内容
        * @url 
        * @param height 必选 float 跳跃高度 
        * @remark 这里是备注信息
        */
        void Jump(float height);

    }

}
