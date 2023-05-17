using iTextSharp.text.pdf.security;

namespace PAdES_LTVSigner
{
    public class SignatureSettings
    {
        public string Reason { get; set; }

        public string Location { get; set; }

        public IOcspClient OcspClient { get; set; }

        public ITSAClient TsaClient { get; set; }

        public SignatureSettings()
        {
            OcspClient = new OcspClientBouncyCastle();
        }

        public void SetTsaClient(string url, string user, string password)
        {
            TsaClient = new TSAClientBouncyCastle(url, user, password);
        }
    }
}
