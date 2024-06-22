using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Anti_Money_Laundering_tracking_System
{
    public partial class CustLogin : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-O16H131N\SQLEXPRESS;Initial Catalog=master;Integrated Security=True");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (System.Web.HttpContext.Current.Session["Alert"] != null)
            {
                if (Session["Alert"].ToString() == "true")
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "msgtype", "alert('Customer Registered Successfully!!!')", true);
                    this.Session.Clear();
                }
            }
            Session["Banner"] = "CustomerLogin";
            Session["Login"] = "Login";
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string q = "Select * from CustDetails where Username = '" + txtuname.Text + "' and Password = '" + txtpass.Text + "'";
            SqlDataAdapter da = new SqlDataAdapter(q, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                string cid = ds.Tables[0].Rows[0][0].ToString();
                Session["Cid"] = cid;
                Response.Redirect("NewTransactions.aspx");
            }
        }
    }
}