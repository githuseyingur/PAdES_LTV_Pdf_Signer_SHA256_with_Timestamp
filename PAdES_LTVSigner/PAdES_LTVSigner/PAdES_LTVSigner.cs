using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using iTextSharp.text;
using Org.BouncyCastle.Pkcs;

namespace PAdES_LTVSigner
{
    public static class Signer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <param name="cert"></param>
        /// <param name="caCerts"></param>
        /// <param name="signatureSettings"></param>
        public static void Sign(string inputPath, string outputPath, X509Certificate2 cert, ICollection<X509Certificate2> caCerts, SignatureSettings signatureSettings)
        {
            
            var cp = new Org.BouncyCastle.X509.X509CertificateParser();
            var chain = new List<Org.BouncyCastle.X509.X509Certificate> { cp.ReadCertificate(cert.RawData)};
            chain.AddRange(caCerts.Select(caCert => cp.ReadCertificate(caCert.RawData)));

            FileStream privateKeyStream = new FileStream("C:\\padesPdf\\my_pkcs12.pfx", System.IO.FileMode.Open);
            Pkcs12Store pk12 = new Pkcs12Store(privateKeyStream, "942712".ToCharArray());
            privateKeyStream.Dispose();

            string alias = null;
            foreach (string tAlias in pk12.Aliases)
            {
                if (pk12.IsKeyEntry(tAlias))
                {
                    alias = tAlias;
                    break;
                }
            }
            var pkc = pk12.GetKey(alias).Key;

            IExternalSignature pks = new PrivateKeySignature(pkc, "SHA-256"); 
          // IExternalSignature pk = new X509Certificate2Signature(cert, DigestAlgorithms.SHA256); 
            
            IList<ICrlClient> crlList = new List<ICrlClient>();
            crlList.Add(new CrlClientOnline());

            var signed = SignDocument(inputPath, chain, pks, CryptoStandard.CMS, signatureSettings.Reason,
                     signatureSettings.Location, crlList, null,  signatureSettings.TsaClient, 0);

            File.WriteAllBytes(outputPath, signed);
            AddLtv(signed, outputPath, signatureSettings.OcspClient, crlList.First());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="chain"></param>
        /// <param name="pks"></param>
        /// <param name="subfilter"></param>
        /// <param name="reason"></param>
        /// <param name="location"></param>
        /// <param name="crlList"></param>
        /// <param name="ocspClient"></param>
        /// <param name="tsaClient"></param>
        /// <param name="estimatedSize"></param>
        /// <returns></returns>
        private static byte[] SignDocument(String input,
                         ICollection<Org.BouncyCastle.X509.X509Certificate> chain,
                         IExternalSignature pks,
                         CryptoStandard subfilter,
                         String reason, String location,
                         ICollection<ICrlClient> crlList,
                         IOcspClient ocspClient,
                         ITSAClient tsaClient,
                         int estimatedSize
            )
        {
            using (var stream = new MemoryStream())
            {
                // Creating the reader and the stamper
                PdfReader reader = null;
                PdfStamper stamper = null;
                try
                {
                    reader = new PdfReader(input);
                    
                    stamper = PdfStamper.CreateSignature(reader, stream, '\0');

                    // Creating the appearance
                    PdfSignatureAppearance appearance = stamper.SignatureAppearance;
                    appearance.Reason = reason;
                    appearance.Location = location;
                    appearance.SetVisibleSignature(new Rectangle(36, 748, 144, 780), 1, "sig"); // CLOSE

                    // Creating the signature
                    MakeSignature.SignDetached(
                        appearance,
                        pks,
                        chain, 
                        crlList, 
                        ocspClient, 
                        tsaClient,  // TSA URL ID PASS
                        estimatedSize, 
                        subfilter
                        );
                }
                finally
                {
                    reader?.Close();
                    stamper?.Close();
                }

                return stream.GetBuffer();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="ocsp"></param>
        /// <param name="crl"></param>
        private static void AddLtv(byte[] src, String dest, IOcspClient ocsp, ICrlClient crl)
        {
            var r = new PdfReader(src);
            var fos = new FileStream(dest, FileMode.Create);
            var stp = new PdfStamper(r, fos, '\0', true);
            LtvVerification v = stp.LtvVerification;
            AcroFields fields = stp.AcroFields;
            List<String> names = fields.GetSignatureNames();
            String sigName = names[names.Count - 1];
            PdfPKCS7 pkcs7 = fields.VerifySignature(sigName);
            if (pkcs7.IsTsp)
                v.AddVerification(sigName, ocsp, crl, LtvVerification.CertificateOption.SIGNING_CERTIFICATE, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.NO);
            else
                foreach (String name in names)
                    v.AddVerification(name, ocsp, crl, LtvVerification.CertificateOption.WHOLE_CHAIN, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.NO);

            stp.Close();
        }
    }
}
