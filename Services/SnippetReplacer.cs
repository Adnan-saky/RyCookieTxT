using System;
using System.Threading;
using System.Threading.Tasks;
using RyCookieText.Helpers;

namespace RyCookieText.Services
{
    /// <summary>
    /// Replaces matched keywords with snippet content using SendInput API
    /// </summary>
    public class SnippetReplacer
    {
        private readonly int _keystrokeDelayMs;

        public SnippetReplacer(int keystrokeDelayMs = 5)
        {
            _keystrokeDelayMs = keystrokeDelayMs;
        }

        /// <summary>
        /// Replaces the keyword with snippet content
        /// </summary>
        public async Task ReplaceAsync(MatchResult match)
        {
            await Task.Run(() =>
            {
                // Delete the keyword (including trigger char)
                DeleteCharacters(match.KeywordLength + 1);

                // Type the replacement text
                TypeText(match.Snippet.Content);

                // Re-type the trigger character
                TypeCharacter(match.TriggerChar);
            });
        }

        /// <summary>
        /// Deletes N characters by sending backspace keys
        /// </summary>
        private void DeleteCharacters(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SendKey(Win32Api.VK_BACK);
                Thread.Sleep(_keystrokeDelayMs);
            }
        }

        /// <summary>
        /// Types text character by character
        /// </summary>
        private void TypeText(string text)
        {
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    SendKey(Win32Api.VK_RETURN);
                }
                else
                {
                    TypeCharacter(c);
                }
                Thread.Sleep(_keystrokeDelayMs);
            }
        }

        /// <summary>
        /// Types a single character using Unicode input
        /// </summary>
        private void TypeCharacter(char c)
        {
            var inputs = new Win32Api.INPUT[2];

            // Key down
            inputs[0] = new Win32Api.INPUT
            {
                type = Win32Api.INPUT_KEYBOARD,
                u = new Win32Api.InputUnion
                {
                    ki = new Win32Api.KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = c,
                        dwFlags = Win32Api.KEYEVENTF_UNICODE,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            // Key up
            inputs[1] = new Win32Api.INPUT
            {
                type = Win32Api.INPUT_KEYBOARD,
                u = new Win32Api.InputUnion
                {
                    ki = new Win32Api.KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = c,
                        dwFlags = Win32Api.KEYEVENTF_UNICODE | Win32Api.KEYEVENTF_KEYUP,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            Win32Api.SendInput(2, inputs, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32Api.INPUT)));
        }

        /// <summary>
        /// Sends a virtual key press
        /// </summary>
        private void SendKey(int vkCode)
        {
            var inputs = new Win32Api.INPUT[2];

            // Key down
            inputs[0] = new Win32Api.INPUT
            {
                type = Win32Api.INPUT_KEYBOARD,
                u = new Win32Api.InputUnion
                {
                    ki = new Win32Api.KEYBDINPUT
                    {
                        wVk = (ushort)vkCode,
                        wScan = 0,
                        dwFlags = 0,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            // Key up
            inputs[1] = new Win32Api.INPUT
            {
                type = Win32Api.INPUT_KEYBOARD,
                u = new Win32Api.InputUnion
                {
                    ki = new Win32Api.KEYBDINPUT
                    {
                        wVk = (ushort)vkCode,
                        wScan = 0,
                        dwFlags = Win32Api.KEYEVENTF_KEYUP,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };

            Win32Api.SendInput(2, inputs, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32Api.INPUT)));
        }
    }
}
