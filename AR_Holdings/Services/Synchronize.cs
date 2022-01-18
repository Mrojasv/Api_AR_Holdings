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
                    var _ColaArticulos = db.Query<ColaArticulos>($"SELECT ID, SKU, Cantidad, Sincronizado " +
                                        $"FROM ColaArticulos ", null, commandType: CommandType.Text, transaction: tran).ToList();

                    //Obtener lista del inventario de Shopify, requiero accesToken??
                    var _ShopifyInventory = new List<ShopifyInventory>();
                    var _shopifyService = new ProductService(Settings.General.ShopifyUrl, Settings.General.ShopAccessToken);
                    var _products = await _shopifyService.ListAsync();

                    foreach (ColaArticulos _product in _ColaArticulos)
                    {
                        _product.Sincronizado = false;
                        var _shopifyProduct = _ShopifyInventory.Where(w => w.sku == _product.SKU).FirstOrDefault();
                        if(!string.IsNullOrEmpty(_shopifyProduct.sku))
                        {
                            _product.Cantidad = _shopifyProduct.available;
                            _product.Sincronizado = true;
                        }

                        dbparams = new DynamicParameters();
                        dbparams.Add("ID", _product.ID, DbType.Int32);
                        dbparams.Add("Cantidad", _product.Cantidad, DbType.Int32);
                        dbparams.Add("FechaActualizacion", _date, DbType.DateTime);
                        dbparams.Add("Sincronizado", _product.Sincronizado, DbType.Boolean);

                        db.Query<int>($"UPDATE ColaArticulos " +
                        $"SET Cantidad = @Cantidad " +
                        $",FechaActualizacion = @FechaActualizacion " +
                        $",Sincronizado = @Sincronizado " +
                        $"WHERE ID = @ID ", dbparams, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
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
