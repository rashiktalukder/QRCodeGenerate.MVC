using Microsoft.AspNetCore.Mvc;
using QRCodeGenerate.MVC.Models;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using static QRCoder.PayloadGenerator;

namespace QRCodeGenerate.MVC.Controllers
{
    public class QRCodeGeneratorController : Controller
    {
        public IActionResult Index()
        {
            QrCodeModel qrCodeModel = new QrCodeModel();

            return View(qrCodeModel);
        }

        [HttpPost]
        public IActionResult Index(QrCodeModel qrCodeModel)
        {
            Payload payload = PayloadBasedOnQrCodeType(qrCodeModel);

            QRCodeGenerator qrCodeGen = new QRCodeGenerator();
            QRCodeData qrCodeData = qrCodeGen.CreateQrCode(payload);
            QRCode qrCode = new QRCode(qrCodeData);
            var qrCodeAsBitmap = qrCode.GetGraphic(20);

            string base64Str = Convert.ToBase64String(BitmapToByteArr(qrCodeAsBitmap));
            qrCodeModel.QRImageURL = "data:image/png;base64," + base64Str;

            return View("Index",qrCodeModel);
        }

        public Payload PayloadBasedOnQrCodeType(QrCodeModel qrCodeModel) 
        {
            Payload payload = null;

            if(qrCodeModel.QrCodeType == "website")
            {
                payload = new Url(qrCodeModel.WebsiteURL);
            }
            else if(qrCodeModel.QrCodeType == "bookmark")
            {
                payload = new Bookmark(qrCodeModel.BookmarkURL, qrCodeModel.BookmarkURL);
            }
            else if (qrCodeModel.QrCodeType == "sms")
            {
                payload = new SMS(qrCodeModel.SMSPhoneNumber, qrCodeModel.SMSBody);
            }
            else if (qrCodeModel.QrCodeType == "whatsApp")
            {
                payload = new WhatsAppMessage(qrCodeModel.WhatsAppNumber, qrCodeModel.WhatsAppMessage);
            }
            else if (qrCodeModel.QrCodeType == "email")
            {
                payload = new Mail(qrCodeModel.ReceiverEmailAddress, qrCodeModel.EmailSubject, qrCodeModel.EmailMessage);
            }
            else
            {
                payload = new WiFi(qrCodeModel.WIFIName, qrCodeModel.WIFIPassword, WiFi.Authentication.WPA);
            }

            return payload;
        }

        private byte[] BitmapToByteArr(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
