using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ConfigurationTool.XML.XMLServiceConfiguration
{
	/// <summary>
	/// Serializationclass: Contains information of ServiceConfigurations, Roles and their Settings.
	/// ServiceConfiguration/*
	/// </summary>
	[Serializable]
	[XmlRoot(Namespace = "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration", IsNullable = true)]
	public class ServiceConfiguration
	{
		#region fields

		[XmlAttribute("serviceName")]
		public string ServiceName { get; set; }

		[XmlAttribute("osFamily")]
		public string OsFamily { get; set; }

		[XmlAttribute("osVersion")]
		public string OsVersion { get; set; }

		[XmlAttribute("schemaVersion")]
		public string SchemaVersion { get; set; }

		[XmlElement("Role")]
		public List<Role> RoleList { get; set; }

		#endregion fields

		#region ctors

		/// <summary>
		/// Constructor: Create new role collection.
		/// </summary>
		public ServiceConfiguration()
		{
			RoleList = new List<Role>();
		}

		/// <summary>
		/// Constructor: Create new role collection with exisiting roles.
		/// </summary>
		/// <param name="roles">Existing roles</param>
		public ServiceConfiguration(List<Role> roles)
		{
			RoleList = roles;
		}

		#endregion ctors

		/// <summary>
		/// Merges two serviceconfigurations into a new one.
		/// </summary>
		/// <param name="secondServiceConfiguration">The file Serviceconfiguration to merge from</param>
		/// <returns>The merged Serviceconfiguration. If Serviceconfiguration is null, then (this)</returns>
		public void Merge(ServiceConfiguration secondServiceConfiguration)
		{
			if (secondServiceConfiguration == null)
			{
				return;
			}
			foreach (var role in RoleList)
			{
				foreach (var otherRole in secondServiceConfiguration.RoleList)
				{
					if (role.Name == otherRole.Name)
					{
						role.Merge(otherRole);
					}
				}
			}
		}

		#region getoperations

		/// <summary>
		/// Gets the name of the current service configuration from the settings.
		/// </summary>
		/// <returns></returns>
		public string GetServiceConfigurationName()
		{
			var configuration = this;
			foreach (var role in configuration.RoleList)
			{
				foreach (var setting in role.SettingList)
				{
					if (setting.Name.Equals("TeamViewer.WindowsAzure.ServiceConfigurationName"))
					{
						return setting.Value;
					}
				}
			}
			return null;
		}

		#endregion getoperations
	}
}