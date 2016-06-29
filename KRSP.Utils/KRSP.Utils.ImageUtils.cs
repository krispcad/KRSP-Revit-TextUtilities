using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;

namespace KRSP.Utils
{
	public class ImageUtils
	{
        public static BitmapSource GetSource(System.Drawing.Image img)
        {
            BitmapImage bmImg = new BitmapImage();

            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                bmImg.BeginInit();
                bmImg.CacheOption = BitmapCacheOption.OnLoad;
                bmImg.UriSource = null;
                bmImg.StreamSource = ms;
                bmImg.EndInit();
            }
            return bmImg;
        }
	}
}
