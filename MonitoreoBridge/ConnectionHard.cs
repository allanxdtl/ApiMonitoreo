using Ivi.Visa;
using NationalInstruments.Visa;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitoreoBridge
{
    public class ConnectionHard
    {
        private readonly string _devideUSBAddress;

        public ConnectionHard(string deviceUSBAdress)
        {
            _devideUSBAddress = deviceUSBAdress;
        }

        public async Task<decimal?> ReadResistencia()
        {
            decimal? resistencia = null;
            await Task.Run(() =>
            {
                try
                {
                    ResourceManager rm = new ResourceManager();
                    using (MessageBasedSession session = (MessageBasedSession)rm.Open(_devideUSBAddress))
                    {
                        session.RawIO.Write("*RST");
                        session.RawIO.Write("CONF:RES");
                        session.RawIO.Write("INIT");
                        session.RawIO.Write("READ?");

                        string response = session.RawIO.ReadString();

                        resistencia = decimal.Parse(response);
                    }
                }
                catch (VisaException ex)
                {
                    MessageBox.Show(ex.Message, "Error al obtener el valor de resistencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
            return resistencia;
        }

        public async Task<decimal?> ReadContinuidad()
        {
            decimal? continuidad = null;

            await Task.Run(() =>
            {
                try
                {
                    ResourceManager rm = new ResourceManager();
                    using (MessageBasedSession session = (MessageBasedSession)rm.Open(_devideUSBAddress))
                    {
                        session.RawIO.Write("*RST");
                        session.RawIO.Write("CONF:CONT");
                        session.RawIO.Write("INIT");
                        session.RawIO.Write("READ?");

                        string response = session.RawIO.ReadString().Trim();
                        if (decimal.TryParse(response, out var valor))
                        {
                            continuidad = valor;
                        }
                    }
                }
                catch (VisaException ex)
                {
                    MessageBox.Show(ex.Message, "Error al obtener continuidad",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            return continuidad; // Ohms: si es menor a 50 => hay continuidad
        }

    }
}
