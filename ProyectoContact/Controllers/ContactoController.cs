using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using ProyectoContact.Models;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Runtime.Remoting.Messaging;
using System.Web.Services.Description;

namespace ProyectoContact.Controllers
{
    public class ContactoController : Controller
    {    
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString(); 
        
        private static List<Contacto> lista = new List<Contacto>();    

         
        // GET: Contacto
        public ActionResult Index()
        {
            lista = new List<Contacto>();
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand comando = new SqlCommand("LISTAR",con);
                comando.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Contacto contacto = new Contacto();
                        contacto.IdContacto = Convert.ToInt32(lector["ID"]);
                        contacto.Nombre = lector["NOMBRE"].ToString();
                        contacto.Apellido = lector["APELLIDO"].ToString();
                        contacto.Telefono = lector["TELEFONO"].ToString() ;
                        contacto.Correo = lector["CORREO"].ToString();

                        lista.Add(contacto);
                    }
                }
            }
            return View(lista);
        }
        public ActionResult ErrorRegistrar()
        {
            return View();
        }
        public ActionResult Registrar()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult RegistrarNuevo(Contacto ocontacto)
        {


            if (!ModelState.IsValid)
            {
               return RedirectToAction("ErrorRegistrar", "Contacto");
                
            }
           
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand comando = new SqlCommand("sp_registrar", con);
                comando.Parameters.AddWithValue("Nombre", ocontacto.Nombre);
                comando.Parameters.AddWithValue("Apellido", ocontacto.Apellido);
                comando.Parameters.AddWithValue("Telefono", ocontacto.Telefono);
                comando.Parameters.AddWithValue("Correo", ocontacto.Correo);
                comando.CommandType = CommandType.StoredProcedure;
                con.Open();
                comando.ExecuteNonQuery();
            }

            return RedirectToAction("Index", "Contacto");

        }
        public ActionResult Editar(int? idcontacto)
        {
            
            try
            {
                if (idcontacto == null)
                    return RedirectToAction("Index", "Contacto");
                
                Contacto contacto = lista.Where(c => c.IdContacto == idcontacto).FirstOrDefault();
                
                
                return View(contacto);

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public ActionResult Editar(Contacto ocontacto)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    SqlCommand comando = new SqlCommand("sp_editar", con);
                    comando.Parameters.AddWithValue("IdContacto", ocontacto.IdContacto);
                    comando.Parameters.AddWithValue("Nombre", ocontacto.Nombre);
                    comando.Parameters.AddWithValue("Apellido", ocontacto.Apellido);
                    comando.Parameters.AddWithValue("Telefono", ocontacto.Telefono);
                    comando.Parameters.AddWithValue("Correo", ocontacto.Correo);
                    comando.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    comando.ExecuteNonQuery();
                }
            }
            else
            {
                return RedirectToAction("ErrorRegistrar","Contacto");
            }

            return RedirectToAction("Index", "Contacto");
        }

        public ActionResult Eliminar(int? idcontacto)
        {

            try
            {
                if (idcontacto == null)
                    return RedirectToAction("Index", "Contacto");

                Contacto contacto = lista.Where(c => c.IdContacto == idcontacto).FirstOrDefault();


                return View(contacto);

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult Eliminar(string IdContacto)
        {
            using (SqlConnection con = new SqlConnection(conexion))
            {
                SqlCommand comando = new SqlCommand("sp_eliminar", con);
                comando.Parameters.AddWithValue("IdContacto", IdContacto);
                comando.CommandType = CommandType.StoredProcedure;
                con.Open();
                comando.ExecuteNonQuery();
            }


            return RedirectToAction("Index", "Contacto");
        }
    }
}