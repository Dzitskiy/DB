using NHibernate.Mapping.Attributes;
using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB1.Entities
{
	public class Client
	{
		private IList<Deposit> deposits1;

		public long id { get; set; }

		public string first_name { get; set; }

		public string last_name { get; set; }

		public string middle_name { get; set; }

		public string email { get; set; }

		public virtual IList<Deposit> deposits { get => deposits1; set => deposits1 = value; }
	}
}
