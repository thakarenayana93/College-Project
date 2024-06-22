using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Anti_Money_Laundering_tracking_System
{
    public partial class NewTransactions : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-O16H131N\SQLEXPRESS;Initial Catalog=master;Integrated Security=True");
        string cid, cname,id,PrevHash;
        int tid;
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Banner"] = "Login";
            Session["Login"] = "Customer";
            Object cIdObj = Session["Cid"];
            if (cIdObj != null) {
                cid = cIdObj.ToString();
            }
            
            if (!IsPostBack)
            {                
                string qu = "Select Access,Cname from CustDetails where Cid='" + cid + "'";
                SqlDataAdapter da = new SqlDataAdapter(qu, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    cname = ds.Tables[0].Rows[0][1].ToString();
                    string access = ds.Tables[0].Rows[0][0].ToString();
                    if(access == "Pending")
                    {
                        msgPending.Visible = true;
                        msgBlock.Visible = false;
                        btnTransfer.Enabled = false;
                        btnCancel.Enabled = false;
                    }
                    else if(access == "Denied")
                    {
                        msgPending.Visible = false;
                        msgBlock.Visible = true;
                        btnTransfer.Enabled = false;
                        btnCancel.Enabled = false;
                    }
                    else
                    {
                        qu = "Select c.Cid,c.Cname,a.Accno,a.CurrBalance from CustDetails c, AccountDetails a where c.Cid = a.Cid and c.Cid = '" + cid + "'";
                        da = new SqlDataAdapter(qu, con);
                        ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            lbname.Text = ds.Tables[0].Rows[0][1].ToString();
                            txtAcntNo.Text = ds.Tables[0].Rows[0][2].ToString();
                            lbcurrbal.Text = ds.Tables[0].Rows[0][3].ToString();
                        }
                        msgPending.Visible = false;
                        msgBlock.Visible = false;
                        btnTransfer.Enabled = true;
                        btnCancel.Enabled = true;

                    }
                }

                qu = "Select Tid from Transactions order by Tid desc";
                da = new SqlDataAdapter(qu, con);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    id = ds.Tables[0].Rows[0][0].ToString();
                    tid = Convert.ToInt32(id);
                    tid++;
                    lbTid.Text = tid.ToString();
                }
                else
                {
                    lbTid.Text = "1";
                }
            }
        }

        protected void btnTransfer_Click(object sender, EventArgs e)
        {
            int balance = 0;

            if (txtAmnt.Text != null)
            {
                try
                {
                    balance = Convert.ToInt32(txtAmnt.Text);
                }
                catch (Exception excp)
                {
                    balance = 0;
                }

            }

            int currBalance = 0;

            if (lbcurrbal.Text != null) {
                try
                {
                    currBalance =  Convert.ToInt32(lbcurrbal.Text);
                }
                catch (Exception excp) {
                    currBalance = 0;
                }
                
            }
        
            if(currBalance >= balance)
            {
                string q = "Select PrevHash,CurrHash from Transactions where Cid='" + cid + "' order by Tid desc";
                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    PrevHash = ds.Tables[0].Rows[0][1].ToString();                   
                }
                else
                {
                    PrevHash = "Genesis";
                }

                string date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                date = date.Replace('-', '/');

                Blocklist bl = new Blocklist();
                bl.tid = lbTid.Text;              
                bl.cid = cid;
                bl.toname = txtReName.Text;
                bl.frmname = lbname.Text;
                bl.toaccno = txtReAccNo.Text;
                bl.frmaccno = txtAcntNo.Text;
                bl.amount = txtAmnt.Text;
                bl.bname = txtBName.Text;
                bl.branch = txtBrName.Text;
                bl.ifsc = txtIfsc.Text;               
                bl.datetime = date;
                bl.previoushash = PrevHash;


                Block block = new Block();
                block.gethash(new string[] { bl.tid, bl.cid, bl.toname, bl.frmname }, new string[] { bl.amount, bl.toaccno, bl.frmaccno, bl.bname, bl.branch, bl.ifsc }, bl.previoushash, bl.datetime);
                string Currentblock = block.getFinalBlock();
                bl.block = Currentblock;

                con.Open();
                string qu = "Insert into Transactions Values('" + cid + "','" + txtAmnt.Text + "','" + txtReName.Text + "','" + txtReAccNo.Text + "','" + lbname.Text + "','" + txtAcntNo.Text + "','" + txtBName.Text + "','" + txtBrName.Text + "','" + txtIfsc.Text + "','" + date + "','" + PrevHash + "','" + Currentblock + "')";
                SqlCommand cmd = new SqlCommand(qu, con);
                cmd.ExecuteNonQuery();                

                currBalance = currBalance - balance;

                qu = "Update AccountDetails set CurrBalance = '" + currBalance + "' where Cid = '" + cid + "'";
                cmd = new SqlCommand(qu, con);
                cmd.ExecuteNonQuery();
                con.Close();
                
                tid = Convert.ToInt32(lbTid.Text);
                tid++;
                lbTid.Text = tid.ToString();
                txtAmnt.Text = "";
                txtReName.Text = "";
                txtReAccNo.Text = "";
                txtBName.Text = "";
                txtBrName.Text = "";
                txtIfsc.Text = "";

                Page.ClientScript.RegisterStartupScript(GetType(), "msgtype", "alert('Fund Transfer Successfully!!!')", true);

            }
            else
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "msgtype", "alert('Your account have Insufficient Balance')", true);
            }     
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

        }

        public class Block
        {
            byte[] Entities;
            byte[] Data;
            byte[] prevhash;
            byte[] Datetime;
            byte[] Finalhashblock = null;

            //entitities
            // user ids - myid,bankid,transactionid

            //data
            //amount
            public byte[] gethash(string[] entities, string[] data, string prevhashblock, string transactiondatetime)
            {
                string finaldata = "";
                foreach (string s in entities)
                {
                    finaldata += s;
                }

                string bnkdata = "";
                foreach (string s1 in data)
                {
                    bnkdata += s1;
                }

                Entities = Encoding.Default.GetBytes(finaldata);
                Data = Encoding.Default.GetBytes(bnkdata);
                prevhash = Encoding.Default.GetBytes(prevhashblock);
                Datetime = Encoding.Default.GetBytes(transactiondatetime);

                using (SHA512 sha = new SHA512Managed())
                using (MemoryStream st = new MemoryStream())
                using (BinaryWriter bw = new BinaryWriter(st))
                {
                    bw.Write(Entities);
                    bw.Write(Data);
                    bw.Write(prevhash);
                    bw.Write(Datetime);
                    var finalblock = st.ToArray();
                    Finalhashblock = sha.ComputeHash(finalblock);
                    return Finalhashblock;
                }
            }

            public string getFinalBlock()
            {
                if (Finalhashblock != null)
                {
                    return BitConverter.ToString(Finalhashblock).Replace("-", "");
                }
                else
                {
                    return "Not Defined";
                }
            }
        }
    }
}