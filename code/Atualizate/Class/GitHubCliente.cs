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
using Atualizate.Class;
using System.Windows;
using static Atualizate.MainWindow;


namespace Atualizate.Class
{
    
    class GitHubCliente
    {
        private HttpClient _client;
        private string _baseUrl;
        private string _urlJsonConfig;
        private string _urlFileSystem;

        public Models.Version _versionUpdate;
        public Models.Version _versionLocal;
        private Serialize _serialize;

        private static readonly string repoOwner = "AmauryMagno";
        private static readonly string repoName = "CamScan";
        private static readonly string branchName = "Atualizate";
        private static readonly string token = "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";

        public void InitializeHttpClient()
        {
            try
            {
                _client = new HttpClient();
                _client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                _baseUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}";
                _urlJsonConfig = $"{_baseUrl}/contents/code/Atualizate/version/version.json?ref={branchName}";
                _urlFileSystem = $"{_baseUrl}/contents/code?ref={branchName}";
            }
            catch (Exception err)
            {
                MessageBox.Show($"Erro na conexão ao repositório do GitHub: {err}");
            }
        }

        private async Task<string>? GitConnection(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch(Exception err)
            {
                MessageBox.Show($"Conexão com GitHub não estabelecida: {err}", "Erro de Conexão");
                return null;
            }
        }

        public async Task<string> DownloadJsonConfig(string downloadPath)
        {
            try
            {
                var response = await GitConnection(_urlJsonConfig);
                var options = new JsonSerializerOptions
                {
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };
                var deserializedResponse = _serialize.DeserializerJson<Models.Content>(response);
                var contetnBase64 = deserializedResponse.content;
                if (!string.IsNullOrEmpty(response))
                {
                    byte[] jsonBytes = Convert.FromBase64String(contetnBase64);
                    string jsonString = Encoding.UTF8.GetString(jsonBytes);
                    _versionUpdate =_serialize.DeserializerJson<Models.Version>(jsonString);

                    string directoryPath = Path.GetDirectoryName(downloadPath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    File.WriteAllText(downloadPath, JsonSerializer.Serialize(_versionUpdate, new JsonSerializerOptions { WriteIndented = true }));
                    MessageBox.Show("Arquivo version.json baixado com sucesso!", "Download Completo");
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
                string jsonContent = File.ReadAllText(path);
                _versionLocal = _serialize.DeserializerJson<Models.Version>(jsonContent);
                return _versionLocal.Number;
            }catch(Exception err)
            {
                MessageBox.Show($"Erro na leitura do arquivo local: {err.Message}");
                return "Erro";
            }
            
        }

        public async Task<Boolean> DownloadFileZip(string downloadPath)
        {
            try
            {
                var response = await GitConnection(_urlFileSystem);

                var items = _serialize.DeserializerJson<Models.Content[]>(response);

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
                            return true;
                        }

                    }
                }
                return false;
            
            }catch(Exception err)
            {
                MessageBox.Show($"Erro no download do arquivo ZIP: {err.Message}");
                return false;
            }
        }
    }
}
