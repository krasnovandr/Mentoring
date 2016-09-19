using System;
using System.Drawing;
using System.Web.Mvc;
using ProfileSample.DAL;

namespace ProfileSample.Controllers
{
    public class HomeController : Controller, IDisposable
    {
        private readonly ImageRepository _imageRepository;
        bool _disposed;
        public HomeController()
        {
            _imageRepository = new ImageRepository();
        }
        public ActionResult Index()
        {
            var images = _imageRepository.GetImagesId();
            return View(images);
        }

        [OutputCache(Duration = 120, VaryByParam = "id")]
        public ActionResult GetImage(int? id)
        {
            if (id.HasValue == false)
            {
                return new EmptyResult();
            }
            var image = _imageRepository.GetImage(id.Value);
            var imageResized = ResizeImage(image, 300, 150);
            return File(imageResized, "image/jpeg");
        }

        public byte[] ResizeImage(byte[] data, int widthToResize, int heightToResize)
        {
            int maxwidth = widthToResize;
            int maxheight = heightToResize;

            //convert to full size image
            var ic = new ImageConverter();
            var img = (Image)(ic.ConvertFrom(data)); //original size
            if (img != null && img.Width > maxwidth | img.Height > maxheight) //resize if it is too big
            {
                var bitmap = new Bitmap(maxwidth, maxheight);
                using (var graphics = Graphics.FromImage(bitmap))
                    graphics.DrawImage(img, 0, 0, maxwidth, maxheight);


                data = (byte[])ic.ConvertTo(bitmap, typeof(byte[]));
            }
            return data;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _imageRepository.Dispose();
            }

            _disposed = true;
        }
        //public ActionResult Convert()
        //{
        //    var files = Directory.GetFiles(Server.MapPath("~/Content/Img"), "*.jpg");

        //    using (var context = new ProfileSampleEntities())
        //    {
        //        foreach (var file in files)
        //        {
        //            using (var stream = new FileStream(file, FileMode.Open))
        //            {
        //                byte[] buff = new byte[stream.Length];

        //                stream.Read(buff, 0, (int) stream.Length);

        //                var entity = new ImgSource()
        //                {
        //                    Name = Path.GetFileName(file),
        //                    Data = buff,
        //                };

        //                context.ImgSources.Add(entity);
        //                context.SaveChanges();
        //            }
        //        } 
        //    }

        //    return RedirectToAction("Index");
        //}

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}