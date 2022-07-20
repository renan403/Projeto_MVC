namespace MVC.Models.Service
{
    public class GeralService:IDisposable
    {
        private bool disposedValue;

        //cep e cidade
        public string[] RetornoRNCUC(string endereco)
        {
            string[] array = new string[6];
            if (endereco == null || endereco == "")
            {             
                return array;
            }
            try
            {
                var separador = endereco.Split("/");
                array[0] = separador[0] ?? "";
                array[1] = separador[1] ?? "";
                array[2] = separador[2] ?? "";
                array[3] = separador[3] ?? "";
                array[4] = separador[4] ?? "";
                array[5] = separador[5] ?? "";
            }
            catch
            {
                return array;
            }
                return array;
            
           
            
        }
        public string? RetornaNomeNull(string nome)
        {
            if (nome == "")
            {
                return null;
            }
            return nome;
        }
        public string FormatarNomeNav(string nomeParaFormatar)
        { 
            if (nomeParaFormatar != null)
            {
                var nome = nomeParaFormatar.Split(" ");
                if (nome.Length == 1)
                {
                    var primeiroNome = char.ToUpper(nome[0][0]) + nome[0].Substring(1);

                    return $"{primeiroNome}";
                }
                if (nome[1].Length <= 3)
                {
                    var primeiroNome = char.ToUpper(nome[0][0]) + nome[0].Substring(1);
                    var conjunto = nome[1];
                    var sobreNome = char.ToUpper(nome[2][0]) + nome[2].Substring(1);

                    return $"{primeiroNome} {conjunto} {sobreNome}";
                }
                else
                {
                    var primeiroNome = char.ToUpper(nome[0][0]) + nome[0].Substring(1);
                    var sobreNome = char.ToUpper(nome[1][0]) + nome[1].Substring(1);
                    return $"{primeiroNome} {sobreNome}";
                }
            }
            return string.Empty;
        }
        public string ultimosDigitos(string card)
        {
            card = card.Substring(card.Length-4);
            return string.Empty;
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
        // ~EnderecoService()
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
