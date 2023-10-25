<h1>Gerenciador de Arquivos</h1>
<h3>Configuracao Geral</h3>
<p><b>Passo 1:</b></p>
Dentro do projeto <b>WEB</b> e tambem do projeto  <b>API</b>, altere o arquivo appsettings.json com a string de conexao do seu banco de dados SQL conforme a imagem abaixo:<br>
![image](https://github.com/dkraide/GerenciadorArquivo/assets/43674712/f330bf22-d855-4f7b-82b1-66e134b31a6e)
<br><br>
<p><b>Passo 2:</b></p>
execute o comando update-database no Console de Gerenciador de Pacotes conforme a imagem abaixo:<br>
![image](https://github.com/dkraide/GerenciadorArquivo/assets/43674712/0f513ac5-3b81-40dc-ac99-fdd3a3aac07e)
<br><br>
se preferir, pode fazer o backup do banco de dados usando o arquivo backup.bak em Communication > dep: <br>
![image](https://github.com/dkraide/GerenciadorArquivo/assets/43674712/5791160b-2300-48ae-b3fd-bbede7628da5)
<br><br>
<p><b>Passo 3:</b></p>
Dentro do projeto Communication, na pasta Constants na classe Consts.cs altere o campo URLAPI com a url da API quando executada
O projeto WEB utiliza a API para envio e tratamento dos arquivos. Se nao configurar esse campo, nao conseguira fazer upload de arquivos.<br>
![image](https://github.com/dkraide/GerenciadorArquivo/assets/43674712/742988c1-93a9-494e-96f3-ca2ea52e3aab)
<br><br>

Passo 4:
Ao efetuar o passo a passo. Sera criado automaticamente um usuario com username "daniel123" e senha "123456" com privilegios de admin, podendo a partir dele criar novos usuarios.
<br><br>
<h3>Configuracao API</h3>
<br>
A API esta protejida um esquema de autorizacao JWT Token. Para acessar todos os endpoints, primeiro deve efetuar login com o usuario admin (saiba mais no passo 4 de configuracoes geral), ou com algum usuario criado por voce.
<br>
<p><b>Passo 1 - Efetuar Login:</b></p>
Clique em Try it Out, preencha o JSON com os dados de userName e password, por fim clique em Execute <br>
![image](https://github.com/dkraide/GerenciadorArquivo/assets/43674712/40dce8b8-ee5a-4427-8e12-d6fb6a1a97a4)
<br><br>
<p><b>Passo 2 - Utilize o Token</b></p>
Um Token sera gerado. copie esse codigo e coloque em Authorize.
![image](https://github.com/dkraide/GerenciadorArquivo/assets/43674712/acec81fc-7ccb-463b-b722-a1839d79670a) <br>
![image](https://github.com/dkraide/GerenciadorArquivo/assets/43674712/261b5dc1-0448-48cc-ae61-688f6ff21560)
<br><br>
<h3>Observacoes gerais</h3>
Voce pode fazer o upload de qualquer tipo de arquivo. Porem se o arquivo XML, ele sera limitado para aceitar apenas NF-e. Se enviar qualquer outro XML que nao esteja no padrao NF-e, retornatara um erro.





