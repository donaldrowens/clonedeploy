﻿using System;
using CloneDeploy_Common;
using CloneDeploy_Entities;
using CloneDeploy_Web.BasePages;

namespace CloneDeploy_Web.views.users
{
    public partial class views_users_creategroup : Users
    {
        protected void btnSubmit_OnClick(object sender, EventArgs e)
        {
            var userGroup = new CloneDeployUserGroupEntity
            {
                Name = txtGroupName.Text,
                Membership = ddlGroupMembership.Text,
                IsLdapGroup = chkldap.Checked ? 1 : 0
            };
            if (chkldap.Checked)
                userGroup.GroupLdapName = txtLdapGroupName.Text;

            var result = Call.UserGroupApi.Post(userGroup);
            if (!result.Success)
                EndUserMessage = result.ErrorMessage;
            else
            {
                EndUserMessage = "Successfully Created User Group";
                Response.Redirect("~/views/users/editgroup.aspx?groupid=" + result.Id);
            }
        }

        protected void chkldap_OnCheckedChanged(object sender, EventArgs e)
        {
            divldapgroup.Visible = chkldap.Checked;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RequiresAuthorization(AuthorizationStrings.Administrator);
        }
    }
}