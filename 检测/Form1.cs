using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.MonthCalendar;

namespace 检测
{
    public partial class Form1 : Form
    {
        private static HWindow hwindow;
        public HTuple ImagePath = new HTuple();
        HObject ho_Rectangle = null;
        HObject ho_Contours;
        HTuple ImageHeight = null, ImageWidth = null, hv_Number=null, hv_Area=null, hv_Row=null, hv_Column = null, hv_Area3 = null, hv_Row3= null, hv_Column3 = null;
        private HObject getImage = new HObject();
        private HObject getImage1 = new HObject();
        HObject ho_Image, ho_RegionFillUp, ho_Region, ho_RegionOpening, ho_ConnectedRegions, ho_SelectedRegions, ho_ReducedImage, ho_Region1, ho_ConnectedRegions1, ho_Rectangle1, ho_ImageReduced;



        private void button3_Click(object sender, EventArgs e)
        {
            hwindow.ClearWindow();
            hwindow = null;
        }

        HTuple hv_Index = new HTuple();
        HTuple hv_Row1 = new HTuple();
        HTuple hv_Column1 = new HTuple(), hv_Row2 = new HTuple();
        HTuple hv_Column2 = new HTuple();
        public Form1()
        {
            InitializeComponent();
        }

        private void hWindowControl1_HMouseWheel(object sender, HMouseEventArgs e)
        {
            try
            {
                HOperatorSet.SetColor(hwindow, "red");
                HTuple Zoom, Row, Col, Button;
                HTuple Row0, Column0, Row00, Column00, Ht, Wt, r1, c1, r2, c2;
                if (e.Delta > 0)
                {
                    Zoom = 1.5;
                }
                else
                {
                    Zoom = 0.5;
                }
                HOperatorSet.GetMposition(hwindow, out Row, out Col, out Button);
                HOperatorSet.GetPart(hwindow, out Row0, out Column0, out Row00, out Column00);
                Ht = Row00 - Row0;
                Wt = Column00 - Column0;
                if (Ht * Wt < 32000 * 32000 || Zoom == 1.5)
                {
                    r1 = (Row0 + ((1 - (1.0 / Zoom)) * (Row - Row0)));
                    c1 = (Column0 + ((1 - (1.0 / Zoom)) * (Col - Column0)));
                    r2 = r1 + (Ht / Zoom);
                    c2 = c1 + (Wt / Zoom);
                    HOperatorSet.SetPart(hwindow, r1, c1, r2, c2);
                    HOperatorSet.ClearWindow(hwindow);
                    HOperatorSet.DispObj(getImage, hwindow);
                    HOperatorSet.DispObj(ho_Region1, hwindow);
                    HTuple end_val13 = hv_Number - 1;
                    HTuple step_val13 = 1;
                    for (hv_Index = 0; hv_Index.Continue(end_val13, step_val13); hv_Index = hv_Index.TupleAdd(step_val13))
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_Row.TupleSelect(hv_Index),
                                hv_Column.TupleSelect(hv_Index), 0, 10, 10);
                            HOperatorSet.DispObj(ho_Rectangle, hwindow);
                        }
                    }
                    if ((int)(new HTuple(hv_Number.TupleGreater(0))) != 0)
                    {
                        HDevelopExport.disp_message(hwindow, "NG", "image", 12, 12, "red", "false");
                    }
                    else
                    {
                        HDevelopExport.disp_message(hwindow, "OK", "image", 12, 12, "green", "false");
                    }
                    HOperatorSet.SetColor(hwindow, "green");
                    HOperatorSet.DispObj(ho_Contours, hwindow);
                }

            }
            catch { }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            HOperatorSet.SetColor(hwindow, "red");
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ReducedImage);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            hWindowControl1.Focus();
            HOperatorSet.DrawRectangle1(hwindow, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
            HOperatorSet.GenRectangle1(out ho_Rectangle1, hv_Row1, hv_Column1, hv_Row2, hv_Column2);
            HOperatorSet.AreaCenter(ho_Rectangle1, out hv_Area3, out hv_Row3, out hv_Column3);
            HOperatorSet.ReduceDomain(getImage, ho_Rectangle1, out ho_ImageReduced);
            HOperatorSet.GenContourRegionXld(ho_Rectangle1, out ho_Contours, "border");
            HOperatorSet.SetColor(hwindow, "green");
            HOperatorSet.DispObj(ho_Contours, hwindow);
            HOperatorSet.SetColor(hwindow, "red");
            HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 0, 40);
            ho_RegionFillUp.Dispose();
            HOperatorSet.FillUpShape(ho_Region, out ho_RegionFillUp, "area", 1, 100);
            ho_RegionOpening.Dispose();
            HOperatorSet.OpeningCircle(ho_RegionFillUp, out ho_RegionOpening, 3.5);
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area",70);
            ho_ReducedImage.Dispose();
            HOperatorSet.ReduceDomain(getImage, ho_SelectedRegions, out ho_ReducedImage);
            ho_Region1.Dispose();
            HOperatorSet.Threshold(ho_ReducedImage, out ho_Region1, 60, 255);
            ho_ConnectedRegions1.Dispose();
            HOperatorSet.Connection(ho_Region1, out ho_ConnectedRegions1);
            HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_ConnectedRegions1, "area", "and",1, 9999999999);
            HOperatorSet.CountObj(ho_ConnectedRegions1, out hv_Number);
            HOperatorSet.AreaCenter(ho_ConnectedRegions1, out hv_Area, out hv_Row, out hv_Column);
            if (ho_SelectedRegions==null) { }
            double[] Area = hv_Area;
            double[] Y = hv_Row;
            double[] X = hv_Column;
            HTuple end_val13 = hv_Number - 1;
            HTuple step_val13 = 1;
            for (hv_Index = 0; hv_Index.Continue(end_val13, step_val13); hv_Index = hv_Index.TupleAdd(step_val13))
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle, hv_Row.TupleSelect(hv_Index),
                        hv_Column.TupleSelect(hv_Index), 0, 10, 10);
                    HOperatorSet.DispObj(ho_Rectangle, hwindow);
                }
            }
            if ((int)(new HTuple(hv_Number.TupleGreater(0))) != 0)
            {
                HDevelopExport.disp_message(hwindow, "NG", "image", 12, 12, "red", "false");
            }
            else
            {
                HDevelopExport.disp_message(hwindow, "OK", "image", 12, 12, "green", "false");
            }
            textBox1.Text = (hv_Area.Length).ToString();
            textBox2.Text = hv_Area.ToString();
            textBox3.Text = hv_Column.ToString();
            textBox4.Text = hv_Row.ToString();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "tif文件|*.tif*";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImagePath = openFileDialog1.FileName;
                HObject Image;
                HOperatorSet.GenEmptyObj(out Image);
                hwindow = hWindowControl1.HalconWindow;
                hwindow.ClearWindow();
                HOperatorSet.ReadImage(out getImage, ImagePath);
                HOperatorSet.GetImageSize(getImage, out ImageWidth, out ImageHeight);
                HOperatorSet.SetPart(hwindow, 0, 0, ImageHeight - 1, ImageWidth - 1);
                HOperatorSet.DispObj(getImage, hwindow);
                //ew   HOperatorSet.SetColor(hwindow, "red");
            }
        }
    }
}
