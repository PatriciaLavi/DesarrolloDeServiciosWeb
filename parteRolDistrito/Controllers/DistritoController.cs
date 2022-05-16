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
    public class DistritoController : Controller
    {
        private readonly IConfiguration _Iconfig;
        public DistritoController(IConfiguration Iconfig)
        {
            _Iconfig = Iconfig;
        }

        IEnumerable<DistritoModel> Distritos()
        {
            List<DistritoModel> lista = new List<DistritoModel>();
            using (SqlConnection cn = new SqlConnection(_Iconfig["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("usp_DistritoListar", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new DistritoModel()
                    {
                        idDistrito = dr.GetInt32(0),
                        nomDistrito = dr.GetString(1)
                    });
                }
            }
            return lista;
        }

        public async Task<IActionResult> ListadoDistritos()
        {
            return View(await Task.Run(() => Distritos()));
        }

        DistritoModel BuscarDistrito(int id)
        {
            DistritoModel reg = Distritos().Where(c => c.idDistrito == id).FirstOrDefault();
            return reg;
        }

        public IActionResult Create()
        {
            return View(new DistritoModel());
        }

        [HttpPost]

        public IActionResult Create(DistritoModel reg)
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
                    SqlCommand cmd = new SqlCommand("usp_InsertDistrito", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nomdistrito", reg.nomDistrito);
                    int num = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha insertado {num} distrito (s)";
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
            DistritoModel reg = BuscarDistrito(id);
            if (reg == null)
                return RedirectToAction("ListadoDistritos");
            return View(reg);
        }

        [HttpPost]

        public IActionResult Edit(DistritoModel reg)
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
                    SqlCommand cmd = new SqlCommand("usp_EditDistrito", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@iddistrito", reg.idDistrito);
                    cmd.Parameters.AddWithValue("@nomdistrito", reg.nomDistrito);
                    int num = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha actualizado {num} distrito (s)";
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
