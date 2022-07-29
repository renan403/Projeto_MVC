namespace MVC.Models.Service
{
    public class ProdutoService:IDisposable
    {
        private bool disposedValue;

        public ProdutoService()
        {
           
        }
        public static string PegarNomeUrl(string url)
        {
            string separar = url;
            string[] separar1 = separar.Split("%2F");
            string[] separar2 = separar1[2].Split("?");
            return separar2[0];
        }
        public async Task SubirImg(IWebHostEnvironment _iweb, ModelProduto p ,string idUser , int opcao, string email, string senha)
        {
            using (Auth auth = new())
            {
                if (p.ImgArquivo != null)
                {
                    if (p.ImgArquivo.Length > 0)
                    {
                        var path = Path.Combine(_iweb.WebRootPath, $"img\\Temp\\{p.ImgArquivo.FileName}");
                        try
                        {
                            using Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                            p.ImgArquivo.CopyTo(stream);

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        using Auth storage = new();
                        var url = await storage.UploadImage(_iweb, p.ImgArquivo, idUser, email, senha);
                        using Data data = new();
                        string nomeVendedor = await data.RetornaNome(idUser);
                        p.NomeVendedor = nomeVendedor;
                        p.UrlImg = url;
                        p.ImgArquivo = null;
                        
                        switch (opcao)
                        {
                            case 0:                                
                                await data.AddProduto(idUser,p);
                                break;
                            case 1:
                                await data.AlterarProduto(idUser,p);
                                break;
                        }
                       
                    }
                }

            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ProdutoService()
        // {
        //     // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
