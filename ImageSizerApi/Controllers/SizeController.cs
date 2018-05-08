using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace ImageSizerApi.Controllers
{
    [Route("api/[controller]")]
    public class SizeController : Controller
    {

        const string APIKEY = "X";
        const int QUALITY = 75;

        /// <summary>
        /// Resizes an image
        /// </summary>
        /// <param name="url"></param>
        /// <param name="apiKey"></param>
        /// <returns>image byte[]</returns>
        [HttpGet]
        public IActionResult Get(string url, int size, string apiKey)
        {
            if (apiKey != APIKEY)
                return null;

            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(url);

            try
            {

                using (var input = new MemoryStream(imageBytes))
                {
                    using (var inputStream = new SKManagedStream(input))
                    {
                        using (var original = SKBitmap.Decode(inputStream))
                        {
                            int width, height;
                            if (original.Width > original.Height)
                            {
                                width = size;
                                height = original.Height * size / original.Width;
                            }
                            else
                            {
                                width = original.Width * size / original.Height;
                                height = size;
                            }

                            using (var resized = original.Resize(new SKImageInfo(width, height), SKBitmapResizeMethod.Lanczos3))
                            {
                                if (resized == null) return File(imageBytes, "image/jpeg");

                                using (var image = SKImage.FromBitmap(resized))
                                {
                                    using (var output = new MemoryStream())
                                    {
                                        image.Encode(SKEncodedImageFormat.Jpeg, QUALITY).SaveTo(output);

                                        return File(output.ToArray(), "image/jpeg");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return File(imageBytes, "image/jpeg");
            }
        }

    }
}
