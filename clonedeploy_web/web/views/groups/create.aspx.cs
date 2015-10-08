﻿using System;
using BasePages;
using Helpers;
using Models;
using Security;

namespace views.groups
{
    public partial class GroupCreate : Groups
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          
            if (IsPostBack) return;
            if (new Authorize().IsInMembership("User"))
                Response.Redirect("~/views/dashboard/dash.aspx?access=denied");
            PopulateForm();
        }

        protected void PopulateForm()
        {
           PopulateImagesDdl(ddlGroupImage);
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            var group = new Group();

        
            group.Name = txtGroupName.Text;
            group.Description = txtGroupDesc.Text;
            group.Image = Convert.ToInt32(ddlGroupImage.SelectedValue);
            group.SenderArguments = txtGroupSenderArgs.Text;
            group.Type = ddlGroupType.Text;
            group.ImageProfile = group.Image == 0 ? 0 : Convert.ToInt32(ddlImageProfile.SelectedValue);
          
            var bllGroup = new BLL.Group();
            bllGroup.AddGroup(group);


                Response.Redirect("~/views/groups/edit.aspx?groupid=" + group.Id);

        }


        protected void ddlGroupImage_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlGroupImage.Text == "Select Image") return;
            PopulateImageProfilesDdl(ddlImageProfile, Convert.ToInt32(ddlGroupImage.SelectedValue));

        }
    }
}