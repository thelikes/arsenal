use Win32::API;

# rundll32.pl - load dll with perl
# @thelikes_
# help: https://stackoverflow.com/questions/45059300/win32api-giving-wrong-prototype-error
# sample dll: https://github.com/thelikes/arsenal/blob/main/c/exec_dll-2.c

$function = Win32::API->new("c:\\payloads\\exec_dll\\output\\exec_dll64.dll", "DllMain",[ 'N', 'P', 'P' ], 'N', '__cdecl');

exit(0);