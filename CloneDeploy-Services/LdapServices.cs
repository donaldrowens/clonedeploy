﻿using System;
using System.DirectoryServices;
using CloneDeploy_Common;
using log4net;

namespace CloneDeploy_Services
{
    /// <summary>
    ///     Summary description for Ldap
    /// </summary>
    public class LdapServices
    {
        private readonly ILog log = LogManager.GetLogger(typeof(LdapServices));

        public bool Authenticate(string username, string pwd, string ldapGroup = null)
        {
            if (SettingServices.GetSettingValue(SettingStrings.LdapEnabled) != "1") return false;

            var path = "LDAP://" + SettingServices.GetSettingValue(SettingStrings.LdapServer) + ":" +
                       SettingServices.GetSettingValue(SettingStrings.LdapPort) + "/" +
                       SettingServices.GetSettingValue(SettingStrings.LdapBaseDN);
            string _filterAttribute = null;

            var entry = new DirectoryEntry(path, username, pwd);

            if (SettingServices.GetSettingValue(SettingStrings.LdapAuthType) == "Basic")
                entry.AuthenticationType = AuthenticationTypes.None;
            else if (SettingServices.GetSettingValue(SettingStrings.LdapAuthType) == "Secure")
                entry.AuthenticationType = AuthenticationTypes.Secure;
            else if (SettingServices.GetSettingValue(SettingStrings.LdapAuthType) == "SSL")
                entry.AuthenticationType = AuthenticationTypes.SecureSocketsLayer;
            try
            {
                // Bind to the native AdsObject to force authentication.
                var obj = entry.NativeObject;
                var search = new DirectorySearcher(entry);
                search.Filter = "(" + SettingServices.GetSettingValue(SettingStrings.LdapAuthAttribute) + "=" + username +
                                ")";
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("memberOf");

                var result = search.FindOne();
                if (null == result)
                {
                    return false;
                }

                // Update the new path to the user in the directory
                path = result.Path;
                _filterAttribute = (string) result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                log.Error("Could Not Authenticate User: " + username + " " + ex.Message);
                return false;
            }

            if (ldapGroup != null)
            {
                return GetGroups(_filterAttribute, path, ldapGroup);
            }
            return true;
        }

        public bool GetGroups(string _filterAttribute, string _path, string ldapGroup)
        {
            var search = new DirectorySearcher(_path);
            search.Filter = "(cn=" + _filterAttribute + ")";
            search.PropertiesToLoad.Add("memberOf");
            try
            {
                var result = search.FindOne();
                var propertyCount = result.Properties["memberOf"].Count;
                string dn;
                int equalsIndex, commaIndex;

                for (var propertyCounter = 0;
                    propertyCounter < propertyCount;
                    propertyCounter++)
                {
                    dn = (string) result.Properties["memberOf"][propertyCounter];

                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return false;
                    }
                    if (string.Equals(ldapGroup, dn.Substring(equalsIndex + 1,
                        commaIndex - equalsIndex - 1), StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error obtaining group names. " + ex.Message);
                return false;
            }
            return false;
        }
    }
}