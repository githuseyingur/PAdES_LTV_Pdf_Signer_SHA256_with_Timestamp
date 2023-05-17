# PAdES_LTVSigner

What we need for signing a Pdf with Timestamp : 
- pfx (PKCS12) file, 
- an exist Pdf file with random data (You can write a string inside the file)




### Generate a PKCS12 FILE =

Generate an RSA private key:
C:\Openssl\bin\openssl.exe genrsa -out

Where:

is the desired filename for the private key file

is the desired key length of either 1024, 2048, or 4096

For example, type:

C:\Openssl\bin\openssl.exe genrsa -out my_key.key 2048.

Generate a Certificate Signing Request:
In version 0.9.8h and later:

C:\Openssl\bin\openssl.exe req -new -key -out -config C:\Openssl\bin\openssl.cfg

Where:

is the input filename of the previously generated private key

is the output filename of the certificate signing request

For example, type:

C:\Openssl\bin\openssl.exe req -new -key my_key.key -out my_request.csr

Follow the on-screen prompts for the required certificate request information.

Generate a self-signed public certificate based on the request:

C:\Openssl\bin\openssl.exe x509 -req -days 3650 -in -signkey -out

Where:

is the input filename of the certificate signing request

is the input filename of the previously generated private key

is the output filename of the public certificate

For example, type:

C:\Openssl\bin\openssl.exe x509 -req -days 3650 -in my_request.csr -signkey my_key.key -out my_cert.crt

Generate a PKCS#12 file:
C:\Openssl\bin\openssl.exe pkcs12 -keypbe PBE-SHA1-3DES -certpbe PBE-SHA1-3DES -export -in -inkey -out <PKCS#12 Filename> -name ""

Where:

is the input filename of the public certificate, in PEM format

is the input filename of the private key

<PKCS#12 Filename> is the output filename of the pkcs#12 format file

is the desired name that will sometimes be displayed in user interfaces.

For example, type:

C:\Openssl\bin\openssl.exe pkcs12 -keypbe PBE-SHA1-3DES -certpbe PBE-SHA1-3DES -export -in my_cert.crt -inkey my_key.key -out my_pkcs12.pfx -name "my-name"
