using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WebPages_Event : System.Web.UI.Page
{
    DataManager dm = new DataManager();
    SqlConnection objsql = new SqlConnection(ConfigurationManager.AppSettings["ConnStr"].ToString());
    int FlagPhoto = 0;
    int FlagAadhar = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindEvent();
            BindEvents();
            divEventAdd.Visible = true;
            divViewEvent.Visible = false;
            divViewEvent.Visible = false;
        }
    }
    public void ShowMessage(string msg)
    {
        StringBuilder bd = new StringBuilder();
        bd.Append(@"<script type='txt/javascript'>");
        bd.Append("Alert('" + msg + "');");
        bd.Append(@"</script>");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message", bd.ToString(), false);
    }

    private void ShowMessageAndRedirect(string msg, string url)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("function Redirect()");
            sb.Append("{");
            sb.Append(" window.location.href='" + url + "'");
            sb.Append("}");
            sb.Append("alert('" + msg + "');");
            sb.Append("setTimeout('Redirect()',1000);");
            Response.Write("<script>" + sb + "</script>");
        }
        catch (Exception ex)
        {
            ex.Data.Clear();
            //Response.Redirect("oops.aspx");
        }
    }
    private void BindEvent()
    {
        try
        {

            gvevent.DataSource = null;
            gvevent.DataBind();
            string count = "";

            if (ddlShowTop.SelectedIndex > 0)
            {
                int top = int.Parse(ddlShowTop.SelectedItem.ToString());
                count += " top(" + top + ") ";
            }

            string Qry = "SELECT   * FROM dbo.tbl_Events WHERE Event_ActiveFlag=1";
            if (txttitle.Text != "")
                Qry += " AND Event_Title LIKE '%" + txttitle.Text + "%'";
            if (txtdate.Text != "")
                Qry += " AND Event_Date LIKE '%" + txtdate.Text + "%'";
            if (txtattach.Text != "")
                Qry += " AND Event_AttachmentTitle LIKE '%" + txtattach.Text + "%'";
            if (txtattachment.Text != "")
                Qry += " AND Event_Attachment LIKE '%" + txtattachment.Text + "%'";
            if (editor3.Content != "")
                Qry += " AND Event_Description LIKE '%" + editor3.Content + "%'";
            if (ddlDepartment.SelectedValue != "--Select--")
                Qry += " AND Event_Department LIKE '%" + ddlDepartment.SelectedValue + "%'";

            Qry += " order by Event_MID desc";
            Qry += " Select count(Event_MID) TotalRecords from tbl_Events where Event_ActiveFlag=1";
            DataSet ds = dm.GetDataSet(Qry);
            if (ds != null)
            {
                gvevent.DataSource = ds.Tables[0];
                gvevent.DataBind();
            }
        }
        catch (Exception ex)
        {
            ex.Data.Clear();
        }
    }
    void BindEvents()
    {
        try
        {

            gvevent.DataSource = null;
            gvevent.DataBind();
            string count = "";


            string Qry = "SELECT   * FROM tbl_Events inner join tbl_Events on Reg_MID=Event_MID WHERE Event_ActiveFlag=1";
            //if (TxtTitle1.Text != "")
            //    Qry += " AND Achievement_Title LIKE '%" + TxtTitle1.Text + "%'";


            Qry += " order by Event_MID desc";
            Qry += " Select count(Event_MID) TotalRecords from tbl_Events where Event_ActiveFlag=1";
            DataSet ds = dm.GetDataSet(Qry);
            if (ds != null)
            {
                gvevent.DataSource = ds.Tables[0];
                gvevent.DataBind();
            }
        }
        catch (Exception ex)
        {
            ex.Data.Clear();
        }
    }

    private void ClearAll()
    {
        Response.Redirect("../webpages/Event.aspx");
    }

    protected void btnview_Click(object sender, EventArgs e)
    {
        Response.Redirect("../webpages/Event.aspx");
    }
    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            string Department = ddlDepartment.Text;
            string Date = txtdate.Text;
            string Attach = txtattach.Text;
            string Attachment = txtattachment.Text;
            string Title = txttitle.Text;
            string Photo1 = "";
            string Photo2 = "";
            string Photo3 = "";
            string Photo4 = "";
            string Photo5 = "";
            string Description = editor3.Content;
            int Mid = int.Parse(Session["Event_MID"].ToString());




            if (btnsave.Text == "Save")
            {
                if (fupImage.HasFile)
                {
                    UploadImage();
                    if (Session["ImagePath"] != null)
                    {
                        Photo1 = Session["ImagePath"].ToString();
                    }
                    else
                    {
                        Photo1 = PreviewImage.Src;
                    }
                }
                string qry = "Insert into tbl_Events(Event_MID,Event_Department,Event_Title,Event_Date,Event_AttachmentTitle,Event_Attachment)values('" + Mid + "','" + Title + "','" + Description + "','" + Photo + "')";
                objsql.Open();
                SqlCommand cmd = new SqlCommand(qry, objsql);
                int res = cmd.ExecuteNonQuery();
                objsql.Close();
                if (res > 0)
                {
                    ShowMessageAndRedirect("Submitted Successfully", "Event.aspx");
                }
                else
                {
                    ShowMessageAndRedirect("Sorry Try Again!", "Event.aspx");
                }
            }
            else
            {
                string qry1 = "update tbl_Events set Event_Department='" + Department + "',Event_Title='" + Title + "',Event_Date='" + Photo + "',Event_AttachmentTitle='" + Photo + "',Event_Attachment='" + Photo + "'";
                objsql.Open();
                SqlCommand cmd = new SqlCommand(qry1, objsql);
                int res = cmd.ExecuteNonQuery();
                objsql.Close();
                if (res > 0)
                {
                    ShowMessageAndRedirect("Submitted Successfully", "StudentMaster.aspx");
                    divEventAdd.Visible = false;
                    divViewEvent.Visible = true;
                }
                else
                {
                    ShowMessageAndRedirect("Sorry Try Again!", "StudentMaster.aspx");
                    divEventAdd.Visible = false;
                    divViewEvent.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            ex.Data.Clear();
        } 

    }
    protected void btncancel_Click(object sender, EventArgs e)
    {

    }
    protected void btnadd_Click(object sender, EventArgs e)
    {

            BindReg();
            divAchievementview.Visible = false;
            divAchievement.Visible = false;
            divAchieventsGrid.Visible = true;
            divAddStudent.Visible = false;
        
    }
    protected void btnsearch_Click(object sender, EventArgs e)
    {
        BindEvent();
    }
    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        ClearAll();
    }
}
 protected void gvAchievementview_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "cmdEdit")
        {
            int RegId = int.Parse(e.CommandArgument.ToString());

            string qry = "select * from dbo.tbl_Registration where  Reg_ActiveFlag=1  and Reg_MID='" + RegId + "'";
            DataSet ds = dm.GetDataSet(qry);
            if (ds != null)
            {
                Session["Reg_MID"] = RegId.ToString();


                GetRegDetails(RegId);





            }
        }
        else if (e.CommandName == "Del")
        {
            try
            {

                int RegId = int.Parse(e.CommandArgument.ToString());
                int UserId = 0;// int.Parse(Session["UserId"].ToString());
                int Result = 0;


                string qry1 = "update table tbl_Registration set Reg_ActiveFlag=0 where Reg_MID='" + RegId + "'";
                objsql.Open();
                SqlCommand cmd = new SqlCommand(qry1, objsql);
                int res = cmd.ExecuteNonQuery();
                objsql.Close();
                if (res > 0)
                {
                    ShowMessageAndRedirect("Submitted Successfully", "StudentMaster.aspx");
                    ClearAll();
                    divAchievement.Visible = false;
                    divAchievementview.Visible = true;
                }
                else
                {
                    ShowMessageAndRedirect("Sorry Try Again!", "StudentMaster.aspx");
                    divAchievement.Visible = false;
                    divAchievementview.Visible = true;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
                Response.Redirect("~/oops.aspx");
            }

        }


    }

    private void GetRegDetails(int Mid)
    {
        string Query = "SELECT   * FROM dbo.tbl_Registration WHERE Reg_ActiveFlag=1 and Reg_MID=" + Mid + "";
        DataSet ds = dm.GetDataSet(Query);
        if (ds.Tables[0].Rows.Count > 0)
        {
            ddlDepartment.SelectedValue = ds.Tables[0].Rows[0]["Reg_Department"].ToString();
            ddlDepartment.Enabled = false;
            TxtsName.Text = ds.Tables[0].Rows[0]["Reg_Name"].ToString();
            TxtSRegNo.Text = ds.Tables[0].Rows[0]["Reg_RegNo"].ToString();


            //editor3.Content = ds.Tables[0].Rows[0]["Achievement_Description"].ToString();
            //PreviewImage.Src = ds.Tables[0].Rows[0]["Achievement_Photo"].ToString().Replace("~", "..");




            divAchieventsGrid.Visible = false;
            divAchievement.Visible = true;
            divAchievementview.Visible = false;
        }
    }

    private void UploadImage()
    {
        try
        {
            string filename = System.IO.Path.GetFileName(fupImage.PostedFile.FileName);
            Random r = new Random();
            int randomno = r.Next();

            string sImageFileExtension = filename.Substring(filename.LastIndexOf(".")).ToLower();
            string flnm = Path.GetFileNameWithoutExtension(filename);
            if (sImageFileExtension == ".jpg" || sImageFileExtension == ".jpeg" || sImageFileExtension == ".png")
            {
                string newfilename = randomno + "_" + flnm + sImageFileExtension;
                string strFPath = "~/RegPhoto/" + newfilename;
                fupImage.PostedFile.SaveAs(Server.MapPath(strFPath));
                Session["ImagePath"] = strFPath;
                CompressAndUploadPhoto(fupImage);
            }
        }
        catch (Exception ex)
        {
            ex.Data.Clear();
            Response.Redirect("~/oops.aspx");
        }
    }
    private void CompressAndUploadPhoto(FileUpload fupImage)
    {
        try
        {
            // First we check to see if the user has selected a file
            if (fupImage.HasFile)
            {
                // Find the fileUpload control
                string filename = fupImage.FileName;

                // Specify the upload directory
                string directory = Server.MapPath("~/RegPhoto/");

                // Create a bitmap of the content of the fileUpload control in memory
                Bitmap originalBMP = new Bitmap(fupImage.FileContent);

                int newWidth = 150;
                int newHeight = newWidth;

                double actualHeight = originalBMP.Height;
                double actualWidth = originalBMP.Width;
                double maxHeight = 600.0;
                double maxWidth = 800.0;
                double imgRatio = actualWidth / actualHeight;
                double maxRatio = maxWidth / maxHeight;
                double compressionQuality = 0.5;//50 percent compression
                if (actualHeight > maxHeight || actualWidth > maxWidth)
                {
                    if (imgRatio < maxRatio)
                    {
                        //adjust width according to maxHeight
                        imgRatio = maxHeight / actualHeight;
                        actualWidth = imgRatio * actualWidth;
                        actualHeight = maxHeight;
                    }
                    else if (imgRatio > maxRatio)
                    {
                        //adjust height according to maxWidth
                        imgRatio = maxWidth / actualWidth;
                        actualHeight = imgRatio * actualHeight;
                        actualWidth = maxWidth;
                    }
                    else
                    {
                        actualHeight = maxHeight;
                        actualWidth = maxWidth;
                    }
                }

                newWidth = Convert.ToInt16(actualWidth);
                newHeight = Convert.ToInt16(actualHeight);
                // Create a new bitmap which will hold the previous resized bitmap
                Bitmap newBMP = new Bitmap(originalBMP, newWidth, newHeight);
                // Create a graphic based on the new bitmap
                Graphics oGraphics = Graphics.FromImage(newBMP);

                // Set the properties for the new graphic file
                oGraphics.SmoothingMode = SmoothingMode.AntiAlias; oGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Draw the new graphic based on the resized bitmap
                oGraphics.DrawImage(originalBMP, 0, 0, newWidth, newHeight);

                // Save the new graphic file to the server              
                Random rm = new Random();
                string randomno = rm.Next(11111, 99999).ToString();
                string sImageFileExtension = filename.Substring(filename.LastIndexOf(".")).ToLower();
                string newfilename = randomno + sImageFileExtension;
                string strPath = directory + "kss_" + randomno + sImageFileExtension;
                newBMP.Save(strPath);
                string strFPath = "~/RegPhoto/" + "kss_" + randomno + sImageFileExtension;
                Session["MYPHOTO_Reg"] = strFPath;
                originalBMP.Dispose();
                newBMP.Dispose();
                oGraphics.Dispose();
            }
        }
        catch (Exception ex)
        {
            ex.Data.Clear();
            Response.Redirect("~/oops.aspx");
        }
}