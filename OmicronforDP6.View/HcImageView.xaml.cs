using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HalconDotNet;
using ViewROI;
using System.Collections.ObjectModel;

namespace OmicronforDP6.View
{
    /// <summary>
    /// HcImageView.xaml 的交互逻辑
    /// </summary>
    public partial class HcImageView : UserControl
    {
        public ROIController roiController;
        public HWndCtrl viewController;
        public HcImageView()
        {
            InitializeComponent();
            roiController = new ROIController();
            viewController = new HWndCtrl(this.Viewer);
            viewController.useROIController(roiController);
            viewController.setViewState(HWndCtrl.MODE_VIEW_MOVE);

            roiController.ActiveChanged += roiController_ActiveChanged;
            roiController.ROIChanged += roiController_ROIChanged;
        }

        public bool ROIChanged
        {
            get { return (bool)GetValue(ROIChangedProperty); }
            set { SetValue(ROIChangedProperty, value); }
        }
        public static readonly DependencyProperty ROIChangedProperty =
            DependencyProperty.Register("ROIChanged", typeof(bool), typeof(HcImageView), new PropertyMetadata(
                new PropertyChangedCallback((d, e) =>
                {
                    var imageViewer = d as HcImageView;
                    imageViewer.ROIChanged = (bool)e.NewValue;
                })));
        void roiController_ROIChanged(object sender, EventArgs e)
        {
            ROIChanged = !ROIChanged;
        }

        void roiController_ActiveChanged(object sender, EventArgs e)
        {
            ActiveIndex = roiController.activeROIidx;
        }
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(HImage), typeof(HcImageView), new PropertyMetadata(
                new PropertyChangedCallback((d, e) =>
                {
                    var imageViewer = d as HcImageView;
                    var image = e.NewValue as HImage;
                    if (image != null)
                    {
                        //imageViewer.Image = image;
                        try
                        {
                            imageViewer.viewController.addIconicVar(image);
                        }
                        catch { }

                        imageViewer.viewController.repaint();
                        GC.Collect();

                        //imageViewer.Viewer.roiController.reset();
                    }
                }
                  )
                ));
        public HImage Image
        {
            get { return (HImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }


        public static readonly DependencyProperty HObjectListProperty =
            DependencyProperty.Register("HObjectList", typeof(ObservableCollection<HObject>), typeof(HcImageView), new PropertyMetadata(
                new PropertyChangedCallback((d, e) =>
                {
                    var imageViewer = d as HcImageView;
                    var HObjectList = e.NewValue as ObservableCollection<HObject>;
                    if (HObjectList == null)
                    { }
                    else
                    {
                        for (int i = 0; i < HObjectList.Count; i++)
                        {
                            try
                            {
                                imageViewer.viewController.addIconicVar(HObjectList[i]);
                            }
                            catch { }
                        }
                    }
                    imageViewer.viewController.repaint();
                    GC.Collect();
                })));
        public ObservableCollection<HObject> HObjectList
        {
            get { return (ObservableCollection<HObject>)GetValue(HObjectListProperty); }
            set { SetValue(HObjectListProperty, value); }
        }



        public static readonly DependencyProperty ROIListProperty =
            DependencyProperty.Register("ROIList", typeof(ObservableCollection<ROI>), typeof(HcImageView), new PropertyMetadata(
                new PropertyChangedCallback((d, e) =>
                {
                    var imageViewer = d as HcImageView;
                    var ROIList = e.NewValue as ObservableCollection<ROI>;
                    if (ROIList == null)
                        imageViewer.roiController.ROIList.Clear();

                    else
                        imageViewer.roiController.ROIList = ROIList;
                })));
        public ObservableCollection<ROI> ROIList
        {
            get { return (ObservableCollection<ROI>)GetValue(ROIListProperty); }
            set { SetValue(ROIListProperty, value); }
        }
        public static DependencyProperty ActiveIndexProperty =
            DependencyProperty.Register("ActiveIndex", typeof(int), typeof(HcImageView), new PropertyMetadata(
                new PropertyChangedCallback((d, e) =>
                {
                    var imageViewer = d as HcImageView;
                    var mActiveIndex = (int)e.NewValue;
                    imageViewer.roiController.activeROIidx = mActiveIndex;
                })));
        public int ActiveIndex
        {
            get { return (int)GetValue(ActiveIndexProperty); }
            set { SetValue(ActiveIndexProperty, value); }
        }
        public static readonly DependencyProperty SizeEnableProperty =
            DependencyProperty.Register("SizeEnable", typeof(bool), typeof(HcImageView), new PropertyMetadata(
                new PropertyChangedCallback((d, e) =>
                {
                    var imageViewer = d as HcImageView;
                    imageViewer.viewController.setViewState(HWndCtrl.MODE_VIEW_MOVE);
                    imageViewer.viewController.repaint();
                })));
        public bool SizeEnable
        {
            get { return (bool)GetValue(SizeEnableProperty); }
            set { SetValue(SizeEnableProperty, value); }
        }

        public static readonly DependencyProperty RepaintProperty =
            DependencyProperty.Register("Repaint", typeof(bool), typeof(HcImageView), new PropertyMetadata(
                new PropertyChangedCallback((d, e) =>
                {
                    var imageViewer = d as HcImageView;
                    imageViewer.viewController.repaint();
                })));
        public bool Repaint
        {
            get { return (bool)GetValue(RepaintProperty); }
            set { SetValue(RepaintProperty, value); }
        }

    }
}
