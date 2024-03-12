using NHibernate.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace DB1.Entities
{
	public class Deposit
	{
		public long id { get; set; }

		public virtual Client client { get; set; }

		public virtual DateTime created_at { get; set; }
	}
}
