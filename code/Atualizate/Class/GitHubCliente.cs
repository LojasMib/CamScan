using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using static Atualizate.MainWindow;

namespace Atualizate.Class
{
    
    class GitHubCliente
    {
        private HttpClient _client;
        private string _baseUrl;
        public Version _versionUpdate;
        public Version _versionLocal;


        private static readonly string repoOwner = "AmauryMagno";
        private static readonly string repoName = "CamScan";
        private static readonly string branchName = "Atualizate";
        private static readonly string token = "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";

        private string _urlJsonConfig;
        private string _urlFileSystem;

        public class Version
        {
            public string GitBranch { get; private set; }
            public string Number { get; private set; }
            public string NameExecutable { get; private set; }
            public string PathExecutable { get; private set; }
            public string NameFileZip { get; private set; }
            public string Outdate { get; private set; }
        }

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


        public void InitializeHttpClient()
        {
            try
            {
                _client = new HttpClient();
                _client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                _baseUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}";
                _urlJsonConfig = $"{ _baseUrl}/contents/code/Atualizate/version/version.json?ref={ branchName}";
                _urlFileSystem = $"{_baseUrl}/contents/code?ref={branchName}";
            }
            catch (Exception err)
            {
                MessageBox.Show($"Erro na conexão ao repositório do GitHub: {err}");
            }
        }

        private Version? DeserializerJson(string fileJson)
        {
            return JsonSerializer.Deserialize<Version>(fileJson);
        }

        private string SerializerJson(Version version)
        {
            string jsonString = JsonSerializer.Serialize(version, new JsonSerializerOptions { WriteIndented = true });
            return jsonString;
        }

        private async Task<string>? GitConnection(string url)
        {
            using(HttpClient client = _client)
            {
                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch(Exception err)
                {
                    MessageBox.Show($"Conexão com GitHub não estabelecida: {err}", "Erro de Conexão");
                    return null;
                }
            }
        }

        public async Task<string> DownloadJsonConfig(string downloadPath)
        {
            try
            {
                var response = await GitConnection(_urlJsonConfig);
                if (!string.IsNullOrEmpty(response))
                {
                    _versionUpdate = DeserializerJson(response);
                    string responseSerializer = SerializerJson(_versionUpdate);
                    string directoryPath = Path.GetDirectoryName(downloadPath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    File.WriteAllText(downloadPath, responseSerializer);
                    MessageBox.Show("Arquivo version.xml baixado com sucesso!", "Download Completo");
                    return _versionUpdate.Number;
                }
                else
                {
                    MessageBox.Show("Erro: resposta da conexão é nula ou vazia.", "Erro");
                    return "Erro";
                }
            }
            catch(Exception err)
            {
                MessageBox.Show($"Erro no Download: {err.Message}");
                return "Erro";
            }
        }

        public string ReaderFileJson(string path)
        {
            try
            {
                _versionLocal = DeserializerJson(path);
                return _versionLocal.Number;
            }catch(Exception err)
            {
                MessageBox.Show($"Erro na leitura do arquivo local: {err.Message}");
                return "Erro";
            }
            
        }

        public async void DownloadFileZip(string downloadPath)
        {
            try
            {
                var response = await GitConnection(_urlFileSystem);

                var items = System.Text.Json.JsonSerializer.Deserialize<Content[]>(response);

                foreach (var item in items)
                {
                    if (item.name == _versionUpdate.NameFileZip)
                    {
                        var responseFile = await _client.GetAsync(item.download_url);
                        responseFile.EnsureSuccessStatusCode();

                        var stream = await responseFile.Content.ReadAsStreamAsync();
                        using (var fileStream = new FileStream(downloadPath, FileMode.Create))
                        {
                            await stream.CopyToAsync(fileStream);
                        }

                    }
                }
            
            }catch(Exception err)
            {
                MessageBox.Show($"Erro no download do arquivo ZIP: {err.Message}");
            }
        }
    }
}
