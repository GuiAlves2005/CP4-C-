using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessadorAssincronoDeArquivos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Exibe a tela inicial esperada
            Console.WriteLine("=== Processador Assíncrono de Arquivos de Texto ==="); 
            Console.WriteLine("Informe o caminho de um diretório contendo arquivos .txt:");

            // Solicita ao usuário um diretório
            string? directoryPath = Console.ReadLine(); // 

            // Valida o diretório informado
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                Console.WriteLine("Diretório inválido ou não encontrado. Pressione qualquer tecla para sair.");
                Console.ReadKey();
                return;
            }

            // Busca todos os arquivos .txt no diretório
            var files = Directory.GetFiles(directoryPath, "*.txt");

            if (files.Length == 0)
            {
                Console.WriteLine("Nenhum arquivo .txt encontrado no diretório especificado.");
                return;
            }

            // Lista os arquivos localizados
            Console.WriteLine("\nArquivos .txt encontrados:");
            foreach (var file in files)
            {
                Console.WriteLine($"- {Path.GetFileName(file)}"); 
            }
            Console.WriteLine("\nIniciando processamento...");

            var stopwatch = Stopwatch.StartNew();

            // Coleção segura para threads para armazenar os resultados
            var results = new ConcurrentBag<string>();

            // Processa cada arquivo de forma paralela usando async/await [cite_start] 
            var processingTasks = files.Select(async filePath => 
            {
                // Mostra na tela o início do processamento do arquivo
                Console.WriteLine($"Processando arquivo {Path.GetFileName(filePath)}..."); 

                // Lê o conteúdo do arquivo
                string[] lines = await File.ReadAllLinesAsync(filePath); 

                // Calcula a quantidade de linhas e palavras
                int lineCount = lines.Length; 
                int wordCount = lines.Sum(line => line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length); 

                // Feedback visual para o usuário, indicando que o programa não está travado
                Console.WriteLine($"Concluído: {Path.GetFileName(filePath)} - {lineCount} linha(s), {wordCount} palavra(s)."); 

                // Adiciona o resultado formatado à coleção
                results.Add($"{Path.GetFileName(filePath)} - {lineCount} linhas - {wordCount} palavras");
            });

            // Aguarda a conclusão de todas as tarefas de processamento
            await Task.WhenAll(processingTasks);

            stopwatch.Stop();
            Console.WriteLine($"\nProcessamento concluído com sucesso em {stopwatch.Elapsed.TotalSeconds:F2} segundos!");

            // Gera o relatório consolidado
            await GenerateReport(results.ToList());
        }

        static async Task GenerateReport(List<string> results)
        {
            // Define o caminho da pasta e do arquivo de relatório
            string exportFolderName = "export"; // 
            string reportFileName = "relatorio.txt"; // 
            string exportDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exportFolderName); 
            string reportFilePath = Path.Combine(exportDirectoryPath, reportFileName); 

            try
            {
                // Cria o diretório 'export' se ele não existir
                Directory.CreateDirectory(exportDirectoryPath);

                // Ordena os resultados pelo nome do arquivo (opcional, mas melhora a legibilidade)
                results.Sort();

                // Salva os resultados em um arquivo
                await File.WriteAllLinesAsync(reportFilePath, results);

                // Mensagem de conclusão clara para o usuário
                Console.WriteLine($"Relatório gerado em: {reportFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro ao gerar o relatório: {ex.Message}");
            }
        }
    }
}