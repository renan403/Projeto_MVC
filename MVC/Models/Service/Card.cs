using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MVC.Models.Service
{
    public class Card:IDisposable
    {    
        private string _card = "";    
        public string Cartao{            
            set { this._card = value; }
        }

        private string _bandeira = "";
        private bool disposedValue;

        public string Bandeira
        {      
            set { this._bandeira = value; }
        }

        public bool CartaoValido()
        {
            return ValidCard();
        }
        private bool ValidCard()
        {
            CreditCardAttribute credit= new ();
            _card = _card.Replace(" ", "");
            switch (_bandeira)
            {
                case "Visa":
                    if (Regex.IsMatch(_card, @"^([4][0-9]{3})([0-9]{4})([0-9]{4})([0-9]{4})$") && _card.Length >= 13 && _card.Length <= 16)
                    {
                        if (credit.IsValid(_card))
                            return true;
                        return false;

                    }
                    return false;
                case "MasterCard":
                    if (Regex.IsMatch(_card, @"^((51|52|53|54|55)[0-9]{1}\d)([0-9]{4})([0-9]{4})([0-9]{4})$") && _card.Length >= 16 && _card.Length <= 19)
                    {
                        if (credit.IsValid(_card))
                            return true;
                        return false;
                    }
                    return false;
                case "Amex":
                    if (Regex.IsMatch(_card, @"^((34|37)[0-9]{2})([0-9]{6})([0-9]{5})$") && _card.Length == 15)
                    {
                        if (credit.IsValid(_card))
                            return true;
                        return false;
                    }
                    return false;
                case "DinerClub":
                    if (Regex.IsMatch(_card, @"^((300|301|302|303|304|305)[0-9]{1})([0-9]{6})([0-9]{4})$") && _card.Length == 14)
                    {

                        if (credit.IsValid(_card))
                            return true;
                        return false;

                    }
                    return false;
            }             
            return false;
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
        // ~Card()
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
