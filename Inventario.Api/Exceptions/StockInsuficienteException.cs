namespace Inventario.Api.Exceptions;

public class StockInsuficienteException : Exception
{
    public IReadOnlyList<int> ProductosRechazados { get; }

    public StockInsuficienteException(IEnumerable<int> productosRechazados)
        : base("No hay stock suficiente para uno o más productos.")
    {
        ProductosRechazados = productosRechazados.ToList();
    }
}