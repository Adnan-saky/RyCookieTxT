using System;
using System.Text;
using RyCookieText.Helpers;

namespace RyCookieText.Services
{
    /// <summary>
    /// Manages a rolling buffer of recently typed characters
    /// </summary>
    public class BufferManager
    {
        private readonly StringBuilder _buffer;
        private readonly int _maxSize;

        public BufferManager(int maxSize = 50)
        {
            _maxSize = maxSize;
            _buffer = new StringBuilder(maxSize);
        }

        /// <summary>
        /// Gets the current buffer content
        /// </summary>
        public string GetBuffer()
        {
            return _buffer.ToString();
        }

        /// <summary>
        /// Adds a character to the buffer
        /// </summary>
        public void AddCharacter(char c)
        {
            _buffer.Append(c);

            // Trim if exceeds max size
            if (_buffer.Length > _maxSize)
            {
                _buffer.Remove(0, _buffer.Length - _maxSize);
            }
        }

        /// <summary>
        /// Handles a keyboard event and updates the buffer
        /// </summary>
        public void ProcessKeyEvent(KeyboardEventArgs e)
        {
            // Handle backspace
            if (e.VirtualKeyCode == Win32Api.VK_BACK)
            {
                if (_buffer.Length > 0)
                {
                    _buffer.Remove(_buffer.Length - 1, 1);
                }
                return;
            }

            // Handle Enter (newline)
            if (e.VirtualKeyCode == Win32Api.VK_RETURN)
            {
                AddCharacter('\n');
                return;
            }

            // Clear buffer on Ctrl, Alt, or navigation keys
            if (e.VirtualKeyCode == Win32Api.VK_CONTROL ||
                e.VirtualKeyCode == Win32Api.VK_MENU ||
                (e.VirtualKeyCode >= 0x21 && e.VirtualKeyCode <= 0x28)) // Arrow keys, Home, End, etc.
            {
                Clear();
                return;
            }

            // Add regular character
            if (e.Character != '\0')
            {
                AddCharacter(e.Character);
            }
        }

        /// <summary>
        /// Removes the last N characters from the buffer
        /// </summary>
        public void RemoveLastCharacters(int count)
        {
            if (count > 0 && count <= _buffer.Length)
            {
                _buffer.Remove(_buffer.Length - count, count);
            }
        }

        /// <summary>
        /// Clears the buffer
        /// </summary>
        public void Clear()
        {
            _buffer.Clear();
        }

        /// <summary>
        /// Gets the last N characters from the buffer
        /// </summary>
        public string GetLastCharacters(int count)
        {
            if (count <= 0 || _buffer.Length == 0)
                return string.Empty;

            int startIndex = Math.Max(0, _buffer.Length - count);
            int length = Math.Min(count, _buffer.Length);
            return _buffer.ToString(startIndex, length);
        }
    }
}
