using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootBallDataHelper
{
    public class FloatingWin
    {
        public bool doubt { get; set; }       
        //队名
        public string home { get; set; }
        public string away { get; set; }

        public string half_score { get; set; }
        public string score { get; set; }
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
        public string player1 { get; set; }
        public string player2 { get; set; }
        public bool haveEvent = false;
    }

    public enum Position
    {
        middle,
        left,
        right,
    }
    public enum EventId
    {
        middle, //中场休息
        over,   //比赛结束
        yellow, //黄牌
        red,    //红牌
        turn,   //换人
        soccer, //进球
        dianqiu,//点球
        dianqiuweijin,//点球未进
        other, //其他
    }

    struct Point
    {
        public int left;
        public int right;
    }

    public class TechnicalStatistic
    {
        Point shootRight { get; set; }      //射正
        Point shotdeflection { get; set; }  //射偏
        Point offensive { get; set; }       //进攻
        Point dangerOffense { get; set; }   //危险进攻
        Point possessionRate { get; set; }  //控球率        
        Point corner { get; set; }          //角球
        Point yellow { get; set; }          //黄牌
        Point red { get; set; }             //红牌
        Point penalty { get; set; }         //点球

    }
   
}
