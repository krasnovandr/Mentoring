using System;
using System.Collections.Generic;
using System.Linq;
using ProfileSample.Models;

namespace ProfileSample.DAL
{
    public class ImageRepository : IDisposable
    {
        private readonly ProfileSampleEntities _context;
        bool _disposed;

        public ImageRepository()
        {
            _context = new ProfileSampleEntities();
        }

        public List<ImageViewModel> GetImagesId()
        {
            var images = _context.ImgSources.Select(item => new ImageViewModel
            {
                Id = item.Id
            }).ToList();
            
            return images;
        }

        public byte[] GetImage(int id)
        {
            var image = _context.ImgSources.Find(id);
            return image.Data;
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
                _context.Dispose();
            }

            _disposed = true;
        }
    }
}