using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace PAdES_LTVSigner
{
    public class CertificateHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeName"></param>
        /// <returns></returns>
        static public X509Certificate2 ShowCertifikateDialog(StoreName storeName)
        {
            var store = new X509Store(storeName, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            var collection = store.Certificates;
            var fcollection = collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            var scollection = X509Certificate2UI.SelectFromCollection(fcollection, "Select certificate", "List of certificates", X509SelectionFlag.SingleSelection);

            store.Close();

            if (scollection.Count == 0)
            {
                throw new Exception("A suggested certificate to use for this example " +
                    "is not in the certificate store. Select " +
                    "an alternate certificate to use for " +
                    "signing the message.");
            }
            return scollection[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="caCertificates"></param>
        /// <returns></returns>
        public static void GetCaCertificates(X509Certificate2 certificate, ref ICollection<X509Certificate2> caCertificates)
        {
            caCertificates = caCertificates ?? new List<X509Certificate2>();

            var issuer = FindCertificateAcrossCertStore(X509FindType.FindBySubjectDistinguishedName, certificate.Issuer);
            caCertificates.Add(issuer);

            if(!String.Equals(issuer.Subject, issuer.Issuer))
                GetCaCertificates(issuer, ref caCertificates);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="findBy"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static X509Certificate2 FindCertificateAcrossCertStore(X509FindType findBy, string value)
        {
            var certColl = new X509Certificate2Collection();
            foreach (var e in (StoreName[])Enum.GetValues(typeof(StoreName)))
            {
                var store = new X509Store(e, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);

                certColl.AddRange(store.Certificates.Find(findBy, value, false));

                store.Close();

                if (certColl.Count != 0)
                {
                    return certColl[0];
                }
            }

            throw new Exception("Certificate not found: " + value);
        }
    }
}
