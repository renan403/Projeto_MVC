# Projeto_MVC_OLD

### Estou atualizando esse mesmo projeto em outro repositorio segue o link https://github.com/renan403/Projeto_cSharp

O objetivo que tenho é praticar a linguagem C# para desenvolver um pequeno site de compra utilizando firebase realtime, authenticator e storage, ainda não é a conclusão dele, com o tempo que eu for mexendo irei atualizando e melhorando.



## Site de Compra

O projeto não está da forma final, encontrei alguns impedimentos que futuramente eu tentarei concertar. Resumidamente tem venda e compra de produtos, histórico de compras, detalhes do pedido, exibir recibo, carrinho, criação de cadastro, cadastro de forma de pagamento ( Eu iria implementar codigo QR porem ainda não pensei uma forma de ligar com o celular, vou deixar o codigo no final para caso queira testar ) e endereços, com exceção do Cadastro do user (não coloquei muita informação) ambos podem ser alterados depois de serem salvos, nas primeiras versões do projeto iria ser usado validação de Email pelo proprio projeto mas não foi possivel (a google decidiu tirar uma função que deixava um app externo enviar email (como essa função passou a ser paga, eu tive de retirar e ultilizar o firebase autentication, não sera muito util mas deixei a biblioteca que fiz no projeto, assim como a de "segurança" que serviria para criptografar as informações antes de serem enviadas para o firebase e entre algumas telas)).

Há alguns probleminhas que só posso arrumar depois de aprender outros conteudos, portanto, decidi que irei dar uma pausa para estudar e assim que voltar irei arrumar tanto visualmente quanto em questão a erros e performance.


Obs:A ideia principal era criar site de compra utilizando o MVC e duas APIs( uma de endereço(facilitar no cadastro) e uma de simulação de crédito), porem como a de simulação de crédito é necessario CNPJ eu optei por não tentar utilizar, encontrei outras APIs gratuitas porem não faria muito sentido utiliza-las, então, fiz apenas um simples validador na parte de cadastro de cartão <br />(para facilitar utilize https://www.4devs.com.br/gerador_de_numero_cartao_credito).
<br />
<br />
E por fim caso queira ver o comportamento no proprio firebase é só criar a conta e um projeto gratuito, pegar substituir no arquivo Auth (chave api(que está no "geral do projeto") linha 15, appspot você entrará no Storage é gs://NomeDoProjeto (coloque apenas oque estiver apos as duas '//') linha 30 por exemplo), e no arquivo Data você deve colocar a url do Realtime na linha 24 (caso não esteja dando erro, confere se as regras permite gravar e ler informações).
<br />
<br />
Enfim agradeço seu tempo, qualquer duvida, critica, sugestão ou elogio eu irei agradecer.
<br />
<br />
  try // obs: este código esta no site https://www.macoratti.net/15/06/c_qrcd1.htm<br />
  {<br />
     //seria uma ideia de fazer algo semelhante ao pix<br />
     var bw = new BarcodeWriter();<br />
     var encOptions = new EncodingOptions() { Width = 200, Height = 200, Margin = 0 };<br />
     bw.Options = encOptions;<br />
     bw.Format = BarcodeFormat.QR_CODE;<br />
     var resultado = new Bitmap(bw.Write("Conteudo que deve aparecer após escanear"));<br />
     //                                   vou deixar de exemplo<br />
     var path = Path.Combine("caminho", $"img\\Temp\\CodigoQr.Png");<br />
     Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite,FileShare.ReadWrite);<br />
     resultado.Save(stream, System.Drawing.Imaging.ImageFormat.Png);<br />
   }<br />
   catch<br />
   {<br />
      throw;<br />
   }<br />
