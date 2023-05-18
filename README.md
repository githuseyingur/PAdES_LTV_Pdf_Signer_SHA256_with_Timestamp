# PAdES LTV Pdf Signer with Timestamp & SHA-256

What we need for signing a Pdf with Timestamp?
- pfx (PKCS12) file
- an exist Pdf file with random data (You can write a string inside the file like "Hello World") -- sha256.pdf

<br>

## Generating a pfx(PKCS12) FILE for Testing


### *Open Command Line(CMD) and apply following lines:*

*Install OpenSSL --> [Download OpenSSL](https://slproweb.com/products/Win32OpenSSL.html)*

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

![Ekran görüntüsü 2023-05-17 162332](https://github.com/githuseyingur/PAdES_pdf_LTV_signer_SHA256_with_ts/assets/120099096/a7e3eda9-f624-41b6-b5b0-c666b05406d4)

![Ekran görüntüsü 2023-05-17 162726](https://github.com/githuseyingur/PAdES_pdf_LTV_signer_SHA256_with_ts/assets/120099096/adab001f-4e15-4b44-a232-7715b3dd1dcf)

Add this line if you want see the signature in your pdf file: 
```cs
 appearance.SetVisibleSignature(new Rectangle(36, 748, 144, 780), 1, "sig");
```
*this will be the output :*

![sign](https://github.com/githuseyingur/PAdES_pdf_LTV_signer_SHA256_with_ts/assets/120099096/584ce07d-edef-4be6-8d37-d2862c4c546d)


