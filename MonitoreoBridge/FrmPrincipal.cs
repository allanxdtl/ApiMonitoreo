using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using NationalInstruments.Visa;

namespace MonitoreoBridge
{
    public partial class FrmPrincipal : Form
    {
        Config api;
        HttpClient client;

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            using (StreamReader rd = new StreamReader("config.json"))
            {
                //Leo el archivo json
                string json = rd.ReadToEnd();
                //Deserializamos el archivo en el objeto
                api = JsonSerializer.Deserialize<Config>(json);

                //Instanciamos un HttpCliente con la ruta de la api obtenida del json
                client = new HttpClient()
                {
                    BaseAddress = new Uri(api.api),
                };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        private async void TxtNoSerie_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    var response = await client.GetAsync("/api/ProductoTerminado/GetByNoSerie/" + TxtNoSerie.Text);
                    response.EnsureSuccessStatusCode();

                    var body = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var productos = JsonSerializer.Deserialize<List<ProductoTerminado>>(body, options);
                    var producto = productos?.FirstOrDefault();

                    TxtId.Text = producto.Id.ToString();
                    TxtNombre.Text = producto.Nombre;
                    dtFechaProd.Value = producto.FechaDeProduccion;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ocurrio un error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            TxtId.Text = string.Empty;
            TxtNombre.Text = string.Empty;
            dtFechaProd.Value = DateTime.Now;
            TxtNoSerie.Text = string.Empty;
            TxtNoSerie.Focus();
        }

        private async void BtnResistencia_Click(object sender, EventArgs e)
        {
            ConnectionHard obj = new ConnectionHard(api.usb);

            decimal? value = await obj.ReadResistencia();

            if (!value.HasValue)
                return;

            Prueba prueba = new Prueba()
            {
                NoSerie = TxtNoSerie.Text,
                IdPrueba = 1,
                ValorMedido = value
            };

            string json = JsonSerializer.Serialize(prueba);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/RegistroPruebas/RealizarPrueba", content);

            // Validar la respuesta
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Prueba registrada correctamente.");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Error al registrar la prueba: {response.StatusCode}\n{error}");
            }
        }

        private async void BtnContinuidad_Click(object sender, EventArgs e)
        {
            ConnectionHard obj = new ConnectionHard(api.usb);

            decimal? value = await obj.ReadContinuidad();

            if (!value.HasValue)
                return;

            Prueba prueba = new Prueba()
            {
                NoSerie = TxtNoSerie.Text,
                IdPrueba = 2,
                ValorMedido = value
            };

            string json = JsonSerializer.Serialize(prueba);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/RegistroPruebas/RealizarPrueba", content);

            // Validar la respuesta
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Prueba registrada correctamente.");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Error al registrar la prueba: {response.StatusCode}\n{error}");
            }
        }
    }
}
