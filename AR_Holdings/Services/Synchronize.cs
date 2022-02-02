using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AR_Holdings.DBContent;
using AR_Holdings.Utilities;
using Dapper;
using ShopifySharp;
using static AR_Holdings.Models.Products;

namespace AR_Holdings.Services
{
    public interface ISynchronize
    {
        Task ProductsShopifyAsync();
    }

    public class Synchronize: ISynchronize
    {
        private readonly IDapper _Dapper;

        public Synchronize(IDapper Dapper)
        {
            _Dapper = Dapper;
        }

        public async Task ProductsShopifyAsync()
        {
            using IDbConnection db = _Dapper.GetDbconnection();
            try
            {
                DateTime _date = DateTime.Now;
                var dbparams = new DynamicParameters();

                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    var _colaArticulos = db.Query<ColaArticulos>($"SELECT ID, SKU, Cantidad, Sincronizado " +
                                        $"FROM ColaArticulos ", null, commandType: CommandType.Text, transaction: tran).ToList();

                    var _shopifyService = new ProductService(Settings.General.ShopifyUrl, Settings.General.ShopifyPass);
                    var _productsShopify = await _shopifyService.ListAsync();

                    foreach (var _product in _productsShopify?.Items)
                    {
                        ColaArticulos _articulo = _colaArticulos.Where(w => w.SKU == _product.Variants.FirstOrDefault().SKU).FirstOrDefault();
                        var _item = _product.Variants.FirstOrDefault();

                        if (_articulo is null)
                        {
                            dbparams = new DynamicParameters();
                            dbparams.Add("SKU", _item.SKU, DbType.String);
                            dbparams.Add("Cantidad", _item.InventoryQuantity ?? 0, DbType.Int32);
                            dbparams.Add("FechaRegistro", _date, DbType.DateTime);
                            dbparams.Add("Sincronizado", false, DbType.Boolean);

                            db.Query<int>($"INSERT INTO ColaArticulos(SKU,Cantidad,FechaRegistro,Sincronizado) " +
                            $"VALUES(@SKU, @Cantidad, @FechaRegistro, @Sincronizado) " +
                            $"", dbparams, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
                        }
                        else
                        {
                            dbparams = new DynamicParameters();
                            dbparams.Add("ID", _articulo.ID, DbType.Int32);
                            dbparams.Add("Cantidad", _item.InventoryQuantity, DbType.Int32);
                            dbparams.Add("FechaActualizacion", _date, DbType.DateTime);
                            dbparams.Add("Sincronizado", true, DbType.Boolean);

                            db.Query<int>($"UPDATE ColaArticulos " +
                            $"SET Cantidad = @Cantidad " +
                            $",FechaActualizacion = @FechaActualizacion " +
                            $",Sincronizado = @Sincronizado " +
                            $"WHERE ID = @ID ", dbparams, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
                        }
                    }

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
