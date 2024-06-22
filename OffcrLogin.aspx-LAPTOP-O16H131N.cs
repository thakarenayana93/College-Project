using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

namespace Anti_Money_Laundering_tracking_System
{
    public partial class OffcrLogin : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-O16H131N\SQLEXPRESS;Initial Catalog=master;Integrated Security=True");

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Banner"] = "OfficerLogin";
            Session["Login"] = "Login";
        }       

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string q = "Select * from OfficerDetails where Username = '" + txtuname.Text + "' and Password = '" + txtpass.Text + "'";
            SqlDataAdapter da = new SqlDataAdapter(q, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Session["Oid"] = ds.Tables[0].Rows[0][0].ToString();
                Session["Banner"] = "Login";
                Session["Login"] = "Officer";
                Response.Redirect("BankDetails.aspx");
            }
        }
    }
}