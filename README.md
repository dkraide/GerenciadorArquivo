Passo 1:
Dentro de WEB e API, altere o arquivo appsettings.json com a string de conexao do seu banco de dados SQL

Passo 2:
execute o comando update-database no Console de Gerenciador de Pacotes
![image](https://github.com/dkraide/GerenciadorArquivo/assets/43674712/0f513ac5-3b81-40dc-ac99-fdd3a3aac07e)

se preferir, pode fazer o backup do banco de dados usando o arquivo backup.bak em Communication > dep

Passo 3:
Dentro do projeto Communication, na pasta Constants na classe Consts.cs altere o campo URLAPI com a url da API quando executada
O projeto WEB utiliza a API para envio e tratamento dos arquivos. Se nao configurar esse campo, nao conseguira fazer upload de arquivos.

Passo 4:
Ao efetuar o passo a passo. Sera criado automaticamente um usuario com username "daniel123" e senha "123456" com privilegios de admin, podendo a partir dele criar novos usuarios.

