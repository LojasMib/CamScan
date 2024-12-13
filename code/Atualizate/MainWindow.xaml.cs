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
using System.Diagnostics;
using Atualizate.Class;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Atualizate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GitHubCliente _gitClient;
        private readonly Logger _log;

        private readonly int? _camScanProcessId;

        private readonly string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        

        public MainWindow()
        {
            InitializeComponent();
            
            string[] args = Environment.GetCommandLineArgs();
            _log = new Logger();
            if (args.Length > 1)
            {
                _camScanProcessId = int.Parse(args[1]);
                _log.Log($"Parâmetro recebido: {_camScanProcessId}");

            }
            else
            {
                _log.Log("ERRO: Nenhum parâmetro foi recebido!");
                Application.Current.Shutdown();
            }

            this.ContentRendered += MainWindow_ContentRenderer;
            
        }

        private async void MainWindow_ContentRenderer(object sender, EventArgs e)
        {
            await UpdateProgress(0, "Atualize Start");
            Initialize_Conection();
        }

        private async void Initialize_Conection()
        {
            try
            {
                string jsonConfigLocal = System.IO.Path.Combine(currentDirectory, "version", "version.json");
                _gitClient = new GitHubCliente(jsonConfigLocal);
                string InitializeConnection =  _gitClient.InitializeHttpClient();
                if( InitializeConnection != "true")
                {
                    _log.Log(InitializeConnection);
                    Application.Current.Shutdown();
                }
                await UpdateProgress(10, "Conexão com servidor estabelecida!");

                await GetRemoteVersionAsync();
            }
            catch(Exception err)
            {
                _log.Log($"ERRO: Erro na conexão ao repositório do GitHub: {err}");
            }
        }

        private void DeleteAllFilesExcept(string folderPath, string folder1)
        {
            try
            {
                folder1 = Path.GetFileName(folder1);

                foreach (string filePath in Directory.GetFiles(folderPath))
                {
                    string fileName = Path.GetFileName(filePath);

                    if (IsFileInUse(filePath))
                    {
                        _log.Log($"Aviso: Arquivo em uso, não será excluído: {fileName}");
                    }
                    

                    if (!fileName.Equals(folder1, StringComparison.OrdinalIgnoreCase) && !fileName.Equals(_gitClient._versionLocal.Outdate, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Delete(filePath);
                    }
                }

                foreach(string directoryPath in Directory.GetDirectories(folderPath))
                {
                    string folderName = Path.GetFileName(directoryPath);
                    if(!folderName.Equals(folder1, StringComparison.OrdinalIgnoreCase) &&
                        !folderName.Equals(_gitClient._versionLocal.Outdate, StringComparison.OrdinalIgnoreCase))
                    {
                        Directory.Delete(directoryPath, true);
                    }
                }
            }
            catch(Exception ex)
            {
                _log.Log($"ERRO: Erro ao tentar excluir arquivos para atualização {ex.Message}");
            }
        }

        private bool IsFileInUse(string filePath)
        {
            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    fs.Close();
                }
            }
            catch (IOException)
            {
                return true; // Arquivo em uso
            }
            return false;
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            try
            {
                // Cria o diretório de destino se ele não existir
                Directory.CreateDirectory(destinationDir);

                // Copia todos os arquivos
                foreach (string filePath in Directory.GetFiles(sourceDir))
                {
                    string fileName = Path.GetFileName(filePath);
                    string destFilePath = Path.Combine(destinationDir, fileName);
                    File.Copy(filePath, destFilePath, true); // Sobrescreve se já existir
                }

                // Copia todas as subpastas recursivamente
                foreach (string subDirPath in Directory.GetDirectories(sourceDir))
                {
                    string subDirName = Path.GetFileName(subDirPath);
                    string destSubDirPath = Path.Combine(destinationDir, subDirName);
                    CopyDirectory(subDirPath, destSubDirPath);
                }

                _log.Log("Arquivos e pastas copiados para diretório correto");
            }
            catch (Exception ex)
            {
                _log.Log($"ERRO: Erro ao copiar o conteúdo: {ex.Message}");
            }
        }

        private void DeleteOutdatedFiles(string PathNewVersion, string fileZip)
        {
            Directory.Delete(PathNewVersion, true);
            File.Delete(fileZip);
        }

        private async Task UpdateProgressBar(int progress, string status)
        {
            try
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    ProgressBar.Value = progress;
                    ProgressLabel.Text = $"Progresso: {progress}%";
                    InfoProgress.Text = status + (string.IsNullOrEmpty(InfoProgress.Text) ? "" : "\n" + InfoProgress.Text);
                }, DispatcherPriority.Render);
            }
            catch (Exception ex)
            {
                _log.Log($"ERRO: Falha ao atualizar a barra de progresso: {ex.Message}");
            }

        }

        private async Task GetRemoteVersionAsync()
        {
            try
            {

                //Passo 1: Configuração Inicial
                string versionPath = System.IO.Path.Combine(currentDirectory, "version");
                string jsonConfigLocal = System.IO.Path.Combine(currentDirectory, "version", "version.json");
                string newFileJsonConfig = System.IO.Path.Combine(versionPath, "newversion", "newversion.json");

                CreateDirectories(versionPath);

                await UpdateProgress(15, "Diretórios do arquivo json atual e do novo verificados e criados!");

                //Passo 2: Download e validação do .json Remoto
                string newJson = await _gitClient.DownloadJsonConfig(newFileJsonConfig);
                string currentJson = _gitClient._versionLocal.Number;
                ValidateJsonContent(currentJson, newJson);

                await UpdateProgress(20, "Arquivo remoto .json, baixado!");

                //Passo 3: Verifica se há atualização
                if (NeedsUpdate(currentJson, newJson))
                {
                    await UpdateProgress(30, "Atualização encontrada");

                    string downloadNewFile = System.IO.Path.Combine(versionPath, _gitClient._versionUpdate.NameFileZip);
                    await _gitClient.DownloadFileZip(downloadNewFile);
                
                    await UpdateProgress(40, "Atualização baixada");

                    //Passo 4: Processar atualização
                    await ProcessUpdate(versionPath, currentDirectory);

                }
                await UpdateProgress(100, "Atualização concluída!");
                
            }
            catch(HttpRequestException err)
            {
                _log.Log($"ERRO: Erro de comunicação HTTP com servidor {err.Message}");
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                _log.Log($"ERRO: Erro durante o processamento: {ex.Message}");
                Application.Current.Shutdown();
            }
            finally
            {
                Application.Current.Shutdown();
            }
        }
        
        private async Task UpdateProgress(int progress, string message)
        {
            await UpdateProgressBar(progress, message);
            _log.Log(message);
        }

        private void CreateDirectories(string versionPath)
        {
            Directory.CreateDirectory(versionPath);
            Directory.CreateDirectory(Path.Combine(versionPath, "newversion"));
        }

        private void ValidateJsonContent(string currentJson, string newJson)
        {
            if (currentJson.Contains("ERRO:"))
            {
                throw new Exception("O JSON local está vazio.");
            }
            if (newJson.Contains("ERRO:"))
            {
                throw new Exception("O JSON remoto está vazio.");
            }
        }

        private bool NeedsUpdate(string currentJson, string newJson)
        {
            int currentVersion = int.Parse(currentJson.Replace(".", ""));
            int newVersion = int.Parse(newJson.Replace(".", ""));

            return newVersion > currentVersion;
        }

        private async Task ProcessUpdate(string versionPath, string currentDirectory)
        {
            string zipPath = Path.Combine(versionPath, "CamScan.zip");
            string currentPath = Path.GetFullPath(Path.Combine(currentDirectory, "..", ".."));
            string atualizatePath = Path.GetFullPath(Path.Combine(currentDirectory, ".."));

            if(!Directory.Exists(currentPath))
            {
                throw new Exception("Pasta principal não encontrada");
            }
            if (!Directory.Exists(atualizatePath))
            {
                throw new Exception("Pasta de atualização não encontrada");
            }
            if(!File.Exists(zipPath))
            {
                throw new Exception("O arquivo ZIP de atualização não foi encontrado.");
            }

            Process camScanProcess = Process.GetProcessById(_camScanProcessId.Value);
            camScanProcess.Kill();
            await UpdateProgress(50, "Aplicação principal encerrada");
            await Task.Delay(300);
            DeleteAllFilesExcept(currentPath, atualizatePath);
            await UpdateProgress(55, "Arquivos da pasta principal deletados");

            await UpdateProgress(60, "Descompactando atualização...");
            ZipFile.ExtractToDirectory(zipPath, currentPath);
            await UpdateProgress(65, $"Descompactação concluida!");

            DeleteOutdatedFiles(Path.Combine(versionPath, "newversion"), zipPath);
            await UpdateProgress(70, "Arquivo zip deletado");

            _gitClient.UpdateJsonConfig(Path.Combine(versionPath, "version.json"));
            await UpdateProgress(80, "Novo arquivo .json salvo");

            RestartApplication(currentPath);
        }

        private async void RestartApplication(string currentPath)
        {
            string executablePath = Path.Combine(currentPath, _gitClient._versionUpdate.NameExecutable);
            if (File.Exists(executablePath))
            {
                Process atualizateProcess = new Process();
                atualizateProcess.StartInfo.FileName = $"{currentPath}/{_gitClient._versionUpdate.NameExecutable}";
                atualizateProcess.Start();
                await UpdateProgress(90, "Aplicação principal reiniciada");
            }
            else
            {
                throw new Exception($"Arquivo executável não encontrado: {executablePath}");
            }
        }
    }
}