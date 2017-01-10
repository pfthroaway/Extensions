using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Extensions
{
    /// <summary>Key types available for standard TextBox/PasswordBox manipulation.</summary>
    public enum KeyType { Decimals, Integers, Letters, NegativeDecimals, NegativeIntegers }

    public class Functions
    {
        /// <summary>Turns several Keyboard.Keys into a list of Keys which can be tested using List.Any.</summary>
        /// <param name="keys">Array of Keys</param>
        /// <returns>Returns list of Keys' IsKeyDown state</returns>
        private static IEnumerable<bool> GetListOfKeys(params Key[] keys)
        {
            return keys.Select(Keyboard.IsKeyDown).ToList();
        }

        /// <summary>Selects all text in passed TextBox.</summary>
        /// <param name="sender">Object to be cast</param>
        public static void TextBoxGotFocus(object sender)
        {
            TextBox txt = (TextBox)sender;
            txt.SelectAll();
        }

        /// <summary>Selects all text in passed PasswordBox.</summary>
        /// <param name="sender">Object to be cast</param>
        public static void PasswordBoxGotFocus(object sender)
        {
            PasswordBox txt = (PasswordBox)sender;
            txt.SelectAll();
        }

        /// <summary>Deletes all text in textbox which isn't a letter.</summary>
        /// <param name="sender">Object to be cast</param>
        /// <param name="keyType">Type of input allowed</param>
        public static void TextBoxTextChanged(object sender, KeyType keyType)
        {
            TextBox txt = (TextBox)sender;
            switch (keyType)
            {
                case KeyType.Decimals:
                    txt.Text = new string((from c in txt.Text
                                           where char.IsDigit(c) || c.IsPeriod()
                                           select c).ToArray());

                    if (txt.Text.Substring(txt.Text.IndexOf(".", StringComparison.Ordinal) + 1).Contains("."))
                        txt.Text = txt.Text.Substring(0, txt.Text.IndexOf(".", StringComparison.Ordinal) + 1) + txt.Text.Substring(txt.Text.IndexOf(".", StringComparison.Ordinal) + 1).Replace(".", "");
                    break;

                case KeyType.Integers:
                    txt.Text = new string((from c in txt.Text
                                           where char.IsDigit(c)
                                           select c).ToArray());
                    break;

                case KeyType.Letters:
                    txt.Text = new string((from c in txt.Text
                                           where char.IsLetter(c)
                                           select c).ToArray());
                    break;

                case KeyType.NegativeDecimals:
                    txt.Text = new string((from c in txt.Text
                                           where char.IsDigit(c) || c.IsPeriod() || c.IsHyphen()
                                           select c).ToArray());

                    if (txt.Text.Substring(txt.Text.IndexOf(".", StringComparison.Ordinal) + 1).Contains("."))
                        txt.Text = txt.Text.Substring(0, txt.Text.IndexOf(".", StringComparison.Ordinal) + 1) + txt.Text.Substring(txt.Text.IndexOf(".", StringComparison.Ordinal) + 1).Replace(".", "");

                    if (txt.Text.Substring(txt.Text.IndexOf("-", StringComparison.Ordinal) + 1).Contains("-"))
                        txt.Text = txt.Text.Substring(0, txt.Text.IndexOf("-", StringComparison.Ordinal) + 1) + txt.Text.Substring(txt.Text.IndexOf("-", StringComparison.Ordinal) + 1).Replace("-", "");
                    break;

                case KeyType.NegativeIntegers:
                    txt.Text = new string((from c in txt.Text
                                           where char.IsDigit(c) || c.IsHyphen()
                                           select c).ToArray());

                    if (txt.Text.Substring(txt.Text.IndexOf("-", StringComparison.Ordinal) + 1).Contains("-"))
                        txt.Text = txt.Text.Substring(0, txt.Text.IndexOf("-", StringComparison.Ordinal) + 1) + txt.Text.Substring(txt.Text.IndexOf("-", StringComparison.Ordinal) + 1).Replace("-", "");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null);
            }
            txt.CaretIndex = txt.Text.Length;
        }

        /// <summary>Previews a pressed key and determines whether or not it is acceptable input.</summary>
        /// <param name="e">Key being pressed</param>
        /// <param name="keyType">Type of input allowed</param>
        public static void PreviewKeyDown(KeyEventArgs e, KeyType keyType)
        {
            Key k = e.Key;

            IEnumerable<bool> keys = GetListOfKeys(Key.Back, Key.Delete, Key.Home, Key.End, Key.LeftShift,
                Key.RightShift, Key.Enter, Key.Tab, Key.LeftAlt, Key.RightAlt, Key.Left, Key.Right, Key.LeftCtrl,
                Key.RightCtrl, Key.Escape);

            switch (keyType)
            {
                case KeyType.Decimals:
                    e.Handled = !keys.Any(key => key) && (Key.D0 > k || k > Key.D9) &&
                                (Key.NumPad0 > k || k > Key.NumPad9) && k != Key.Decimal && k != Key.OemPeriod;
                    break;

                case KeyType.Integers:
                    e.Handled = !keys.Any(key => key) && (Key.D0 > k || k > Key.D9) &&
                                (Key.NumPad0 > k || k > Key.NumPad9);
                    break;

                case KeyType.Letters:
                    e.Handled = !keys.Any(key => key) && (Key.A > k || k > Key.Z);
                    break;

                case KeyType.NegativeDecimals:
                    e.Handled = !keys.Any(key => key) && (Key.D0 > k || k > Key.D9) &&
                                (Key.NumPad0 > k || k > Key.NumPad9) && k != Key.Decimal && k != Key.Subtract && k != Key.OemPeriod && k != Key.OemMinus;
                    break;

                case KeyType.NegativeIntegers:
                    e.Handled = !keys.Any(key => key) && (Key.D0 > k || k > Key.D9) &&
                                (Key.NumPad0 > k || k > Key.NumPad9) && k != Key.Subtract && k != Key.OemMinus;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null);
                    //&& !(Key.D0 <= k && k <= Key.D9) && !(Key.NumPad0 <= k && k <= Key.NumPad9))
                    //|| k == Key.OemMinus || k == Key.Subtract || k == Key.Decimal || k == Key.OemPeriod)
                    //System.Media.SystemSound ss = System.Media.SystemSounds.Beep;
                    //ss.Play();
            }
        }

        #region Random Number Generation

        /// <summary>Generates a random number between min and max (inclusive).</summary>
        /// <param name="min">Inclusive minimum number</param>
        /// <param name="max">Inclusive maximum number</param>
        /// <param name="lowerLimit">Minimum limit for the method, regardless of min and max.</param>
        /// <param name="upperLimit">Maximum limit for the method, regardless of min and max.</param>
        /// <returns>Returns randomly generated integer between min and max with an upper limit of upperLimit.</returns>
        public static int GenerateRandomNumber(int min, int max, int lowerLimit = int.MinValue,
            int upperLimit = int.MaxValue)
        {
            int result = min < max
                ? ThreadSafeRandom.ThisThreadsRandom.Next(min, max + 1)
                : ThreadSafeRandom.ThisThreadsRandom.Next(max, min + 1);

            return result < lowerLimit ? lowerLimit : (result > upperLimit ? upperLimit : result);
        }

        #endregion Random Number Generation
    }
}