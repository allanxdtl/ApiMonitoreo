using Ivi.Visa;
using NationalInstruments.Visa;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitoreoBridge
{
    public class AudioPrueba
    {

        public async Task<decimal?> ReadDecibeles(int duracionSegundos = 5)
        {
            decimal? decibeles = null;

            await Task.Run(() =>
            {
                try
                {
                    // Obtener el dispositivo de salida (audífonos o bocinas predeterminadas)
                    var enumerator = new MMDeviceEnumerator();
                    var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

                    using (var capture = new WasapiLoopbackCapture(device))
                    {
                        float peakValue = 0f;

                        capture.DataAvailable += (s, e) =>
                        {
                            int bytesPerSample = capture.WaveFormat.BitsPerSample / 8;
                            float max = 0;

                            // Recorre el buffer de audio y busca el valor máximo
                            for (int i = 0; i < e.BytesRecorded; i += bytesPerSample)
                            {
                                float sample = BitConverter.ToSingle(e.Buffer, i);
                                sample = Math.Abs(sample);
                                if (sample > max)
                                    max = sample;
                            }

                            if (max > peakValue)
                                peakValue = max;
                        };

                        // Inicia la captura de audio
                        capture.StartRecording();

                        // Espera el tiempo definido
                        Thread.Sleep(duracionSegundos * 1000);

                        // Detiene la captura
                        capture.StopRecording();

                        // Convierte el valor máximo en decibeles (dBFS)
                        double dB = (peakValue > 0) ? 20 * Math.Log10(peakValue) : -100;
                        decibeles = (decimal)dB;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error al medir nivel de audio",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            return decibeles;
        }

    }
}
