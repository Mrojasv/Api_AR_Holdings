using System;
using AR_Holdings.DBContent;

namespace AR_Holdings.Services
{
    public interface IOrders
    {
        void SaveOrderPaid();
    }

    public class Orders: IOrders
    {
        private readonly IDapper _Dapper;

        public Orders(IDapper Dapper)
        {
            _Dapper = Dapper;
        }

        public void SaveOrderPaid()
        {
            //TransacctionScope
            //Guardar Orden
            //Guardas Articulos
            //Guardar Cliente

            // *** Relaciòn de Cliente x Orden debe estar en la tabla Orden, si no existe cliente, si inserta.
        }
    }
}
