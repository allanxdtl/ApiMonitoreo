using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitoreoBridge
{
    public partial class FrmPrincipal : Form
    {
        Api api;
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
                api = JsonSerializer.Deserialize<Api>(json);

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
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            TxtId.Text = string.Empty;
            TxtNombre.Text = string.Empty;
            dtFechaProd.Value = DateTime.Now;
            TxtNoSerie.Text = string.Empty;
        }
    }
}
