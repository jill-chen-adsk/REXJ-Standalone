// ko-mimura 2019/06/10 ko-mimura
// チーズの回転コマンド
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Windows.Input;
namespace CmdRotateTees
{
	public partial class UserControl1 : UserControl
	{
		private const double tol = 10e-6;
		private static FamilySymbol debugSymbol = null;
		private void ASSERT(bool b) 
		{
			System.Diagnostics.Debug.Assert(b);
		}
		private bool _Equal(double t1, double t2) 
		{
			return MEPCommon.MepCommon.Equal(t1, t2);
			//return System.Math.Abs(t1 - t2) < tol;
		}
		private bool _Equal(ElementId t1, ElementId t2) 
		{
			return MEPCommon.MepCommon.Equal(t1, t2);
			//return t1.Equals(t2);
		}
		private void Rotate(double angle) 
		{
			m_IF.MainWindowIF_Rotate(angle);
		}
		private void Rotate(bool bNegate)
		{
			double angle = 0; {
				bool b = double.TryParse(this.txtAngle.Text, out angle);
				if (!b) {
					angle = 0;
					this.txtAngle.Text = "0";
				}
				angle = MEPCommon.MepCommon.D2R(angle);
				if (bNegate)
					angle = -angle;
			}
			Rotate(angle);
			m_angle += angle;
		}
		private void Click_RotateMinus(object sender, RoutedEventArgs e)
		{
			using (var tran = new Transaction(MEPCommon.MepCommon.m_doc, "Click_RotateMinus")) {
				tran.Start(); {
					Rotate(true);
				} tran.Commit();
			}
		}
		private void Click_RotatePlus(object sender, RoutedEventArgs e)
		{
			using (var tran = new Transaction(MEPCommon.MepCommon.m_doc, "Click_RotatePlus")) {
				tran.Start(); {
					Rotate(false);
				} tran.Commit();
			}
		}
		private double m_angle = 0;
		private void Click_Rest(object sender, RoutedEventArgs e) 
		{
			m_IF.MainWindowIF_Reset();
			m_IF.MainWindowIF_ChangeConnector(txtAngle);
			//using (var tran = new Transaction(m_doc, "Click_Rest")) {
			//	tran.Start();{UserControl1_IF_Rest
			//		m_IF.();
			//	} tran.Commit();
			//}
		}
		private void Click_btnChangeConnector(object sender, RoutedEventArgs e) 
		{
			m_IF.MainWindowIF_ChangeConnector(txtAngle);
		}
		private void Click_btnCommit(object sender, RoutedEventArgs e) 
		{
			m_bCommit = true;
			m_win.Close();
			return;
		}
		private void HandleEsc(object sender, KeyEventArgs e) 
		{
			if (e.Key == Key.Escape) {
				Click_Rest(sender, e);
				m_win.Close();
			}
		}
		public bool m_bCommit;
		private Window             m_win;
		private MainWindowIF m_IF;
		public UserControl1(Window win, MainWindowIF IF)// MEPCurve mepCurve)
                {
			m_bCommit = false;
			m_win = win;
			m_IF = IF;
			InitializeComponent();
			this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
			Click_btnChangeConnector(null, null);
		}
		~UserControl1()
		{
		}
	}
}
