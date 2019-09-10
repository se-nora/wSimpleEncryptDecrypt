using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ConfigurationTool.XML.XMLServiceConfiguration
{
	/// <summary>
	/// Serializationclass: Contains information of ServiceConfigurations, Roles and their Settings.
	/// ServiceConfiguration/Role/*
	/// </summary>
	[Serializable]
	public class Role
	{
		#region fields

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlArray("ConfigurationSettings")]
		public List<Setting> SettingList { get; set; }

		#endregion fields

		#region ctors

		/// <summary>
		/// Constructor: Create new setting collection.
		/// </summary>
		public Role() : this(new List<Setting>())
		{
		}

		/// <summary>
		/// Constructor: Create new setting collection.
		/// </summary>
		public Role(List<Setting> settings)
		{
			SettingList = settings;
		}

		#endregion ctors

		/// <summary>
		/// Merges the settings of two comparable roles into a new one.
		/// </summary>
		/// <param name="secondRole">The role to merge with</param>
		public void Merge(Role secondRole)
		{
			foreach (var mySettings in SettingList)
			{
				foreach (var otherSetting in secondRole.SettingList)
				{
					if (mySettings.Name != otherSetting.Name)
					{
						continue;
					}
					mySettings.Value = otherSetting.Value;
					mySettings.IsNew = true;
				}
			}
		}
	}
}