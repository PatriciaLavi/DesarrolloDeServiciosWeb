using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace parteRolDistrito.Models
{
    public class ProductoModel
    {
		public int id_producto { get; set;}
		public string nom_producto { get; set; } 
		public decimal precio { get; set; }
		public string ruta_imagen { get; set; }
		public string nombre_imagen { get; set; }
		public int id_categoria { get; set; }
		public int stock { get; set; }
		public Boolean activo { get; set; }
	}
}
