# PAdES LTV Pdf Signer with Timestamp

What we need for signing a Pdf with Timestamp : 
- pfx (PKCS12) file
- an exist Pdf file with random data (You can write a string inside the file)

<br>

## Generating a pfx(PKCS12) FILE for Testing
<br>
*Open Command Line(CMD) and apply following lines:*
<br>
#### Generate an RSA private key:

```ruby
C:\Openssl\bin\openssl.exe genrsa -out my_key.key 2048.
```

#### Generate a Certificate Signing Request:

```ruby
C:\Openssl\bin\openssl.exe req -new -key my_key.key -out my_request.csr
```

#### Generate a self-signed public certificate based on the request:

```ruby
C:\Openssl\bin\openssl.exe x509 -req -days 3650 -in my_request.csr -signkey my_key.key -out my_cert.crt
```

#### Generate a PKCS12 file:

```ruby
C:\Openssl\bin\openssl.exe pkcs12 -keypbe PBE-SHA1-3DES -certpbe PBE-SHA1-3DES -export -in my_cert.crt -inkey my_key.key -out my_pkcs12.pfx -name "my-name"
```

