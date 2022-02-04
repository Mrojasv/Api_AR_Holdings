using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AR_Holdings.DBContent;
using AR_Holdings.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using ShopifySharp;
using static AR_Holdings.Models.Shopify;

namespace AR_Holdings.Services
{
    public interface IInvoice
    {
        void SaveInvoice(ShopifyOrder order);
    }

    public class Invoice: IInvoice
    {
        private readonly IDapper _Dapper;
        private readonly ILogger<Invoice> _logger;

        public Invoice(IDapper Dapper, ILogger<Invoice> logger)
        {
            _Dapper = Dapper;
            _logger = logger;
        }

        public void SaveInvoice(ShopifyOrder order)
        {
            // *** Relaciòn de Cliente x Orden debe estar en la tabla Orden, si no existe cliente, si inserta.
            DateTime _date = DateTime.Now;
            using IDbConnection db = _Dapper.GetDbconnection();
            try
            {
                var invoices = new Invoices
                {
                    Orden = new Orden
                    {
                        NumeroOrden = order.order_number ?? 0,
                        SubTotal = order.subtotal_price ?? 0,
                        TotalImpuestos = order.total_tax ?? 0,
                        Total = order.total_price ?? 0,
                        FechaOrden = order.created_at
                    },
                    Cliente = new Cliente
                    {
                        Nombre = order.customer.first_name ?? "Anonimo",
                        Apellido = order.customer.last_name,
                        Email = order.customer.email,
                        Telefono = order.customer.phone,
                        Direccion = order.customer.default_address.address1
                    }
                };

                var _articulos = new List<Articulos>();
                if (order.line_items != null && order.line_items.Any())
                {
                    foreach (var item in order.line_items)
                    {
                        var _articulo = new Articulos
                        {
                            SKU = item.sku,
                            Precio = item.price ?? 0,
                            Cantidad = item.quantity ?? 0,
                            Nombre = item.name,
                            SubTotal = (item.price * item.quantity) ?? 0,
                            TotalImpuestos = (decimal)0, //no encuentro insumos para calcularlo
                            Total = (item.price * item.quantity) ?? 0
                        };

                        _articulos.Add(_articulo);
                    }
                }
                invoices.Articulos = _articulos;

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
                    dbparams.Add("Apellido", invoices.Cliente.Apellido, DbType.String);
                    dbparams.Add("Email", invoices.Cliente.Email, DbType.String);
                    dbparams.Add("Telefono", invoices.Cliente.Telefono, DbType.String);
                    dbparams.Add("Direccion", invoices.Cliente.Direccion, DbType.String);

                    var _clienteId = db.Query<int>($"INSERT INTO Cliente (OrderId,Nombre,Apellido,Email,Telefono, Direccion) " +
                        $"OUTPUT INSERTED.ID " +
                        $"VALUES( @OrderId, @Nombre, @Apellido, @Email, @Telefono, @Direccion " +
                        $") ", dbparams, commandType: CommandType.Text, transaction: tran).FirstOrDefault();

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    _logger.LogError($"{ ex.Message} - {ex.StackTrace}");
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ ex.Message} - {ex.StackTrace}");
                throw ex;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }
        }
    }
}
