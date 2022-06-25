using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FootBallDataHelper
{
    public class FBDHelper
    {
        IWebDriver spider { get; set; }
        
        //打开指定日期的完成比赛
        //https://live.leisu.com/wanchang-20220622
        string wanchengUrl = "https://live.leisu.com/wanchang-";
        public bool OpenTimeFinishPage(string time)
        {
            spider = new ChromeDriver();
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

        #region 数据解析
        


        #endregion


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
        #endregion

    }
}
