[DllImport("kernel32.dll", SetLastError = true)]
static extern IntPtr FlsAlloc(IntPtr callback);
static void Main(string[] args)
{
    IntPtr mem = FlsAlloc(IntPtr.Zero);
    if (mem == null)
    {
        return;
    }
    [...]