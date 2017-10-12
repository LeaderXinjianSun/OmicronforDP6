using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BingLibrary.hjb;
using BingLibrary.hjb.Intercepts;
using System.ComponentModel.Composition;

using System.Windows.Threading;

namespace OmriconforDP6.ViewModel
{
    [BingAutoNotify]
    public class MainDataContext : DataSource
    {
        #region 属性
        public virtual uint Text1 { get; set; } = 0;
        #endregion
        #region 方法
        public void CountOprate(object p)
        {
            switch (p.ToString())
            {
                case "0":
                    Timer.Start();
                    break;
                case "1":
                    Timer.Stop();
                    break;
                case "2":
                    Text1 = 0;
                    break;
                default:
                    break;
            }
        }
        #endregion
        #region 变量
        DispatcherTimer Timer;
        #endregion
        #region 私有方法
        private void Timer_Tick(object sender, System.EventArgs e)
        {
            Text1++;
            if (Text1 >= 65535)
            {
                Text1 = 0;
            }
        }
        #endregion
        #region 构造函数
        public MainDataContext()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(100);
            Timer.Tick += new EventHandler(Timer_Tick);
        }
        #endregion

    }
    class VMManager
    {
        [Export(MEF.Contracts.Data)]
        [ExportMetadata(MEF.Key, "md")]
        MainDataContext md = MainDataContext.New<MainDataContext>();
    }
}
