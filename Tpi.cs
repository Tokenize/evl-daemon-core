using System.Text;

namespace EvlDaemon
{
    public class Tpi
    {
        /// <summary>
        /// Calculates the checksum of the given command.
        /// </summary>
        /// <param name="command">Command for which to compute checksum.</param>
        /// <returns>Calculated checksum as a hex value string.</returns>
        public static string CalculateChecksum(string command)
        {
            string checksum = string.Empty;
            byte[] chars = Encoding.UTF8.GetBytes(command.ToCharArray());
            int sum = 0;

            foreach (byte c in chars)
            {
                sum += c;
            }

            // Truncate to an 8-bit value
            sum = sum & 255;
            
            return sum.ToString("X2");
        }

        /// <summary>
        /// Verifies that the checksum for the given data packet is correct.
        /// </summary>
        /// <param name="packet">Combined data and checksum value.</param>
        /// <returns>True if checksum is valid, false otherwise.</returns>
        public static bool VerifyChecksum(string packet)
        {
            string checksum = GetChecksum(packet);
            string cmd = GetCommand(packet);
            string cmdChecksum = CalculateChecksum(cmd);

            return (checksum == cmdChecksum);
        }

        /// <summary>
        /// Returns the checksum portion of the given data packet.
        /// </summary>
        /// <param name="packet">Data packet</param>
        /// <returns>Checksum portion of data packet</returns>
        public static string GetChecksum(string packet)
        {
            string trimmed = packet.Trim();
            return trimmed.Substring(trimmed.Length - 2);
        }

        /// <summary>
        /// Returns the command portion of the given data packet.
        /// </summary>
        /// <param name="packet">Data packet</param>
        /// <returns>Command portion of data packet</returns>
        public static string GetCommand(string packet)
        {
            return packet.Trim().Substring(0, 3);
        }

        /// <summary>
        /// Returns the data portion of the given data packet if it exists.
        /// </summary>
        /// <param name="packet">Data packet</param>
        /// <returns>Data portion of the data packet if it exists, and an empty string otherwise</returns>
        public static string GetData(string packet)
        {
            string trimmed = packet.Trim();
            if (trimmed.Length > 5)
            {
                return trimmed.Substring(3, trimmed.Length - 5);
            }

            return string.Empty;
        }
    }
}
