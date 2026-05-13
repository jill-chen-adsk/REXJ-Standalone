using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ADSK.JExtRAC.ExcelImageInsert.Utils
{
    public static class ExcelHelper
    {
        [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
        private static extern int CLSIDFromProgID(string lpszProgId, out Guid pclsid);

        [DllImport("oleaut32.dll", PreserveSig = true)]
        private static extern int GetActiveObject(ref Guid rclsid, IntPtr reserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);

        private static object TryGetActiveObject(string progId)
        {
            int hr = CLSIDFromProgID(progId, out Guid clsid);
            if (hr != 0)
                return null;
            hr = GetActiveObject(ref clsid, IntPtr.Zero, out object obj);
            if (hr != 0)
                return null;
            return obj;
        }

        public static bool IsExcelRunning()
        {
            Process[] processes = Process.GetProcessesByName("EXCEL");
            return processes.Length > 0;
        }

        public static string CaptureSelectionToFile(Components.Attribute cmpAttribute, string fileName)
        {
            object excelApp = TryGetActiveObject("Excel.Application");
            if (excelApp == null)
                return cmpAttribute.ResourceText("IDS_ERR_STARTEXCEL");

            try
            {
                dynamic app = excelApp;
                app.Visible = true;
                app.UserControl = true;

                dynamic selection = app.Selection;
                if (selection == null)
                    return cmpAttribute.ResourceText("IDS_ERR_EXCELRANGESELECT");

                try
                {
                    selection.CopyPicture(1 /* xlScreen */, 2 /* xlBitmap */);
                }
                catch
                {
                    return cmpAttribute.ResourceText("IDS_ERR_EXCELRANGESELECT");
                }

                IDataObject dataObj = Clipboard.GetDataObject();
                if (dataObj != null && dataObj.GetDataPresent(DataFormats.Bitmap))
                {
                    using (Bitmap bmp = (Bitmap)dataObj.GetData(DataFormats.Bitmap))
                    {
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                        ImageCodecInfo bmpCodec = GetEncoderInfo("image/bmp");
                        if (bmpCodec != null)
                            bmp.Save(fileName, bmpCodec, encoderParameters);
                        else
                            bmp.Save(fileName, ImageFormat.Bmp);
                    }
                }
                else
                {
                    return cmpAttribute.ResourceText("IDS_ERR_SETCLIPBOARD");
                }

                return null;
            }
            finally
            {
                if (excelApp != null)
                    Marshal.ReleaseComObject(excelApp);
            }
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == mimeType)
                    return codec;
            }
            return null;
        }
    }
}
