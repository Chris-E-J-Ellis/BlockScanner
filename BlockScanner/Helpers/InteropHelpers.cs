namespace BlockScanner.Helpers
{
    using System.Runtime.InteropServices;

    public static class InteropHelpers
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();
    }
}
