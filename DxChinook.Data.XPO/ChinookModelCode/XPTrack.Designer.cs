﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace DxChinook.Data.XPO.Models
{

	[Persistent(@"Track")]
	public partial class XPTrack : XPLiteObject
	{
		int _TrackId;
		[Key(true)]
		public int TrackId
		{
			get { return _TrackId; }
			set { SetPropertyValue<int>(nameof(TrackId), ref _TrackId, value); }
		}
		string _Name;
		[Size(200)]
		public string Name
		{
			get { return _Name; }
			set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
		}
		XPAlbum _AlbumId;
		[Association(@"XPTrackReferencesXPAlbum")]
		public XPAlbum AlbumId
		{
			get { return _AlbumId; }
			set { SetPropertyValue<XPAlbum>(nameof(AlbumId), ref _AlbumId, value); }
		}
		XPMediaType _MediaTypeId;
		[Association(@"XPTrackReferencesXPMediaType")]
		public XPMediaType MediaTypeId
		{
			get { return _MediaTypeId; }
			set { SetPropertyValue<XPMediaType>(nameof(MediaTypeId), ref _MediaTypeId, value); }
		}
		XPGenre _GenreId;
		[Association(@"XPTrackReferencesXPGenre")]
		public XPGenre GenreId
		{
			get { return _GenreId; }
			set { SetPropertyValue<XPGenre>(nameof(GenreId), ref _GenreId, value); }
		}
		string _Composer;
		[Size(220)]
		public string Composer
		{
			get { return _Composer; }
			set { SetPropertyValue<string>(nameof(Composer), ref _Composer, value); }
		}
		int _Milliseconds;
		public int Milliseconds
		{
			get { return _Milliseconds; }
			set { SetPropertyValue<int>(nameof(Milliseconds), ref _Milliseconds, value); }
		}
		int _Bytes;
		public int Bytes
		{
			get { return _Bytes; }
			set { SetPropertyValue<int>(nameof(Bytes), ref _Bytes, value); }
		}
		decimal _UnitPrice;
		public decimal UnitPrice
		{
			get { return _UnitPrice; }
			set { SetPropertyValue<decimal>(nameof(UnitPrice), ref _UnitPrice, value); }
		}
		[Association(@"XPInvoiceLineReferencesXPTrack")]
		public XPCollection<XPInvoiceLine> InvoiceLines { get { return GetCollection<XPInvoiceLine>(nameof(InvoiceLines)); } }
	}

}
