using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace Atualizate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string repoOwner = "AmauryMagno";
        private static readonly string repoName = "CamScan";
        private static readonly string branchName = "Develop";

        private static readonly string token = "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";
        public MainWindow()
        {
            InitializeComponent();
            Loaded += GetRemoteVersionAsync;
        }

        private async Task<string> SetNumber(string path)
        {
            try
            {
                var reader = new StreamReader(path);
                string xml = await reader.ReadToEndAsync();

                XDocument xmlReader = XDocument.Parse(xml);

                var numberElement = xmlReader.Root?.Element("Number");
                if (numberElement != null)
                {
                    return numberElement.Value;
                }
                else
                {
                    MessageBox.Show("Elemento <number> não encontrado no XML.");
                    return null;
                }
            }
            catch( Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
                
        }
        private async void GetRemoteVersionAsync(object sender, RoutedEventArgs e)
        {
            using(HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/code/Atualizate/version/version.xml?ref={branchName}";

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string downloadNewPath = System.IO.Path.Combine(currentDirectory, "version", "newversion","newversion.xml");
                string downloadPath = System.IO.Path.Combine(currentDirectory, "version", "version.xml");
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(downloadNewPath));

                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();


                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var fileContent = JsonConvert.DeserializeObject<dynamic>(jsonResponse).content.ToString().Trim();

                    byte[] fileBytes = Convert.FromBase64String(fileContent);
                    string xmlContent = Encoding.UTF8.GetString(fileBytes);
                    
                    await File.WriteAllBytesAsync(downloadNewPath, fileBytes);

                    MessageBox.Show("Arquivo version.xml baixado com sucesso!", "Download Completo");

                    string numberVersion = await SetNumber(downloadPath);
                    string newNumberVersion = await SetNumber(downloadNewPath);

                    int numberNumber = int.Parse(numberVersion.Replace(".", ""));
                    int newNumberNumber = int.Parse(newNumberVersion.Replace(".", ""));

                    if (newNumberNumber > numberNumber)
                    {
                        MessageBox.Show($"Nova versão {newNumberVersion} : {newNumberNumber}, versão antiga {numberVersion} : {numberNumber}");
                    }

                }
                catch(HttpRequestException err)
                {
                    MessageBox.Show("Erro ao acessar a API de atualização:" + err.Message);
                }
            }
        }
    }
}