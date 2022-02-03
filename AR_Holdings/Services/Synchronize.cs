using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AR_Holdings.DBContent;
using AR_Holdings.Models;
using AR_Holdings.Utilities;
using Dapper;
using ShopifySharp;

namespace AR_Holdings.Services
{
    public interface ISynchronize
    {
        Task LoadShopifyAsync();
        Task ProductsShopifyAsync();
        Task ShopifyAsync();
    }

    public class Synchronize: ISynchronize
    {
        private readonly IDapper _Dapper;

        public Synchronize(IDapper Dapper)
        {
            _Dapper = Dapper;
        }

        public async Task LoadShopifyAsync()
        {
            using IDbConnection db = _Dapper.GetDbconnection();
            try
            {
                DateTime _date = DateTime.Now;

                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    db.Query<int>($"DELETE " +
                    $"FROM ColaArticulos ", null, commandType: CommandType.Text, transaction: tran).FirstOrDefault();

                    var _shopifyService = new ProductService(Settings.General.ShopifyUrl, Settings.General.ShopifyPass);
                    var _productsShopify = await _shopifyService.ListAsync();

                    foreach (var _product in _productsShopify?.Items)
                    { 
                        var _item = _product.Variants.FirstOrDefault();

                        var dbparams = new DynamicParameters();
                        dbparams.Add("SKU", _item.SKU, DbType.String);
                        dbparams.Add("Cantidad", _item.InventoryQuantity ?? 0, DbType.Int32);
                        dbparams.Add("FechaRegistro", _date, DbType.DateTime);
                        dbparams.Add("Sincronizado", false, DbType.Boolean);

                        db.Query<int>($"INSERT INTO ColaArticulos(SKU,Cantidad,FechaRegistro,Sincronizado) " +
                        $"VALUES(@SKU, @Cantidad, @FechaRegistro, @Sincronizado) " +
                        $"", dbparams, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
                       
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

        public async Task ProductsShopifyAsync()
        {
            using IDbConnection db = _Dapper.GetDbconnection();
            try
            {
                DateTime _date = DateTime.Now;
                

                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    var _colaArticulos = db.Query<ColaArticulos>($"SELECT ID, SKU, Cantidad, Sincronizado " +
                                        $"FROM ColaArticulos ", null, commandType: CommandType.Text, transaction: tran).ToList();

                    var _shopifyService = new ProductService(Settings.General.ShopifyUrl, Settings.General.ShopifyPass);
                    var _productsShopify = await _shopifyService.ListAsync();

                    foreach (var _articulo in _colaArticulos)
                    {
                        var _product = _productsShopify.Items.Where(w => w.Variants.FirstOrDefault().SKU == _articulo.SKU).FirstOrDefault();
                        var _productVariant = _product?.Variants.FirstOrDefault();

                        var dbparams = new DynamicParameters();
                        dbparams.Add("ID", _articulo.ID, DbType.Int32);

                        if (_product is null)
                        {
                            dbparams.Add("Cantidad", _articulo.Cantidad, DbType.Int32);
                            dbparams.Add("FechaActualizacion", (DateTime?)null, DbType.DateTime);
                            dbparams.Add("Sincronizado", false, DbType.Boolean);
                        }
                        else
                        {
                            dbparams.Add("Cantidad", _productVariant.InventoryQuantity, DbType.Int32);
                            dbparams.Add("FechaActualizacion", _date, DbType.DateTime);
                            dbparams.Add("Sincronizado", true, DbType.Boolean);
                        }

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

        public async Task ShopifyAsync()
        {
            using IDbConnection db = _Dapper.GetDbconnection();
            try
            {
                DateTime _date = DateTime.Now;

                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    var _colaArticulos = db.Query<ColaArticulos>($"SELECT ID, SKU, Cantidad, Sincronizado " +
                                        $"FROM ColaArticulos ", null, commandType: CommandType.Text, transaction: tran).ToList();

                    var _shopifyService = new ProductService(Settings.General.ShopifyUrl, Settings.General.ShopifyPass);
                    var _productsShopify = await _shopifyService.ListAsync();

                    foreach (var _articulo in _colaArticulos)
                    {
                        var _product = _productsShopify.Items.Where(w => w.Variants.FirstOrDefault().SKU == _articulo.SKU).FirstOrDefault();
                        var _productVariant = _product?.Variants.FirstOrDefault();

                        var dbparams = new DynamicParameters();
                        dbparams.Add("ID", _articulo.ID, DbType.Int32);

                        if (_product is null)
                        {
                            dbparams.Add("FechaActualizacion", (DateTime?)null, DbType.DateTime);
                            dbparams.Add("Sincronizado", false, DbType.Boolean);
                        }
                        else
                        {
                            _product.Variants.FirstOrDefault().InventoryQuantity = Convert.ToInt64(_articulo.Cantidad);
                            var product = await _shopifyService.UpdateAsync((long)_productVariant.ProductId, _product);

                            dbparams.Add("FechaActualizacion", _date, DbType.DateTime);
                            dbparams.Add("Sincronizado", true, DbType.Boolean);
                        }

                        db.Query<int>($"UPDATE ColaArticulos " +
                        $"SET FechaActualizacion = @FechaActualizacion " +
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
