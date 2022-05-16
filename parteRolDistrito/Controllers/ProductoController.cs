using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using parteRolDistrito.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace parteRolDistrito.Controllers
{
    public class ProductoController : Controller
    {


        private readonly IConfiguration _Iconfig;
        public ProductoController(IConfiguration Iconfig)
        {
            _Iconfig = Iconfig;
        }

        IEnumerable<ProductoModel> Productos()
        {
            List<ProductoModel> lista = new List<ProductoModel>();
            using (SqlConnection cn = new SqlConnection(_Iconfig["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("spListarProducto", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new ProductoModel()
                    {
                        id_producto= dr.GetInt32(0),
                        nom_producto=dr.GetString(1),
                        precio=dr.GetDecimal(2),
                        ruta_imagen=dr.GetString(3),
                        nombre_imagen=dr.GetString(4),
                        id_categoria=dr.GetInt32(5),
                        stock= dr.GetInt32(6),
                        activo= dr.GetBoolean(7)
                    });
                }
            }
            return lista;
        }

        public async Task<IActionResult> ListadoProductos()
        {
            return View(await Task.Run(() => Productos()));
        }

        ProductoModel BuscarProductos(int id)
        {
            ProductoModel reg = Productos().Where(c => c.id_producto == id).FirstOrDefault();
            return reg;
        }

        public IActionResult Create()
        {
            return View(new ProductoModel());
        }

        [HttpPost]

        public IActionResult Create(ProductoModel reg)
        {
            if (!ModelState.IsValid)
            {
                return View(reg);
            }
            string mensaje = string.Empty;
            using (SqlConnection cn = new SqlConnection(_Iconfig["ConnectionStrings:cn"]))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("SPRegistrarProducto", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nomproducto", reg.nom_producto);
                    cmd.Parameters.AddWithValue("@precio", reg.precio);
                    cmd.Parameters.AddWithValue("@rutaimag", reg.ruta_imagen);
                    cmd.Parameters.AddWithValue("@nomimag", reg.nombre_imagen);
                    cmd.Parameters.AddWithValue("@idcategoria", reg.id_categoria);
                    cmd.Parameters.AddWithValue("@stock", reg.stock);
                    cmd.Parameters.AddWithValue("@activo", reg.activo);
                    int num = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha insertado {num} producto (s)";
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message.ToString();
                }
                ViewBag.mensaje = mensaje;
                return View(reg);
            }
        }

        public IActionResult Edit(int id)
        {
            ProductoModel reg = BuscarProductos(id);
            if (reg == null)
                return RedirectToAction("ListadoProductos");
            return View(reg);
        }

        [HttpPost]
        public IActionResult Edit(ProductoModel reg)
        {
            if (!ModelState.IsValid)
            {
                return View(reg);
            }
            string mensaje = string.Empty;
            using (SqlConnection cn = new SqlConnection(_Iconfig["ConnectionStrings:cn"]))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("spactualizarproducto", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@idproducto", reg.id_producto);
                    cmd.Parameters.AddWithValue("@nomproducto", reg.nom_producto);
                    cmd.Parameters.AddWithValue("@precio", reg.precio);
                    cmd.Parameters.AddWithValue("@rutaimag", reg.ruta_imagen);
                    cmd.Parameters.AddWithValue("@nomimag", reg.nombre_imagen);
                    cmd.Parameters.AddWithValue("@idcategoria", reg.id_categoria);
                    cmd.Parameters.AddWithValue("@stock", reg.stock);
                    cmd.Parameters.AddWithValue("@activo", reg.activo);
                    int num = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha actualizado {num} producto (s)";
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message.ToString();
                }
                ViewBag.mensaje = mensaje;
                return View(reg);
            }
        }



        public IActionResult Index()
        {
            return View();
        }
    }
}
