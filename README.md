# PAdES LTV Pdf Signer with Timestamp & SHA-256

What we need for signing a Pdf with Timestamp?
- pfx (PKCS12) file
- an exist Pdf file with random data (You can write a string inside the file like "Hello World") -- sha256.pdf

<br>

## Generating a pfx(PKCS12) FILE for Testing


### *Open Command Line(CMD) and apply following lines:*

*Install OpenSSL --> [Download OpenSSL](https://link-url-here.org](https://slproweb.com/products/Win32OpenSSL.html)*

*Go OpenSSL Directory with 'cd' Command :*

```bash
cd C:\Openssl\bin
```

#### Generate an RSA private key:

```bash
C:\Openssl\bin\openssl.exe genrsa -out my_key.key 2048
```

#### Generate a Certificate Signing Request:

```bash
C:\Openssl\bin\openssl.exe req -new -key my_key.key -out my_request.csr
```

#### Generate a self-signed public certificate based on the request:

```bash
C:\Openssl\bin\openssl.exe x509 -req -days 3650 -in my_request.csr -signkey my_key.key -out my_cert.crt
```

#### Generate a PKCS12 file:

```bash
C:\Openssl\bin\openssl.exe pkcs12 -keypbe PBE-SHA1-3DES -certpbe PBE-SHA1-3DES -export -in my_cert.crt -inkey my_key.key -out my_pkcs12.pfx -name "my-name"
```



## AFTER CERTIFICATE CREATED -->
*Change Files Directories and add TSA Client In Program.cs*
```cs
const string originalPdfPath = @"C:\padestest\sha256.pdf";  // unsigned pdf file path (an existing pdf file)
const string finalPdfPath = @"C:\padestest\signed.pdf";  // output signed pdf file path

settings.SetTsaClient("http://timestamp.identrust.com/", "", "");
```

### RUN - FINISH
#### OUTPUT :
![image](https://github.com/githuseyingur/PAdES_pdf_LTVsigner_with_timestamp/assets/120099096/902a0bfb-9a89-44c0-bd59-53cd4460cadf)

![Ekran görüntüsü 2023-05-17 162726](https://github.com/githuseyingur/PAdES_pdf_LTVsigner_with_timestamp/assets/120099096/d824b323-d3ca-418f-bf85-e74100282e8c)

if you add this lines : 
```cs
 appearance.SetVisibleSignature(new Rectangle(36, 748, 144, 780), 1, "sig");
```
*this will be the output : *


![image](https://github.com/githuseyingur/PAdES_pdf_LTVsigner_with_timestamp/assets/120099096/110b4c07-d9d4-436f-ba83-735b7b6ffde9)


