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
        Serialize _serialize = new Serialize();


        public GitHubCliente(string jsonCurrent)
        {
            ReaderFileJson(jsonCurrent);
        }

        public string InitializeHttpClient()
        {
            try
            {
                _client = new HttpClient();
                _client.DefaultRequestHeaders.UserAgent.TryParseAdd("Atualizate-App/1.0");

                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _versionLocal.TokenAPI); ;
                _baseUrl = $"https://api.github.com/repos/{_versionLocal.RepoOwner}/{_versionLocal.RepoName}";
                _urlJsonConfig = $"{_baseUrl}/contents/code/Atualizate/version/version.json?ref={_versionLocal.BranchName}";
                _urlFileSystem = $"{_baseUrl}/contents/code?ref={_versionLocal.BranchName}";
                return "true";
            }
            catch (Exception err)
            {
                return $"Erro na conexão ao repositório do GitHub: {err}";
            }
        }

        private async Task<string> GitConnection(string url)
        {            
            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException)
            {
                throw new Exception("A solicitação ao Servidor Git expirou.");
            }
            catch (Exception err)
            {
                throw new Exception($"Erro na conexão com o Servidor Git: {err} ");
            }
        }

        public async Task<string> DownloadJsonConfig(string downloadPath)
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
                return _versionUpdate.Number;
            }
            else
            {
                return $"ERRO: resposta da conexão é nula ou vazia.";
            }
        }

        public void ReaderFileJson(string path)
        {
            try
            {
                string jsonContent = File.ReadAllText(path);
                _versionLocal = _serialize.DeserializerJson<Models.Version>(jsonContent);
            }
            catch(Exception ex)
            {
                throw new Exception($"ERRO: erro na leitura do arquivo {ex}");
            }
            
        }

        public void UpdateJsonConfig(string path)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                File.WriteAllText(path, JsonSerializer.Serialize(_versionUpdate, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception err)
            {
                MessageBox.Show($"Erro na atualização do version.json: {err.Message}");
            }
        }

        public async Task DownloadFileZip(string downloadPath)
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
                        return;
                    }
                }
            }
            throw new Exception($"ERRO: erro no download do arquivo zip");
            
        }
    }
}
