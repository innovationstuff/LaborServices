using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace LaborServices.Web.Helpers
{
    class ImageDimensionAttribute : ValidationAttribute
    {
        private readonly int _height;
        private readonly int _width;

        public ImageDimensionAttribute(int width, int height)
        {
            _height = height;
            _width = width;
        }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return true;
            }

            try
            {
                using (var image = Image.FromStream(file.InputStream))
                {
                    return (image.RawFormat.Equals(ImageFormat.Png) || image.RawFormat.Equals(ImageFormat.Jpeg)) && (image.Width == _width && image.Height == _height);
                }
            }
            catch
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("Please select a {0} x {1} pixel image", _width, _height);
        }
    }
}
