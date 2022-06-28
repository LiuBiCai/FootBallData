using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootBallDataHelper
{
    public class FloatingWin
    {
        public int doubt { get; set; }       
        //队名
        public string home { get; set; }
        public string away { get; set; }

        public string half_score = "";
        public string score = "";
        //比赛事件
        public List<SoccerEvent> SoccerEvent { get; set; }
        //技术统计
        public TechnicalStatistic technicalStatistic { get; set; }
    }

    public class SoccerEvent
    {
        public Position position { get; set; }
        public int time { get; set; }
        public EventId eventId { get; set; }
        public string player1 = "";
        public string player2 = "";
        //public bool haveEvent = false;
    }

    public enum Position
    {
        middle,
        left,
        right,
    }
    public enum EventId
    {
        start=0,     //开始
        injureTime=1,//伤停补时
        middle=2, //中场休息
        over=3,   //比赛结束
        yellow=4, //黄牌
        red=5,    //红牌
        turn=6,   //换人
        soccer=7, //进球
        dianqiu=8,//点球
        dianqiuweijin=9,//点球未进
        wulongqiu=10,  //乌龙球
        other=11, //其他
    }

    public struct Point
    {
        public int left;
        public int right;
    }

    public class TechnicalStatistic
    {
        public Point shootRight { get; set; }      //射正
        public Point shotdeflection { get; set; }  //射偏
        public Point offensive { get; set; }       //进攻
        public Point dangerOffense { get; set; }   //危险进攻
        public Point possessionRate { get; set; }  //控球率        
        public Point corner { get; set; }          //角球
        public Point yellow { get; set; }          //黄牌
        public Point red { get; set; }             //红牌
        public Point penalty { get; set; }         //点球

        public int sum = 0;

    }
   
}
