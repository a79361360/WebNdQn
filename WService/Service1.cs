using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace WService
{
    public partial class Service1 : ServiceBase
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        int Num = 0;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            //timer.Interval = 240000;
            timer.Interval = 10000;
            timer.Enabled = true;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Num++;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("G:\\log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Stop.");
            }
        }
        private void ToExeclSendMsgCode() {
            //MobileBLL mbll = new MobileBLL();
            //try {
            //    mbll.ExecuteCooperList();
            //}
            //catch (Exception er) {
            //    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("G:\\log_ExecuteCooperList.txt", true))
            //    {
            //        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "出现异常" + er.Message);
            //    }
            //}
        }
        protected override void OnStop()
        {
        }
    }
}
