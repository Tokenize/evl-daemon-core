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
            
            return sum.ToString("X");
        }

        /// <summary>
        /// Verifies that the checksum for the given data is correct.
        /// </summary>
        /// <param name="data">Combined data and checksum value.</param>
        /// <returns>True if checksum is valid, false otherwise.</returns>
        public static bool VerifyChecksum(string data)
        {
            data = data.Trim();
            string dataChecksum = data.Substring(data.Length - 2);

            string cmd = data.Substring(0, data.Length - 2);
            string cmdChecksum = CalculateChecksum(cmd);

            return (dataChecksum == cmdChecksum);
        }
    }
}
