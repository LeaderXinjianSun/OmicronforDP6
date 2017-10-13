using System;
using HalconDotNet;

namespace ViewROI
{
    public class ROIRegion:ROI
    {
        public HRegion mCurHRegion;

        public ROIRegion(HRegion r)
		{
            mCurHRegion = r;
        }
        public override void draw(HalconDotNet.HWindow window)
        {
            //window.SetColor("white");
            //window.SetLineStyle(0);
            //window.SetLineWidth(1);
            window.DispRegion(mCurHRegion);
        }
    }
}
