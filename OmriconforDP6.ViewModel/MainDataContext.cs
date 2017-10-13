using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BingLibrary.hjb;
using BingLibrary.hjb.Intercepts;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using ViewROI;
using HalconDotNet;
using Leader.DeltaAS300ModbusTCP;
using System.IO;
using 臻鼎科技OraDB;
using System.Data;
using System.Windows.Forms;
using SxjLibrary;
using OfficeOpenXml;

using System.Windows.Threading;

namespace OmriconforDP6.ViewModel
{
    [BingAutoNotify]
    public class MainDataContext : DataSource
    {
        public class DP6SQLROW
        {
            public string BLDATE { get; set; }   //折线作业时间
            public string BLID { get; set; }     //折线治具编号
            public string BLNAME { get; set; }  //折线治具名称
            public string BLUID { get; set; }  //折线人员
            public string BLMID { get; set; }   //折线机台编号
            public string Bar { get; set; }    //单pcs条码
        }
        #region 属性【绑定】
        #region halcon绑定
        public virtual HImage hImage { set; get; }
        public virtual ObservableCollection<HObject> hObjectList { set; get; }
        public virtual ObservableCollection<ROI> ROIList { set; get; } = new ObservableCollection<ROI>();
        public virtual int ActiveIndex { set; get; }
        public virtual bool Repaint { set; get; }
        #endregion
        public virtual string MsgText { set; get; }
        public virtual bool PLCConnect { set; get; }
        public virtual bool IsTCPConnect { set; get; }
        public virtual string AS300IP { set; get; }
        public virtual string HScriptFileName { get; set; }
        public virtual DataTable SinglDt { set; get; }
        public virtual string BLID { set; get; }
        public virtual string BLUID { set; get; }
        public virtual string BLMID { set; get; }
        public virtual string BLNAME { set; get; }
        public virtual string HomePageVisibility { set; get; }
        public virtual string ParameterVisibility { set; get; }
        public virtual string BarcodeRecordVisibility { set; get; }
        public virtual string MainWindowVisibility { set; get; }
        public virtual bool IsLoadin { get; set; }
        public virtual ObservableCollection<DP6SQLROW> BarcodeRecord { set; get; } = new ObservableCollection<DP6SQLROW>();
        #endregion
        #region 方法【绑定】
        /// <summary>
        /// 程序关闭
        /// </summary>
        public void AppClosed()
        {
            WriteIni();
            aS300ModbusTCP.CloseClass();
        }
        /// <summary>
        /// 程序加载完
        /// </summary>
        public async void AppLoaded()
        {
            mydialog.changeaccent("Red");
            MainWindowVisibility = "Collapsed";
            string str = await mydialog.showinput("请输入作业员编号");
            if (str == "")
            {
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                BLUID = str;
                Inifile.INIWriteValue(iniParameterPath, "SQLMSG", "BLUID", str);
                mydialog.changeaccent("Lime");
                MainWindowVisibility = "Visible";
            }
        }
        public void Selectfile(object p)
        {
            switch (p.ToString())
            {
                case "1":
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Filter = "视觉文件(*.hdev)|*.hdev|所有文件(*.*)|*.*";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        HScriptFileName = dlg.FileName;
                        Inifile.INIWriteValue(iniParameterPath, "Camera", "HScriptFileName", HScriptFileName);
                    }
                    dlg.Dispose();
                    break;
                default:
                    break;
            }
        }
        public async void ChoosePage(object p)
        {
            switch (p.ToString())
            {
                case "0":
                    HomePageVisibility = "Visible";
                    ParameterVisibility = "Collapsed";
                    BarcodeRecordVisibility = "Collapsed";
                    IsLoadin = false;
                    break;
                case "1":
                    HomePageVisibility = "Collapsed";
                    ParameterVisibility = "Visible";
                    BarcodeRecordVisibility = "Collapsed";
                    break;
                case "2":
                    BarcodeRecordVisibility = "Visible";
                    HomePageVisibility = "Collapsed";
                    ParameterVisibility = "Collapsed";
                    IsLoadin = false;
                    break;
                case "4":
                    List<string> r;
                    if (!IsLoadin)
                    {
                        MainWindowVisibility = "Collapsed";
                        r = await mydialog.showlogin();
                        if (r[1] == "jsldr")
                        {
                            IsLoadin = true;
                        }
                        MainWindowVisibility = "Visible";
                    }
                    else
                    {
                        IsLoadin = false;
                    }
                    break;
                default:
                    break;
            }
        }
        public void TakePhoto()
        {
            Async.RunFuncAsync(hcInspect, hcInspectCallback);
        }
        public void FunctionTest()
        {
            DP6SQLROW dP6SQLROW = new DP6SQLROW();
            dP6SQLROW.BLDATE = DateTime.Now.ToString();
            dP6SQLROW.BLID = BLID;
            dP6SQLROW.BLMID = BLMID;
            dP6SQLROW.BLNAME = BLNAME;
            dP6SQLROW.BLUID = BLUID;
            dP6SQLROW.Bar = "123";
            WriteRecordtoExcel(dP6SQLROW, false);
        }
        #endregion
        #region 变量        
        private string MessageStr = "";
        AS300ModbusTCP aS300ModbusTCP;
        string iniParameterPath = System.Environment.CurrentDirectory + "\\Parameter.ini";
        private HdevEngine hdevEngine;
        object PlcRW = new object();
        private dialog mydialog = new dialog();
        readonly AsyncLock m_lock = new AsyncLock();
        #endregion
        #region 其他方法
        #region 参数读写
        private void ReadIni()
        {
            try
            {
                AS300IP = Inifile.INIGetStringValue(iniParameterPath, "AS300", "AS300IP", "192.168.1.5");
                HScriptFileName = Inifile.INIGetStringValue(iniParameterPath, "Camera", "HScriptFileName", @"D:\test.hdev");
                BLID = Inifile.INIGetStringValue(iniParameterPath, "SQLMSG", "BLID", "Null");
                BLUID = Inifile.INIGetStringValue(iniParameterPath, "SQLMSG", "BLUID", "Null");
                BLMID = Inifile.INIGetStringValue(iniParameterPath, "SQLMSG", "BLMID", "Null");
                BLNAME = Inifile.INIGetStringValue(iniParameterPath, "SQLMSG", "BLNAME", "Null");
                MsgText = AddMessage("参数读取完成");
            }
            catch (Exception ex)
            {
                MsgText = AddMessage("参数读取失败: " + ex.Message);
            }
        }
        private void WriteIni()
        {
            try
            {
                Inifile.INIWriteValue(iniParameterPath, "AS300", "AS300IP", AS300IP);
                MsgText = AddMessage("参数写入完成");
            }
            catch (Exception ex)
            {
                MsgText = AddMessage("参数写入失败: " + ex.Message);
            }
        }
        #endregion
        #region 独立线程运行函数
        private void PLCRun()
        {
            bool[] AS300_M;
            bool[] m = new bool[96];
            do
            {
                System.Threading.Thread.Sleep(100);
                try
                {
                    //最大100
                    lock (PlcRW)
                    {
                        AS300_M = aS300ModbusTCP.ReadCoils("M5000", 96);
                    }
                    PLCConnect = true;
                    if (m[0] != AS300_M[0])
                    {
                        m[0] = AS300_M[0];
                        if (m[0])
                        {
                            Async.RunFuncAsync(hcInspect, hcInspectCallback);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    PLCConnect = false;
                }
            } while (true);
        }
        /// <summary>
        /// 脚本执行完，给PLC赋值
        /// </summary>
        private async void hcInspectCallback()
        {
            lock (PlcRW)
            {
                aS300ModbusTCP.WriteSigleCoil("M5001", true);
            }
            if (true)
            {
                DP6SQLROW dP6SQLROW = new DP6SQLROW();
                dP6SQLROW.BLDATE = DateTime.Now.ToString();
                dP6SQLROW.BLID = BLID;
                dP6SQLROW.BLMID = BLMID;
                dP6SQLROW.BLNAME = BLNAME;
                dP6SQLROW.BLUID = BLUID;
                dP6SQLROW.Bar = "123";
                using (var releaser = await m_lock.LockAsync())
                {
                    bool r = await Update_A_Row(dP6SQLROW);
                    if (r)
                    {
                        //上传成功
                    }
                    else
                    {
                        //上传失败
                    }
                }
            }
        }

        #endregion
        #region Halcon运行
        private void hcInit()
        {
            hdevEngine = new HdevEngine();
            try
            {
                if (HScriptFileName != System.Environment.CurrentDirectory + "\\" + Path.GetFileNameWithoutExtension(HScriptFileName) + ".hdev")
                {
                    File.Copy(HScriptFileName, System.Environment.CurrentDirectory + "\\" + Path.GetFileNameWithoutExtension(HScriptFileName) + ".hdev", true);
                }
                hdevEngine.initialengine(Path.GetFileNameWithoutExtension(HScriptFileName));
                hdevEngine.loadengine();
                MsgText = AddMessage("初始化视觉引擎");
                if (!Directory.Exists(@"D:\images"))
                {
                    Directory.CreateDirectory(@"D:\images");
                }
                string[] filenames = Directory.GetFiles(@"D:\images");
                if (filenames.Length >1000)
                {
                    foreach (string item in filenames)
                    {
                        File.Delete(item);
                    }
                }
            }
            catch (Exception ex)
            {

                MsgText = AddMessage("初始化视觉引擎失败: " + ex.Message);
            }
        }
        private void hcInspect()
        {
            ObservableCollection<HObject> objectList = new ObservableCollection<HObject>();
            //执行脚本1次
            hdevEngine.inspectengine();
            //获取图像->更新界面
            hImage = hdevEngine.getImage("Image");
            //获取Region->更新界面
            objectList.Add(hdevEngine.getRegion("Regions1"));
            objectList.Add(hdevEngine.getRegion("Regions2"));
            hObjectList = objectList;
            //获得脚本里的参数
            var mo1 = hdevEngine.getmeasurements("mo1");
        }
        #endregion
        #region 数据库操作
        private async Task<bool> Update_A_Row(DP6SQLROW row)
        {

            return await ((Func<Task<bool>>)(() =>
            {
                return Task.Run(() =>
                {
                    try
                    {
                        string[] arrField = new string[1];
                        string[] arrValue = new string[1];
                        OraDB oraDB = new OraDB("qwer", "sfcabar", "sfcabar*168");
                        string tablename = "sfcdata.barautbind";
                        if (oraDB.isConnect())
                        {
                            IsTCPConnect = true;
                            arrField[0] = "SCBARCODE";
                            arrValue[0] = row.Bar;
                            DataSet s = oraDB.selectSQL(tablename.ToUpper(), arrField, arrValue);
                            SinglDt = s.Tables[0];
                            if (SinglDt.Rows.Count == 0)
                            {
                                MsgText = AddMessage("未查询到 " + row.Bar + " 信息");
                                oraDB.disconnect();
                                return false;
                            }
                            else
                            {
                                string[,] arrFieldAndNewValue = { { "BLDATE", ("to_date('" + row.BLDATE + "', 'yyyy/mm/dd hh24:mi:ss')").ToUpper() }, { "BLID", row.BLID }, { "BLNAME", row.BLNAME }, { "BLUID", row.BLUID }, { "BLMID", row.BLMID } };
                                string[,] arrFieldAndOldValue = { { "SCBARCODE", row.Bar } };
                                oraDB.updateSQL2(tablename.ToUpper(), arrFieldAndNewValue, arrFieldAndOldValue);
                                s = oraDB.selectSQL(tablename.ToUpper(), arrField, arrValue);
                                SinglDt = s.Tables[0];
                                oraDB.disconnect();
                                MsgText = AddMessage("条码 " + row.Bar + " 已更新");
                                return true;
                            }
                        }
                        else
                        {
                            IsTCPConnect = false;
                            MsgText = AddMessage("数据库链接失败");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        IsTCPConnect = false;
                        MsgText = AddMessage("数据插入失败: " + ex.Message);
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                    
                }
                    );
            }))();
        }
        #endregion
        private void LoadExcelFile()
        {
            if (!Directory.Exists(@"D:\Record"))
            {
                Directory.CreateDirectory(@"D:\Record");
            }
            if (!Directory.Exists(@"D:\Alarm"))
            {
                Directory.CreateDirectory(@"D:\Alarm");
            }
        }
        private string GetBanci()
        {
            string bancitime;
            if (DateTime.Now.Hour < 8)
            {
                bancitime = DateTime.Now.AddDays(-1).ToLongDateString();
            }
            else
            {
                bancitime = DateTime.Now.ToLongDateString();
            }
            return bancitime + (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 20 ? "Day" : "Night");
        }
        private void WriteRecordtoExcel(DP6SQLROW dP6SQLROW,bool flag)
        {
            string bcstr = GetBanci();
            string filename = @"D:\Record\" + bcstr + ".csv";
            if (!flag)
            {
                filename = @"D:\NotUpdate.csv";
            }
            try
            {
                if (!File.Exists(filename))
                {
                    string[] heads = { "BLDATE", "BLID", "BLNAME", "BLUID", "BLMID", "Bar" };
                    Csvfile.AddNewLine(filename, heads);
                }
                string[] conte = { dP6SQLROW.BLDATE, dP6SQLROW.BLID, dP6SQLROW.BLNAME, dP6SQLROW.BLUID, dP6SQLROW.BLMID, dP6SQLROW.Bar };
                Csvfile.AddNewLine(filename, conte);
            }
            catch (Exception ex)
            {
                MsgText = AddMessage(ex.Message);   
            }
        }
        /// <summary>
        /// 打印窗口字符处理函数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string AddMessage(string str)
        {
            string[] s = MessageStr.Split('\n');
            if (s.Length > 1000)
            {
                MessageStr = "";
            }
            MessageStr += "\n" + System.DateTime.Now.ToString() + " " + str;
            return MessageStr;
        }
        #endregion
        #region 构造函数
        public MainDataContext()
        {
            ReadIni();
            aS300ModbusTCP = new AS300ModbusTCP(AS300IP);
            LoadExcelFile();
            hcInit();
            Async.RunFuncAsync(PLCRun,null); 
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
