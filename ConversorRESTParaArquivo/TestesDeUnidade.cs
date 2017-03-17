using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;

namespace ConversorRESTParaArquivo
{
    [TestClass]
    public class TestesDeUnidade
    {
        [TestMethod]
        public void LerPaginaInicialGoogleESalvarArquivoEmPDF()
        {
            // Define os dados de consulta da API.
            var url = "www.google.com";
            var webapiAddress = @"http://127.0.0.1:9000/api/arquivo?url=";
            var uri = new Uri(webapiAddress + url);

            // Define o path em que o PDF será salvo.
            var nomeArquivo = url.Replace(".", string.Empty);
            var fileName = $@"C:\PDF\{nomeArquivo}.pdf";

            var arquivo = ObterArquivo(uri);
            var sucesso = SalvarArquivo(fileName, arquivo);

            Assert.IsTrue(sucesso);
        }

        private byte[] ObterArquivo(Uri uri)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Define o tempo limite de espera do client.
                    client.Timeout = TimeSpan.FromMinutes(30);

                    // Obtém o resultado do HTTP através da API.
                    var getAsyncResult = client.GetAsync(uri).Result;

                    // Valida o retorno da API e lança uma Exception se não tiver obtido sucesso.
                    getAsyncResult.EnsureSuccessStatusCode();

                    // Converte o resultado do HTTP (HttpResponseMessage) em String contendo o arquivo codificado.
                    var arquivo = getAsyncResult.Content.ReadAsStringAsync().Result;

                    // Desserializa a String em um objeto do tipo byte[].
                    return JsonConvert.DeserializeObject<byte[]>(arquivo);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool SalvarArquivo(string fileName, byte[] arquivo)
        {
            BinaryWriter Writer = null;

            try
            {
                Writer = new BinaryWriter(File.OpenWrite(fileName));
                Writer.Write(arquivo);
                Writer.Flush();
                Writer.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
