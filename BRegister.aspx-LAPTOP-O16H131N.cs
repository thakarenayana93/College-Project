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
    public partial class BRegister : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-O16H131N\SQLEXPRESS;Initial Catalog=master;Integrated Security=True");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["Login"] = "Login";
                Session["Banner"] = "Login";
                string q = "Select Distinct Bid from BankDetails order by Bid Desc";
                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if(ds.Tables[0].Rows.Count > 0)
                {
                    string id = ds.Tables[0].Rows[0][0].ToString();
                    int bid = Convert.ToInt32(id);
                    bid++;
                    lbBid.Text = bid.ToString();
                }
                else
                {
                    lbBid.Text = "1";
                }
            }
        }

        protected void btnregister_Click(object sender, EventArgs e)
        {
            string q = "Select * from BankDetails where username='" + txtusernm.Text + "'";
            SqlDataAdapter da = new SqlDataAdapter(q, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "msgtype", "alert('This username already exist please try another')", true);
            }
            else
            {
                con.Open();
                string qu = "Insert into BankDetails Values('" + txtname.Text + "','" + txtaddr.Text + "','" + txtcontact.Text + "','" + txtemail.Text + "','" + txtusernm.Text + "','" + txtpass.Text + "','" + txtbranch.Text + "')";
                SqlCommand cmd = new SqlCommand(qu, con);
                cmd.ExecuteNonQuery();
                con.Close();
                Session["Alert"] = "true";
                Session["Login"] = "BankLogin";
                Session["Banner"] = "Login";
                Response.Redirect("BnkLogin.aspx");
            }
        }

        protected void btncancel_Click(object sender, EventArgs e)
        {
            Session["Login"] = "BankLogin";
            Session["Banner"] = "Login";
            Response.Redirect("BnkLogin.aspx");
        }
    }
}