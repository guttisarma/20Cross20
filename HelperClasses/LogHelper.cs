using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace HelperClasses
{
    //static class should not use here that mistake  //802
    public static class LogHelper
    {
        public static void WriteErrorLog(string Message)
        {
            StreamWriter sw = null;
            try
            {
                string path = ConfigurationManager.AppSettings["LogsPath"].ToString();
                string Filename = DateTime.Today.ToString("ddMMyyyy") + "_Log.txt";
                sw = new StreamWriter(path + "\\" + Filename, true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void WriteErrorLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                string path = ConfigurationManager.AppSettings["LogsPath"].ToString();
                string Filename = DateTime.Today.ToString("ddMMyyyy") + "_Log.txt";
                sw = new StreamWriter(path + "\\" + Filename, true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
                sw.WriteLine(ex);
                sw.Flush();
                sw.Close();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


        public static byte[] ImageToByteArrayFromFilePath(string imagefilePath)
        {
            byte[] imageArray = File.ReadAllBytes(imagefilePath);
            return imageArray;
        }

        public static byte[] MakeThumbnail(byte[] myImage, int thumbWidth, int thumbHeight)
        {

            using (MemoryStream ms = new MemoryStream())
            using (System.Drawing.Image thumbnail = System.Drawing.Image.FromStream(new MemoryStream(myImage)).GetThumbnailImage(thumbWidth, thumbHeight, null, new IntPtr()))
            {

                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public static void TraceLog(string FileName, string Message)
        {
            StreamWriter sw = null;
            try
            {
                string path = ConfigurationManager.AppSettings["LogsPath"].ToString();
                string Filename = FileName + "_" + DateTime.Today.ToString("ddMMyyyy") + "_Log.txt";
                sw = new StreamWriter(path + "\\" + Filename, true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void TraceLog(string FileName, Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                string path = ConfigurationManager.AppSettings["LogsPath"].ToString();
                string Filename = FileName + "_" + DateTime.Today.ToString("ddMMyyyy") + "_Log.txt";
                sw = new StreamWriter(path + "\\" + Filename, true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex);
                sw.Flush();
                sw.Close();
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        public static byte[] MakeThumbnailFromPath(string imagefilePath, int thumbWidth, int thumbHeight)
        {
            byte[] myImage = ImageToByteArrayFromFilePath(imagefilePath);
            using (MemoryStream ms = new MemoryStream())
            using (System.Drawing.Image thumbnail = System.Drawing.Image.FromStream(new MemoryStream(myImage)).GetThumbnailImage(thumbWidth, thumbHeight, null, new IntPtr()))
            {

                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }



        public static string NumberToWords(int inputNumber)
        {
            string[] Ones = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Ninteen" };
            string[] Tens = { "Ten", "Twenty", "Thirty", "Fourty", "Fift", "Sixty", "Seventy", "Eighty", "Ninty" };

            int no = inputNumber;
            string strWords = "";

            if (no > 999 && no < 10000)
            {
                int i = no / 1000;
                strWords = strWords + Ones[i - 1] + " Thousand ";
                no = no % 1000;
            }


            if (no > 99 && no < 1000)
            {
                int i = no / 100;
                strWords = strWords + Ones[i - 1] + " Hundred ";
                no = no % 100;
            }

            if (no > 19 && no < 100)
            {
                int i = no / 10;
                strWords = strWords + Tens[i - 1] + " ";
                no = no % 10;
            }

            if (no > 0 && no < 20)
            {
                strWords = strWords + Ones[no - 1];
            }
            return strWords;
        }
    }
}
