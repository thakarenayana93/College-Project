using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace Anti_Money_Laundering_tracking_System
{
    public partial class ViewCustomer : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-O16H131N\SQLEXPRESS;Initial Catalog=master;Integrated Security=True");
        string oid;
        protected void Page_Load(object sender, EventArgs e)
        {
            oid = Session["Oid"].ToString(); 
            if (!IsPostBack)
            {
                Gvbind();
            }
        }

        protected void Gvbind()
        {
            string q = "Select distinct c.Cid, c.Cname, c.DOB, c.Caddress, c.Ccontact, c.Cemail, c.Access, a.bid from CustDetails c, OfficerDetails o, AccountDetails a where c.Cid=a.Cid and a.Bid = o.Bid and oid = '" + oid+"'";
            SqlDataAdapter da = new SqlDataAdapter(q, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            int c = ds.Tables[0].Rows.Count;
            if (c > 0)
            {
                GridView1.DataSource = ds;
                GridView1.DataBind();
                lbnodata.Visible = false;
            }
        }

        protected void lbtnsearch_Click(object sender, EventArgs e)
        {
            if (txtsearch.Text == "")
            {
                Gvbind();
            }
            else
            {
                int outputval = 0;
                bool isNumber = int.TryParse(txtsearch.Text, out outputval);
                if (isNumber)
                {
                    con.Open();
                    string q = "Select distinct c.Cid, c.Cname, c.DOB, c.Caddress, c.Ccontact, c.Cemail, c.Access, a.bid from CustDetails c, OfficerDetails o, AccountDetails a where a.Bid = o.Bid and oid = '" + oid + "' and Cid='" + txtsearch.Text + "'";
                    SqlDataAdapter da = new SqlDataAdapter(q, con);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        GridView1.DataSource = ds;
                        GridView1.DataBind();
                        lbnodata.Visible = false;
                    }
                    else
                    {
                        lbnodata.Visible = true;
                    }
                }
                else
                {
                    string q = "Select distinct c.Cid, c.Cname, c.DOB, c.Caddress, c.Ccontact, c.Cemail, c.Access, a.bid from CustDetails c, OfficerDetails o, AccountDetails a where c.Cid=a.Cid and a.Bid = o.Bid and oid = '" + oid + "' and   Cname='" + txtsearch.Text + "'";
                    SqlDataAdapter da = new SqlDataAdapter(q, con);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        GridView1.DataSource = ds;
                        GridView1.DataBind();
                        lbnodata.Visible = false;
                    }
                    else
                    {
                        lbnodata.Visible = true;
                    }
                }
                con.Close();
            }
        }

        protected void lbtnaccDet_Click(object sender, EventArgs e)
        {
            LinkButton lbtn = sender as LinkButton;
            GridViewRow row = lbtn.NamingContainer as GridViewRow;
            string cid = row.Cells[0].Text;
            con.Open();
            string q = "Select * from AccountDetails where Cid='" + cid + "'";
            SqlDataAdapter da = new SqlDataAdapter(q, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                lbcid.Text = cid;
                lbname.Text = ds.Tables[0].Rows[0][3].ToString();
                lbacctype.Text = ds.Tables[0].Rows[0][4].ToString();
                lbacccno.Text = ds.Tables[0].Rows[0][5].ToString();
                lbcurrbal.Text = ds.Tables[0].Rows[0][6].ToString();
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPopup", "$('#myModal').modal({backdrop: 'static', keyboard: false},'show')", true);
        }
    }
}