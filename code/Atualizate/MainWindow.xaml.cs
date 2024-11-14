using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace Atualizate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        private static readonly string repoOwner = "AmauryMagno";
        private static readonly string repoName = "CamScan";
        private static readonly string branchName = "Atualizate";

        private static readonly string token = "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";
        public class Content
        {
            public string FileContent { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public string sha { get; set; }
            public long size { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string git_url { get; set; }
            public string download_url { get; set; }
            public string type { get; set; }
            
        }
        public MainWindow()
        {
            InitializeComponent();
            _client = new HttpClient();
            _client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            _baseUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}";
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
            using(HttpClient client = _client)
            {

                var url = $"{_baseUrl}/contents/code/Atualizate/version/version.xml?ref={branchName}";
                var urlfile = $"{_baseUrl}/contents/code/CamScan.zip?ref={branchName}";

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string basePath = System.IO.Path.Combine(currentDirectory, "version");
                string downloadNewPath = System.IO.Path.Combine(basePath, "newversion","newversion.xml");
                string downloadPath = System.IO.Path.Combine(basePath, "version.xml");
                string downloadNewFile = System.IO.Path.Combine(basePath, "CamScan");
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(downloadNewPath));

                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();


                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var fileContentBase64 = JsonConvert.DeserializeObject<dynamic>(jsonResponse).content.ToString().Trim();

                    byte[] fileBytes = Convert.FromBase64String(fileContentBase64);
                    string xmlContent = Encoding.UTF8.GetString(fileBytes);
                    
                    await File.WriteAllBytesAsync(downloadNewPath, fileBytes);

                    MessageBox.Show("Arquivo version.xml baixado com sucesso!", "Download Completo");

                    string numberVersion = await SetNumber(downloadPath);
                    string newNumberVersion = await SetNumber(downloadNewPath);

                    int numberNumber = int.Parse(numberVersion.Replace(".", ""));
                    int newNumberNumber = int.Parse(newNumberVersion.Replace(".", ""));

                    if (newNumberNumber > numberNumber)
                    {
                        //var responsefile = await client.GetAsync(urlfile);
                        //responsefile.EnsureSuccessStatusCode();

                        //var jsonResponseFile = await responsefile.Content.ReadAsStringAsync();
                        //var fileContentBase64File = JsonConvert.DeserializeObject<dynamic>(jsonResponse).content.ToString().Trim();

                        //byte[] zipFileBytes = Convert.FromBase64String(fileContentBase64File);

                        //await File.WriteAllBytesAsync(downloadNewFile, zipFileBytes);

                        DownloadDirectory(urlfile, downloadNewFile);

                        MessageBox.Show("Arquivo CamScan.zip baixado com sucesso!", "Download completo");
                    }

                }
                catch(HttpRequestException err)
                {
                    MessageBox.Show("Erro ao acessar a API de atualização:" + err.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro durante o processamento: " + ex.Message);
                }
            }
        }

        private async Task DownloadDirectory(string apiUrl, string targetDirectory)
        {
            using var response = await _client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var items = System.Text.Json.JsonSerializer.Deserialize<Content[]>(content);

            foreach (var item in items)
            {
                string filePath = Path.Combine(targetDirectory, item.path);

                if(item.type == "dir")
                {
                    Directory.CreateDirectory(filePath);
                    await DownloadDirectory(item.url, filePath);
                }
                else
                {
                    using var fileStream = File.Create(filePath);
                    if(item.download_url != null)
                    {
                        using var downloadResponse = await _client.GetAsync(item.download_url);
                        await downloadResponse.Content.CopyToAsync(fileStream);
                    }
                    else
                    {
                        byte[] fileBytes = Convert.FromBase64String(content);
                        await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                    }
                }
            }


        }


    }
}