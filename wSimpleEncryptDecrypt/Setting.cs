using System;
using System.Xml.Serialization;

namespace ConfigurationTool.XML.XMLServiceConfiguration
{
	/// <summary>
	/// Serializationclass: Contains information of ServiceConfigurations, Roles and their Settings.
	/// ServiceConfiguration/Role/Setting/*
	/// </summary>
	[Serializable]
	public class Setting
	{
		#region fields

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("value")]
		public string Value { get; set; }

		[XmlIgnore]
		public bool IsNew { get; set; }

		#endregion fields

		#region ctors

		/// <summary>
		/// Constructor: Creates a setting with given parameters.
		/// </summary>
		/// <param name="thisname">Name of the setting</param>
		/// <param name="thisvalue">Value of the setting</param>
		public Setting(string thisname, string thisvalue)
		{
			Name = thisname;
			Value = thisvalue;
		}

		/// <summary>
		/// Constructor: Parameterless contstructor basically does nothing.
		/// </summary>
		public Setting()
		{
		}

		#endregion ctors
	}
}