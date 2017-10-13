using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace ViewROI
{
   public class HdevEngine
    {

        #region engine
        public HWindowControl viewPort = new HWindowControl();
        private HDevEngine engine = new HDevEngine();
        private HDevProgramCall programCall;
        private string programPath;
        private string procedurePath;
        private HDevOpMultiWindowImpl MyHDevOperatorImpl;

        public void initialengine(string filename)
        {
            programPath = System.Environment.CurrentDirectory + @"\" + filename + ".hdev";
            procedurePath = System.Environment.CurrentDirectory + @"\";
            if (!HalconAPI.isWindows)
            {
                programPath = programPath.Replace("\\", "/");
                procedurePath = procedurePath.Replace("\\", "/");
            }
            engine.SetProcedurePath(procedurePath);
            // viewPort.HalconWindow.SetLineWidth(4);
            MyHDevOperatorImpl = new HDevOpMultiWindowImpl(viewPort.HalconWindow);
            engine.SetHDevOperators(MyHDevOperatorImpl);

        }
        public void loadengine()
        {
            try
            {
                HDevProgram program = new HDevProgram(programPath);
                programCall = new HDevProgramCall(program);
            }
            catch { }
        }
        public void disposeengine()
        {
            try
            {
                programCall.Dispose();
            }
            catch { }
        }
        public void inspectengine()
        {
            try
            {
                programCall.Execute();
            }
            catch { }

        }
        public void inspectReset()
        {
            try
            {
                programCall.Reset();
            }
            catch { }

        }
        /// <summary>
        /// 根据参数名称获取对应数值。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public HTuple getmeasurements(string name)
        {
            try { return programCall.GetCtrlVarTuple(name); }
            catch { return ""; }
        }

        public HImage getImage(string name)
        {
            try { return programCall.GetIconicVarImage(name); }
            catch { return null; }
        }
        public HRegion getRegion(string name)
        {
            try { return programCall.GetIconicVarRegion(name); }
            catch { return null; }
        }

        public HObject getObject(string name)
        {
            try { return programCall.GetIconicVarObject(name); }
            catch { return null; }

        }

        #endregion
    }
}
