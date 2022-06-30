using FootBallDataHelper.DBHlper;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FootBallDataHelper
{
    public class FBDHelper
    {
        IWebDriver spider = new ChromeDriver();

        //打开指定日期的完成比赛
        //https://live.leisu.com/wanchang-20220622
        string wanchengUrl = "https://live.leisu.com/wanchang-";
        public bool OpenTimeFinishPage(string time)
        {           
            spider.Navigate().GoToUrl(wanchengUrl+time);
            return true;
        }

        //将鼠标悬浮在指定行的比分处
        public bool HoverRows(int row)
        {
            //通过F12 分析发现 class="qcbf" 可以唯一标定比分
            var rows = spider.FindElements(By.ClassName("qcbf"));
            if(row>=rows.Count())
            {
                return false;
            }
            Actions action = new Actions(spider);
            action.MoveToElement(rows[row]);
            action.Perform();
            return true;
        }

        #region 浮窗
        //判断指定行的浮窗是否开启
        public bool isOpenFloatingWindowRow(int row)
        {
            //通过F12分析lier-score可唯一标识浮窗结构
            var rows = spider.FindElements(By.ClassName("lier-score"));
            if (row >= rows.Count())
            {
                return false;
            }
            //悬浮窗口是否打开 可根据class=tooltip-analysis判断
            if (IsElementPresentWithTime(By.ClassName("tooltip-analysis"), rows[row]))
            {
                return true;
            }
            return false;
        }
        #region 获取联赛名 ID
        public Tuple<bool,string,int> GeteventName(int row)
        {
            var events = spider.FindElements(By.ClassName("event-name"));
            var name=  events[row].Text;
            if(string.IsNullOrEmpty(name))
            {
                return Tuple.Create(false, "name is null", 0);
            }
            var url = events[row].GetAttribute("href");
            if(string.IsNullOrEmpty(url))
            {
                return Tuple.Create(true, name, 0);
            }
            var id = url.Split('-')[1] ;//comp-24" target
            if(string.IsNullOrEmpty(id))
            {
                return Tuple.Create(false, "id is null", 0);
            }
            return new Tuple<bool, string, int>(true,name,int.Parse(id));
        }


        #endregion
        #region 浮窗数据解析
        public Tuple<bool,string,FloatingWin> GetFloatingData()
        {
            var data = new FloatingWin();

            var window = spider.FindElement(By.ClassName("tooltip-analysis"));
            //获取队名
            var names = window.FindElements(By.ClassName("name"));
            if(names.Count>1)
            {
                data.home = names[0].Text;
                data.away = names[1].Text;
            }
            var soccerEvent = GetSoccerEvents(window);
            if(!soccerEvent.Item1)
            {
                return new Tuple<bool, string, FloatingWin>(false,soccerEvent.Item2,data);
            }
            data.SoccerEvent = soccerEvent.Item3;            
            ComputeScore(data, soccerEvent.Item3);
            bool havePlayerName = false;
            foreach(var item in soccerEvent.Item3)
            {
                if(item.eventId==EventId.other)
                {
                    data.doubt = 10;
                }

                if(!string.IsNullOrEmpty(item.player1))
                {
                    havePlayerName = true;
                    break;
                }
            }
            if(!havePlayerName)
            {
                data.doubt++;
            }
            var TechStatistic= GetTechStatistic(window);
            if(!TechStatistic.Item1)
            {
                data.doubt++;
                return new Tuple<bool, string, FloatingWin>(false, soccerEvent.Item2, data);
            }
            if(TechStatistic.Item3.sum==0)
            {
                data.doubt++;
            }            
            data.technicalStatistic = TechStatistic.Item3;
              
            return new Tuple<bool,string ,FloatingWin>(false, "end",data);
        }

        //计算比分 
        public bool ComputeScore(FloatingWin data,List<SoccerEvent>soccerEvents)
        {
            int left = 0;
            int right = 0;
            soccerEvents.Reverse();
            foreach(var e in soccerEvents)
            {
                if(e.position==Position.middle&&e.eventId==EventId.middle)
                {
                    data.half_score = left + ":" + right;
                }
                if(e.time>47&& data.half_score == "")
                {
                    data.half_score = left + ":" + right;
                }

                if(e.eventId==EventId.soccer||e.eventId==EventId.dianqiu||e.eventId==EventId.wulongqiu)
                {
                    if(e.position==Position.left)
                    {
                        left++;
                    }
                    else if(e.position==Position.right)
                    {
                        right++;
                    }
                    else
                    {
                        MessageBox.Show("ComputeScore");

                        return false;
                    }

                }
            }
            if(data.half_score=="")
            {
                data.half_score = left + ":" + right;
            }
            data.score= left + ":" + right;
            return true;
        }

        public Tuple<bool,string,List<SoccerEvent>> GetSoccerEvents(IWebElement window)
        {
            var events = new List<SoccerEvent>();
            
            var eventList= window.FindElement(By.ClassName("eventlist"));
            var rows= eventList.FindElements(By.ClassName("row"));
            foreach (var row in rows)
            {
                var soccerEvent = new SoccerEvent();

                //比赛状态行
                if (IsElementPresent(By.ClassName("cte"),row))
                {
                    soccerEvent.position = Position.middle;
                    var match = row.FindElement(By.ClassName("cte")).Text;
                    Console.WriteLine("cte:" +match);
                    if(match.Contains("比赛结束"))
                    {
                        soccerEvent.eventId = EventId.over;
                    }
                    else if(match.Contains("中场休息"))
                    {
                        soccerEvent.eventId = EventId.middle;
                    }
                    else if (match.Contains("伤停补时"))
                    {
                        soccerEvent.eventId = EventId.injureTime;
                    }
                    else if (match.Contains("比赛开始"))
                    {
                        soccerEvent.eventId = EventId.start;
                    }        
                    else 
                    {                        
                        soccerEvent.eventId = EventId.other;
                        events.Add(soccerEvent);
                    }
                    events.Add(soccerEvent);
                    continue;
                }
                //比赛事件行
                var left = row.FindElement(By.ClassName("left"));
                var right = row.FindElement(By.ClassName("right"));
                var center=row.FindElement(By.ClassName("center")).Text;
                var leftRight = left;
                Console.WriteLine("center " + center);
                int time = 0;
                if(!int.TryParse(center.Replace("\'",""),out time))
                {                    
                    return new Tuple<bool, string, List<SoccerEvent>>(false, "time error", events);
                }
                soccerEvent.time=time;
                if(IsElementPresent(By.ClassName("svg-icon"),left))
                {
                    soccerEvent.position = Position.left;
                    leftRight = left;
                }
                else if (IsElementPresent(By.ClassName("svg-icon"), right))
                {
                    soccerEvent.position = Position.right;
                    leftRight = right;
                }
                else
                {
                    Console.WriteLine("svg-icon");
                    return new Tuple<bool, string, List<SoccerEvent>>(false, "svg-icon", events);
                }
                IWebElement use;               
                if(!TryGetElement(By.TagName("use"), leftRight, out use))
                {
                    return new Tuple<bool, string, List<SoccerEvent>>(false, "get use", events);
                }
                var iconName = use.GetAttribute("xlink:href");
                Console.WriteLine(iconName);
                if(iconName.Contains("huangpai"))
                {
                    soccerEvent.eventId = EventId.yellow;
                }
                else if (iconName.Contains("hongpai"))
                {
                    soccerEvent.eventId = EventId.red;
                }
                else if (iconName.Contains("huanren"))
                {
                    soccerEvent.eventId = EventId.turn;
                }
                else if (iconName.Contains("Soccer"))
                {
                    soccerEvent.eventId = EventId.soccer;
                }
                else if (iconName.Contains("dianqiuweijin"))
                {
                    soccerEvent.eventId = EventId.dianqiuweijin;
                }
                else if (iconName.Contains("dianqiu"))
                {
                    soccerEvent.eventId = EventId.dianqiu;
                }                
                else if(iconName.Contains("wulongqiu"))
                {
                    soccerEvent.eventId = EventId.wulongqiu;
                }
                else if (iconName.Contains("lianghuangyihongbeifen"))
                {
                    soccerEvent.eventId = EventId.lianghuangyihongbeifen;
                }
                else if (iconName.Contains("VAR"))
                {
                    soccerEvent.eventId = EventId.var;
                }
                else
                {
                    MessageBox.Show(iconName);
                    return new Tuple<bool, string, List<SoccerEvent>>(false, "known "+ iconName, events);
                }
               
                if(soccerEvent.eventId== EventId.turn)
                {
                    var turns = leftRight.FindElements(By.ClassName("turn"));
                    soccerEvent.player1 = turns[0].Text;
                    soccerEvent.player2 = turns[1].Text;
                        
                }
                else
                {
                    var span = leftRight.FindElement(By.TagName("span"));
                    soccerEvent.player1 = span.Text;
                }
                events.Add(soccerEvent);
            }

            return new Tuple<bool, string, List<SoccerEvent>>(true,"end", events);
        }

        public Tuple<bool,string,TechnicalStatistic> GetTechStatistic(IWebElement window)
        {
            var techStatic = new TechnicalStatistic();            
            var rows = window.FindElements(By.ClassName("row"));
            for(int i = 1; i < 10; i++)
            {
                var row = rows[rows.Count - i];
                if(!IsElementPresent(By.ClassName("center"),row))
                {
                    return new Tuple<bool, string, TechnicalStatistic>(false, "no tech", techStatic);
                }

                var name = row.FindElement(By.ClassName("center")).Text;

                Point data= GetTechRowData(row);
                if (name=="射正")
                {
                    techStatic.shootRight = data;
                }
                else if(name=="射偏")
                {
                    techStatic.shotdeflection = data;
                }
                else if(name=="进攻")
                {
                    techStatic.offensive = data;
                }
                else if(name=="危险进攻")
                {
                    techStatic.dangerOffense = data;
                }
                else if(name=="控球率")
                {
                    techStatic.possessionRate = data;
                }
                else if(name=="角球")
                {
                    techStatic.corner=data;
                }
                else if(name== "黄牌")
                {
                    techStatic.yellow = data;
                }
                else if (name == "红牌")
                {
                    techStatic.red = data;
                }
                else if (name == "点球")
                {
                    techStatic.penalty = data;
                }
                techStatic.sum += data.left + data.right;
            }
            
            return new Tuple<bool, string, TechnicalStatistic>(true, "end", techStatic);
        }

        public Point GetTechRowData(IWebElement row)
        {
            Point point = new Point();
            var nums=row.FindElements(By.ClassName("num"));
            point.left = int.Parse(nums[0].Text);
            point.right = int.Parse(nums[1].Text);
            return point;

        }

        public int GetDetailID(int row)
        {
            //通过F12分析lier-score可唯一标识浮窗结构
            var rows = spider.FindElements(By.ClassName("lier-score"));
            if (row >= rows.Count())
            {
                return 0;
            }
            var id=rows[row].GetAttribute("id");
            string fid = id.Replace("score", "");
            return int.Parse(fid);
        }

        #endregion



        #endregion

        #region 数据库 操作

        public Tuple<bool,string> StoreData(string date,string events,int eventId,int detailId ,FloatingWin floating)
        {
            try
            {
                Model.init_data data = new Model.init_data();
                data.date = date;
                data.detail_id = detailId;
                data.events = events;
                data.event_id=eventId;
                data.home = floating.home;
                data.away = floating.away;
                data.half_score = floating.half_score;
                data.score = floating.score;
                data.doubt = floating.doubt;
                data.source_data = JsonConvert.SerializeObject(floating.SoccerEvent);
                data.techical_data = JsonConvert.SerializeObject(floating.technicalStatistic);
                var soccer = BllSoccer.Select(detailId);
                if(soccer==null)
                {
                    BllSoccer.Insert(data);
                }
                else
                {
                    data.id = soccer.id;
                    BllSoccer.Update(data);
                }
                return new Tuple<bool, string>(true, "success");
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new Tuple<bool, string>(false, "failed");
            }
            
        }

        public Tuple<bool,string> HaveData(int detailId)
        {
            if(detailId==0)
            {
                return new Tuple<bool, string>(false, "detaiId is 0");
            }
            var soccer = BllSoccer.Select(detailId);
            if (soccer != null)
            {
                return new Tuple<bool, string>(true, "success");
            }
            return new Tuple<bool, string>(false, "no data");
        }

        public Tuple<bool,string> StoreData()
        {
            string date = "20220626";
            DateTime dateTime = DateTime.ParseExact("20220627", "yyyyMMdd",System.Globalization.CultureInfo.InvariantCulture);

            do
            {
                dateTime = dateTime.AddDays(-1);
                
                date = dateTime.ToString("yyyyMMdd");
                
                OpenTimeFinishPage(date);
                Thread.Sleep(3000);
                
                try
                {
                    var rows = spider.FindElements(By.ClassName("qcbf"));
                    for (int row = 0; row < rows.Count; row++)
                    {
                        var detailId = GetDetailID(row);
                        var soccer = BllSoccer.Select(detailId);
                        if (soccer != null)
                        {
                            continue;
                        }

                        var eventname = GeteventName(row).Item2;
                        var eventid = GeteventName(row).Item3;
                        int hoverCount = 0;
                        do
                        {
                            HoverRows(row);
                            Thread.Sleep(500);
                            if (hoverCount++ > 3)
                            {
                                FloatingWin floatingWin = new FloatingWin();
                                floatingWin.doubt = 10;
                                StoreData(date, eventname, eventid, detailId, floatingWin);
                                break;
                            }
                        }
                        while (!isOpenFloatingWindowRow(row));
                        if (hoverCount > 3)
                        {
                            continue;
                        }
                        var floating = GetFloatingData();
                        StoreData(date, eventname, eventid, detailId, floating.Item3);
                    }
                }
                catch
                {
                    dateTime = dateTime.AddDays(1);
                    continue;
                }
            }
            while (true);
          
            return new Tuple<bool, string>(true, "success");
        }


        #endregion

        #region tool
        private bool IsElementPresentWithTime(By by,int timeLimit=2)
        {
            DateTime start = DateTime.Now;
            TimeSpan span = DateTime.Now - start;
            while (span.TotalSeconds < timeLimit)
            {
                try
                {
                    var result = spider.FindElement(by);
                    return true;
                }
                catch (NoSuchElementException)
                {
                    span = DateTime.Now - start;
                    Thread.Sleep(300);

                }

            }
            return false;
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                var result = spider.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;

            }           
        }
        private bool TryGetElement(By by,out IWebElement webElement)
        {
            try
            {
                var result = spider.FindElement(by);
                webElement = result;
                return true;
            }
            catch (NoSuchElementException)
            {
                webElement = null;
                return false;

            }
        }
        private bool IsElementPresent(By by, IWebElement webElement)
        {
            try
            {
                var result = webElement.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;

            }
        }

        private bool TryGetElement(By by,IWebElement webElement,out IWebElement aimElement)
        {
            try
            {
                var result = webElement.FindElement(by);
                aimElement = result;
                return true;
            }
            catch (NoSuchElementException)
            {
                aimElement = null;
                return false;

            }
        }

        private bool IsElementPresentWithTime(By by,IWebElement webElement ,int timeLimit = 2)
        {
            DateTime start = DateTime.Now;
            TimeSpan span = DateTime.Now - start;
            while (span.TotalSeconds < timeLimit)
            {
                try
                {
                    var result = webElement.FindElement(by);
                    return true;
                }
                catch (NoSuchElementException)
                {
                    span = DateTime.Now - start;
                    Thread.Sleep(500);

                }

            }
            return false;
        }
        public string Match(string strSource, string pattern)
        {
            var m = Regex.Match(strSource, pattern, RegexOptions.None);

            return m.Success ? m.Groups[1].Value : null;
        }
        #endregion

    }
}
