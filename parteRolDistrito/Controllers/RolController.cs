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
    public class RolController : Controller
    {
        private readonly IConfiguration _Iconfig;
        public RolController(IConfiguration Iconfig)
        {
            _Iconfig = Iconfig;
        }

        IEnumerable<RolModel> Roles()
        {
            List<RolModel> lista = new List<RolModel>();
            using (SqlConnection cn = new SqlConnection(_Iconfig["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("usp_RolListar",cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new RolModel()
                    {
                        idRol=dr.GetInt32(0),
                        nomRol=dr.GetString(1)
                    });
                }
            }
            return lista;
        }

        public async Task<IActionResult> ListadoRoles()
        {
            return View(await Task.Run(() => Roles()));
        }

        RolModel BuscarRol(int id)
        {
            RolModel reg = Roles().Where(c => c.idRol == id).FirstOrDefault();
            return reg;
        }

        public IActionResult Create()
        {
            return View(new RolModel());
        }

        [HttpPost]
        public IActionResult Create(RolModel reg)
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
                    SqlCommand cmd = new SqlCommand("usp_InsertRol", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nomrol",reg.nomRol);
                    int num = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha insertado {num} rol (es)";
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
            RolModel reg = BuscarRol(id);
            if (reg == null)
                return RedirectToAction("ListadoRoles");
            return View(reg);
        }

        [HttpPost]
        public IActionResult Edit(RolModel reg)
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
                    SqlCommand cmd = new SqlCommand("usp_EditRol", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idrol", reg.idRol);
                    cmd.Parameters.AddWithValue("@nomrol", reg.nomRol);
                    int num = cmd.ExecuteNonQuery();
                    mensaje = $"Se ha actualizado {num} rol (es)";
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
