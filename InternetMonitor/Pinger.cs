using System.Net.NetworkInformation;

namespace InternetMonitor
{
    public class Pinger : IPinger
    {
        private readonly string _hostIp;
        private readonly Ping _ping;

        public Pinger(string hostIp)
        {
            _hostIp = hostIp;
            _ping = new Ping();
        }

        public bool Ping()
        {
            try
            {
                var reply = _ping.Send(_hostIp);
                return reply?.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}
