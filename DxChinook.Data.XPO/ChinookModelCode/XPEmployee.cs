using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace DxChinook.Data.XPO.Models
{

	public partial class XPEmployee
	{
		public XPEmployee(Session session) : base(session) { }
		public override void AfterConstruction() { base.AfterConstruction(); }
	}

}
