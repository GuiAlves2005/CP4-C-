# CP4-C#

Processador Assíncrono de Arquivos de Texto - README Técnico

Este documento detalha a implementação técnica do projeto, explicando a estrutura do código e as escolhas de arquitetura para atender aos requisitos.

💡 Concepção e Estrutura do Projeto
O objetivo era criar uma aplicação de console em C# e .NET 8 capaz de processar múltiplos arquivos de texto de forma paralela e não-bloqueante.



A estrutura do programa foi dividida nas seguintes etapas lógicas:


Entrada e Validação: Coleta e valida o diretório informado pelo usuário.


Processamento Paralelo: O núcleo da aplicação, onde cada arquivo é processado em uma tarefa separada.


Análise Individual de Arquivos: Leitura assíncrona e contagem de linhas/palavras de cada arquivo.


Geração do Relatório: Consolidação e salvamento dos resultados em disco.

🛠️ Detalhes da Implementação
1. Entrada e Validação
A aplicação inicia solicitando ao usuário o caminho de um diretório. A entrada é validada para garantir que o diretório realmente exista antes de prosseguir. Em seguida, o sistema utiliza 

Directory.GetFiles() para buscar por todos os arquivos com a extensão .txt na pasta informada.

2. O Coração Assíncrono do Programa
O núcleo do processamento paralelo foi construído com a seguinte abordagem:

Utilizei 

files.Select(async filePath => { ... }) para transformar a lista de caminhos de arquivo em uma coleção de tarefas assíncronas (Task), onde cada tarefa é responsável por processar um único arquivo.

Para executar todas essas tarefas simultaneamente e aguardar a conclusão de todas, usei o comando 

await Task.WhenAll(...). Isso garante que o programa só continue para a etapa de relatório após todos os arquivos terem sido analisados.

Para coletar os resultados de forma segura (thread-safe) à medida que cada tarefa terminava, optei por uma ConcurrentBag<string>. Esta coleção é projetada para permitir que múltiplas tarefas adicionem itens ao mesmo tempo sem causar conflitos de acesso.

3. Leitura e Análise de Cada Arquivo
Dentro de cada tarefa assíncrona, a implementação foi a seguinte:


Leitura Assíncrona: A leitura do conteúdo do arquivo foi feita com await File.ReadAllLinesAsync(filePath). Essa escolha é crucial para operações de I/O, pois libera a thread principal para manter a aplicação responsiva em vez de esperar o disco responder.


Contagem: A contagem de linhas foi obtida simplesmente pelo tamanho do array de strings retornado (lines.Length). A contagem de palavras foi calculada usando 

string.Split() para dividir cada linha em palavras e Enumerable.Sum() para somar os totais de todas as linhas.

4. Geração do Relatório
A etapa final foi encapsulada no método GenerateReport para manter o código organizado.

Ele primeiro cria o diretório 

export no local da aplicação, caso ainda não exista.

Em seguida, ele salva os resultados (previamente armazenados na 

ConcurrentBag) de forma assíncrona no arquivo relatorio.txt  usando 

await File.WriteAllLinesAsync(...).

🚀 Tecnologias e Conceitos Aplicados

C# e .NET 8 


Programação Assíncrona (async/await) 

Task Parallel Library (TPL) com Task.WhenAll para paralelismo.

Coleções Thread-Safe (ConcurrentBag)


Manipulação de Arquivos Assíncrona (File.ReadAllLinesAsync e File.WriteAllLinesAsync).

NOME - RM
Guilherme Alves de Lima 550433
