﻿using System;
using System.Collections.Generic;
using CloneDeploy_Common;
using CloneDeploy_Entities;
using CloneDeploy_Web.BasePages;

namespace CloneDeploy_Web.views.admin.cluster
{
    public partial class roles : Admin
    {
        protected void btnUpdateSettings_OnClick(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.UpdateAdmin);
            var listSettings = new List<SettingEntity>
            {
                new SettingEntity
                {
                    Name = "Operation Mode",
                    Value = ddlOperationMode.Text,
                    Id = Call.SettingApi.GetSetting("Operation Mode").Id
                },
                new SettingEntity
                {
                    Name = "Tftp Server Role",
                    Value = chkTftpServer.Checked ? "1" : "0",
                    Id = Call.SettingApi.GetSetting("Tftp Server Role").Id
                },
                new SettingEntity
                {
                    Name = "Multicast Server Role",
                    Value = chkMulticastServer.Checked ? "1" : "0",
                    Id = Call.SettingApi.GetSetting("Multicast Server Role").Id
                }
            };

            EndUserMessage = Call.SettingApi.UpdateSettings(listSettings)
                ? "Successfully Updated Settings"
                : "Could Not Update Settings";
            Response.Redirect("~/views/admin/cluster/roles.aspx?level=2");
        }

        protected void ddlOperationMode_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayRoles();
        }

        private void DisplayRoles()
        {
            divRoles.Visible = ddlOperationMode.Text != "Single";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            ddlOperationMode.Text = GetSetting(SettingStrings.OperationMode);
            if (ddlOperationMode.Text != "single")
            {
                chkTftpServer.Checked = Convert.ToBoolean(Convert.ToInt16(GetSetting(SettingStrings.TftpServerRole)));
                chkMulticastServer.Checked =
                    Convert.ToBoolean(Convert.ToInt16(GetSetting(SettingStrings.MulticastServerRole)));
            }
            DisplayRoles();
        }
    }
}