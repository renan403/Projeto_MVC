namespace MVC.Models.Service
{
    public class Gerador:IDisposable
    {
        private bool disposedValue;

        public string aleatorios()
        {
            var rand = new Random();
            var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var numAlea = new string(Enumerable.Repeat(allChar, 6).Select(letra =>letra[rand.Next(allChar.Length)]).ToArray());


            return numAlea;
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
        // ~Gerador()
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
