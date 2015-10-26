﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BasePages;
using Models.ImageSchema;
using ImageSchema = BLL.ImageSchema;

namespace views.images
{
    public partial class ImageSpecs : Images
    {
        protected void btnPart_Click(object sender, EventArgs e)
        {
            var selectedHd = (string) (ViewState["selectedHD"]);
            var control = sender as Control;
            if (control == null) return;
            var gvRow = (GridViewRow) control.Parent.Parent;
            var gv = (GridView) gvRow.FindControl("gvFiles");
            var selectedPartition = gvRow.Cells[3].Text;

            var btn = (LinkButton) gvRow.FindControl("partClick");

            if (gv.Visible == false)
            {
                gv.Visible = true;
                var td = gvRow.FindControl("tdFile");
                td.Visible = true;
                gv.DataSource = ImageSchema.GetPartitionImageFileInfoForGridView(Image, selectedHd,
                    selectedPartition);
                gv.DataBind();
                btn.Text = "-";
            }
            else
            {
                gv.Visible = false;
                var td = gvRow.FindControl("tdFile");
                td.Visible = false;
                btn.Text = "+";
            }
        }

        protected void btnHd_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var gvRow = (GridViewRow) control.Parent.Parent;
            var gv = (GridView) gvRow.FindControl("gvParts");

            var selectedHd = gvRow.Cells[3].Text;
            ViewState["selectedHD"] = gvRow.RowIndex.ToString();
            ViewState["selectedHDName"] = selectedHd;


            var partitions = ImageSchema.GetPartitionsForGridView(Image, selectedHd);
            var btn = (LinkButton) gvRow.FindControl("btnHd");
            if (gv.Visible == false)
            {
                gv.Visible = true;

                var td = gvRow.FindControl("tdParts");
                td.Visible = true;
                gv.DataSource = partitions;
                gv.DataBind();

                btn.Text = "-";
            }
            else
            {
                gv.Visible = false;

                var td = gvRow.FindControl("tdParts");
                td.Visible = false;
                btn.Text = "+";
            }

            foreach (GridViewRow row in gv.Rows)
            {
                row.Cells[6].Text = "hi";
                var gvVg = (GridView) row.FindControl("gvVG");

                if (partitions[row.RowIndex].VolumeGroup != null)
                {
                    gvVg.DataSource = new List<VolumeGroup> {partitions[row.RowIndex].VolumeGroup};
                    gvVg.DataBind();

                    gvVg.Visible = true;
                    var td = row.FindControl("tdVG");
                    td.Visible = true;
                }
                var isActive = ((HiddenField) row.FindControl("HiddenActivePart")).Value;
                if (isActive != "1") continue;
                var box = row.FindControl("chkPartActive") as CheckBox;
                if (box != null) box.Checked = true;
            }
        }


        protected void btnVG_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var gvRow = (GridViewRow) control.Parent.Parent;
            var gv = (GridView) gvRow.FindControl("gvLVS");

            var selectedHd = (string) (ViewState["selectedHD"]);


            var btn = (LinkButton) gvRow.FindControl("vgClick");
            if (gv.Visible == false)
            {
                gv.Visible = true;

                var td = gvRow.FindControl("tdLVS");
                td.Visible = true;
                gv.DataSource = ImageSchema.GetLogicalVolumesForGridView(Image, selectedHd);
                gv.DataBind();
                btn.Text = "-";
            }

            else
            {
                gv.Visible = false;
                var td = gvRow.FindControl("tdLVS");
                td.Visible = false;
                btn.Text = "+";
            }

            foreach (var box in (from GridViewRow row in gv.Rows
                let isActive = ((HiddenField) row.FindControl("HiddenActivePart")).Value
                where isActive == "1"
                select row.FindControl("chkPartActive")).OfType<CheckBox>())
            {
                box.Checked = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) PopulateHardDrives();
        }

        protected void PopulateHardDrives()
        {
            gvHDs.DataSource = ImageSchema.GetHardDrivesForGridView(Image);
            gvHDs.DataBind();


            foreach (var box in (from GridViewRow row in gvHDs.Rows
                let isActive = ((HiddenField) row.FindControl("HiddenActive")).Value
                where isActive == "1"
                select row.FindControl("chkHDActive")).OfType<CheckBox>())
            {
                box.Checked = true;
            }
        }
    }
}