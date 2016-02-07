﻿using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Helpers;
using Security;

namespace views.login
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.RemoveAll();

            if (Request.QueryString["session"] == "expired")
                SessionExpired.Visible = true;

            if (Settings.ForceSsL == "Yes")
            {
                if (!HttpContext.Current.Request.IsSecureConnection)
                {
                    var root = Request.Url.GetLeftPart(UriPartial.Authority);
                    root = root + Page.ResolveUrl("~/");
                    root = root.Replace("http://", "https://");
                    Response.Redirect(root);
                }
            }

            if (Request.IsAuthenticated)
                Response.Redirect("~/views/dashboard/dash.aspx");
        }

        protected void CrucibleLogin_Authenticate(object sender, AuthenticateEventArgs e)
        {
            GetIp();
            var auth = new Authenticate();

            var validationResult = auth.GlobalLogin(CrucibleLogin.UserName, CrucibleLogin.Password, "Web");
            if ((validationResult.IsValid))
            {
                var cloneDeployUser = BLL.User.GetUser(CrucibleLogin.UserName);
                cloneDeployUser.Salt = "";
                cloneDeployUser.Password = "";

                Session["CloneDeployUser"] = cloneDeployUser;
                e.Authenticated = true;
            }
            else
            {
                e.Authenticated = false;
                lblError.Text = validationResult.Message;
                lblError.Visible = true;
            }
        }

        private void GetIp()
        {
            string ipString;
            if (string.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
            {
                ipString = Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                ipString =
                    Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(",".ToCharArray(),
                        StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault();
            }

            IPAddress result = null;
            if (ipString != null && !IPAddress.TryParse(ipString, out result))
                result = IPAddress.None;

            if (result != null)
                Session["ip_address"] = result.ToString();
        }
    }
}