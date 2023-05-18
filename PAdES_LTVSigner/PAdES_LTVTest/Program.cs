using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace PAdES_LTVTest
{
    class Program
    {
        static void Main()
        {
            const string originalPdfPath = @"C:\padesPdf\invoice.pdf";  // unsigned pdf file path (an existing pdf file)
            const string finalPdfPath = @"C:\padesPdf\signed.pdf";  // output signed pdf file path

            var certificate = PAdES_LTVSigner.CertificateHelper.ShowCertifikateDialog(StoreName.My);

            ICollection<X509Certificate2> caCertificates = new List<X509Certificate2>();
            PAdES_LTVSigner.CertificateHelper.GetCaCertificates(certificate, ref caCertificates);

            var settings = new PAdES_LTVSigner.SignatureSettings { Location = "Turkey", Reason = "yok"};
            settings.SetTsaClient("http://timestamp.identrust.com/", "", "");

            PAdES_LTVSigner.Signer.Sign(
                originalPdfPath, 
                finalPdfPath, 
                certificate, 
                caCertificates,
                settings
                );

            Console.WriteLine("Document signed: " + finalPdfPath);
            Console.ReadLine();
        }
    }
}
