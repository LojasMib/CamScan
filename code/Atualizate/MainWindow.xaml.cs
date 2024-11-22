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

namespace Atualizate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GitHubCliente _gitClient;

        private readonly int? _camScanProcessId;
        

        public MainWindow()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                _camScanProcessId = int.Parse(args[1]);
                MessageBox.Show($"Parâmetro recebido: {args[1]}");
                Loaded += InitializeConection;
            }
            else
            {
                MessageBox.Show("Nenhum parâmetro foi recebido!");
                Application.Current.Shutdown();
            }
            
        }

        private void InitializeConection(object sender, RoutedEventArgs e)
        {
            try
            {
                _gitClient = new GitHubCliente();
                _gitClient.InitializeHttpClient();
                GetRemoteVersionAsync();
            }
            catch(Exception err)
            {
                MessageBox.Show($"Erro na conexão ao repositório do GitHub: {err}");
            }
        }

        private void DeleteAllFilesExcept(string folderPath, string file1)
        {
            try
            {
                foreach (string filePath in Directory.GetFiles(folderPath))
                {
                    string fileName = Path.GetFileName(filePath);

                    if (!fileName.Equals(file1, StringComparison.OrdinalIgnoreCase) || !fileName.Equals(_gitClient._versionLocal.Outdate, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Delete(filePath);
                    }
                }
                MessageBox.Show("Arquivos excluídos com sucesso, exceto os especificados!");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Erro ao tentar excluir arquivos " + ex.Message);
            }
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao copiar o conteúdo: {ex.Message}");
            }
        }

        private async void GetRemoteVersionAsync()
        {

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string versionPath = System.IO.Path.Combine(currentDirectory, "version");
            string newPathJsonConfig = System.IO.Path.Combine(versionPath, "newversion","newversion.json");
            string jsonConfigLocal = System.IO.Path.Combine(versionPath, "version.json");            

            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(jsonConfigLocal));
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(newPathJsonConfig));

            try
            {
                string newJson = await _gitClient.DownloadJsonConfig(newPathJsonConfig);

                string currentJson = _gitClient.ReaderFileJson(jsonConfigLocal);
                if (newJson == "Erro" || currentJson == "Erro")
                {
                    Application.Current.Shutdown();
                }

                string downloadNewFile = System.IO.Path.Combine(versionPath, _gitClient._versionUpdate.NameFileZip);

                
                int numberNumber = int.Parse(currentJson.Replace(".", ""));
                int newNumberNumber = int.Parse(newJson.Replace(".", ""));

                if (newNumberNumber > numberNumber)
                {
                    MessageBox.Show("Uma atualização do sistema foi encontrada, o processo de atualizaçã irá iniciar");
                    Boolean result = await _gitClient.DownloadFileZip(downloadNewFile);
                    if (result == false)
                    {
                        Application.Current.Shutdown();
                    }
                    MessageBox.Show("Arquivo CamScan.zip baixado com sucesso!", "Download completo");

                    if(_camScanProcessId.HasValue)
                    {
                        try
                        {
                                
                            string zipPath =  $"{versionPath}/CamScan.zip";
                            string extractPath = Path.GetFullPath(Path.Combine(currentDirectory, "..", ".."));

                            string atualizatePath = Path.GetFullPath(Path.Combine(currentDirectory, ".."));
                            MessageBox.Show($"Extract Path: {extractPath}; Atualizate Path: {atualizatePath};");

                            Process camScanProcess = Process.GetProcessById(_camScanProcessId.Value);
                            camScanProcess.Kill();
                            MessageBox.Show("CamScam foi encerrado para executar atualização.");
                                
                            if (Directory.Exists(extractPath))
                            {
                                DeleteAllFilesExcept(extractPath, atualizatePath);
                                MessageBox.Show($"CamScam.zip será extraido para {extractPath}.");
                                ZipFile.ExtractToDirectory(zipPath, extractPath);
                                MessageBox.Show("Arquivo ZIP extraido com sucesso");
                                string ProgramAtualizate = $"{extractPath}/{_gitClient._versionUpdate.PathExecutable}";
                                if (Directory.Exists(ProgramAtualizate))
                                {
                                    CopyDirectory(ProgramAtualizate, extractPath);
                                    MessageBox.Show("Arquivo principal atualizado");
                                }
                                Directory.Delete($"{extractPath}/CamScan", true);
                            }
                            if (File.Exists($"{extractPath}/CamScan.exe"))
                            {
                                Process atualizateProcess = new Process();
                                atualizateProcess.StartInfo.FileName = $"{extractPath}/{_gitClient._versionUpdate.NameExecutable}";
                                atualizateProcess.Start();
                            }
                            Application.Current.Shutdown();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro ao tentar acessar o CamScan: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Nenhuma atualização encontrada");
                    Application.Current.Shutdown();
                }

            }
            catch(HttpRequestException err)
            {
                MessageBox.Show("Erro ao acessar a API de atualização:" + err.Message);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro durante o processamento: " + ex.Message);
                Application.Current.Shutdown();
            }
        }
    }
}