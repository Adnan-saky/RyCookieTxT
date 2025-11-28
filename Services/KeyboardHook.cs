using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RyCookieText.Helpers;

namespace RyCookieText.Services
{
    /// <summary>
    /// Global low-level keyboard hook for capturing all keyboard events
    /// </summary>
    public class KeyboardHook : IDisposable
    {
        private IntPtr _hookId = IntPtr.Zero;
        private Win32Api.LowLevelKeyboardProc? _hookCallback;

        public event EventHandler<KeyboardEventArgs>? KeyPressed;

        public KeyboardHook()
        {
            _hookCallback = HookCallback;
        }

        /// <summary>
        /// Installs the keyboard hook
        /// </summary>
        public void Install()
        {
            if (_hookId != IntPtr.Zero)
                return;

            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                if (curModule != null && _hookCallback != null)
                {
                    _hookId = Win32Api.SetWindowsHookEx(
                        Win32Api.WH_KEYBOARD_LL,
                        _hookCallback,
                        Win32Api.GetModuleHandle(curModule.ModuleName),
                        0);
                }
            }

            if (_hookId == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to install keyboard hook");
            }
        }

        /// <summary>
        /// Uninstalls the keyboard hook
        /// </summary>
        public void Uninstall()
        {
            if (_hookId != IntPtr.Zero)
            {
                Win32Api.UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)Win32Api.WM_KEYDOWN)
            {
                var hookStruct = Marshal.PtrToStructure<Win32Api.KBDLLHOOKSTRUCT>(lParam);
                var vkCode = (int)hookStruct.vkCode;

                // Raise event
                KeyPressed?.Invoke(this, new KeyboardEventArgs(vkCode));
            }

            return Win32Api.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Uninstall();
        }
    }

    /// <summary>
    /// Event args for keyboard events
    /// </summary>
    public class KeyboardEventArgs : EventArgs
    {
        public int VirtualKeyCode { get; }
        public char Character { get; }
        public bool IsSpecialKey { get; }

        public KeyboardEventArgs(int vkCode)
        {
            VirtualKeyCode = vkCode;
            
            // Check if it's a special key
            IsSpecialKey = vkCode == Win32Api.VK_BACK ||
                          vkCode == Win32Api.VK_RETURN ||
                          vkCode == Win32Api.VK_CONTROL ||
                          vkCode == Win32Api.VK_SHIFT ||
                          vkCode == Win32Api.VK_MENU ||
                          vkCode >= 0x21 && vkCode <= 0x2F || // Navigation keys
                          vkCode >= 0x70 && vkCode <= 0x87;   // Function keys

            // Try to convert to character using ToUnicode (respects keyboard layout and shift state)
            if (!IsSpecialKey)
            {
                try
                {
                    var keyboardState = new byte[256];
                    Win32Api.GetKeyboardState(keyboardState);
                    
                    var buffer = new System.Text.StringBuilder(64);
                    int result = Win32Api.ToUnicode((uint)vkCode, 0, keyboardState, buffer, buffer.Capacity, 0);
                    
                    if (result > 0)
                    {
                        Character = buffer[0];
                    }
                    else
                    {
                        Character = '\0';
                    }
                }
                catch
                {
                    Character = '\0';
                }
            }
        }
    }
}
