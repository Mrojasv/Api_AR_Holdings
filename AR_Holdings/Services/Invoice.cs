using System;
using System.Data;
using System.Linq;
using AR_Holdings.DBContent;
using AR_Holdings.Models;
using Dapper;
using ShopifySharp;

namespace AR_Holdings.Services
{
    public interface IInvoice
    {
        void SaveInvoice(Order order);
    }

    public class Invoice: IInvoice
    {
        private readonly IDapper _Dapper;

        public Invoice(IDapper Dapper)
        {
            _Dapper = Dapper;
        }

        public void SaveInvoice(Order order)
        {
            // *** Relaciòn de Cliente x Orden debe estar en la tabla Orden, si no existe cliente, si inserta.

            var invoices = new Invoices();
            using IDbConnection db = _Dapper.GetDbconnection();
            try
            {
                DateTime _date = DateTime.Now;

                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    var dbparams = new DynamicParameters();
                    dbparams.Add("NumeroOrden", invoices.Orden.NumeroOrden, DbType.String);
                    dbparams.Add("SubTotal", invoices.Orden.SubTotal, DbType.Decimal);
                    dbparams.Add("TotalImpuestos", invoices.Orden.TotalImpuestos, DbType.Decimal);
                    dbparams.Add("Total", invoices.Orden.Total, DbType.Decimal);
                    dbparams.Add("FechaCreacion", _date, DbType.DateTime);
                    dbparams.Add("FechaOrden", invoices.Orden.FechaOrden, DbType.DateTime);

                    var _orderId = db.Query<int>($"INSERT INTO Orden (NumeroOrden,SubTotal,TotalImpuestos,Total,FechaCreacion, FechaOrden) " +
                        $"OUTPUT INSERTED.ID " +
                        $"VALUES( @NumeroOrden, @SubTotal, @TotalImpuestos, @Total, @FechaCreacion, @FechaOrden " +
                        $") ", dbparams, commandType: CommandType.Text, transaction: tran).FirstOrDefault();

                    foreach (var articulo in invoices.Articulos)
                    {
                        dbparams = new DynamicParameters();
                        dbparams.Add("OrderId", _orderId, DbType.Int32);
                        dbparams.Add("SKU", articulo.SKU, DbType.String);
                        dbparams.Add("Precio", articulo.Precio, DbType.Decimal);
                        dbparams.Add("Cantidad", articulo.Cantidad, DbType.Int32);
                        dbparams.Add("Nombre", articulo.Nombre, DbType.String);
                        dbparams.Add("SubTotal", articulo.SubTotal, DbType.Decimal);
                        dbparams.Add("TotalImpuestos", articulo.TotalImpuestos, DbType.Decimal);
                        dbparams.Add("Total", articulo.Total, DbType.Decimal);

                        var _articuloId = db.Query<int>($"INSERT INTO Articulos (OrderId,SKU,Precio,Cantidad,Nombre, SubTotal, TotalImpuestos, Total) " +
                            $"OUTPUT INSERTED.ID " +
                            $"VALUES( @OrderId, @SKU, @Precio, @Cantidad, @Nombre, @SubTotal, @TotalImpuestos, @Total " +
                            $") ", dbparams, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
                    }

                    dbparams = new DynamicParameters();
                    dbparams.Add("OrderId", _orderId, DbType.Int32);
                    dbparams.Add("Nombre", invoices.Cliente.Nombre, DbType.String);
                    dbparams.Add("Apellido", invoices.Orden.TotalImpuestos, DbType.Decimal);
                    dbparams.Add("Email", invoices.Orden.Total, DbType.Decimal);
                    dbparams.Add("Telefono", _date, DbType.DateTime);
                    dbparams.Add("Direccion", invoices.Orden.FechaOrden, DbType.DateTime);

                    var _clienteId = db.Query<int>($"INSERT INTO Cliente (OrderId,Nombre,Apellido,Email,Telefono, Direccion) " +
                        $"OUTPUT INSERTED.ID " +
                        $"VALUES( @OrderId, @Nombre, @Apellido, @Email, @Telefono, @Direccion " +
                        $") ", dbparams, commandType: CommandType.Text, transaction: tran).FirstOrDefault();

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }
        }
    }
}
