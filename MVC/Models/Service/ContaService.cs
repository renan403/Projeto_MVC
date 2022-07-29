using Firebase.Database;

namespace MVC.Models.Service
{
    public class Result
    {
        public List<dynamic>? ProdutosEqtd;
        public float? ValorTotal;
        public int? QuantiaTotal;
        public List<ModelProduto>? Prod;
    }
    public class ContaService
    {
        public async Task<Result> FinalizarCompra(IReadOnlyCollection<FirebaseObject<ModelProduto>> model)
        {
            float? valorTotal = 0;
            int? quantiaTotal = 0;
            List<dynamic> produtosEqtd = new();

            using Data data = new();
            List<ModelProduto> prod = new();
            foreach (var idProduto in model)
            {
                var ID = idProduto.Object.IdProduto;
                ModelProduto item = await data.RetornaProdutoPorID(ID) ?? new ModelProduto { };
                valorTotal += item.PrecoProd * idProduto.Object.QtdPorProd;
                quantiaTotal += idProduto.Object.QtdPorProd;

                List<dynamic> unirLista = new();
                unirLista.Add(item);
                unirLista.Add(idProduto.Object.QtdPorProd);
                unirLista.Add(idProduto.Key);
                produtosEqtd.Add(unirLista);

                // Implementar na model para gerar nota

                item.Qtd = idProduto.Object.QtdPorProd;
                item.ValorTotal = (float)Math.Round((decimal)((decimal)item.PrecoProd * idProduto.Object.QtdPorProd), 2);
                item.Cancelado = false;
                item.Data = DateTime.Now.ToString("dd/MM/yyyy");
                prod.Add(item);
            }
            Result rt = new()
            {
                Prod= prod,
                ProdutosEqtd = produtosEqtd,
                QuantiaTotal = quantiaTotal,
                ValorTotal = valorTotal,
            };
            return rt;


        }
    }
}
