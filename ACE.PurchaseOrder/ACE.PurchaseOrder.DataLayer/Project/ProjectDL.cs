using System;
using System.Data;
using System.Data.Common;
using ACE.PurchaseOrder.CommonLayer;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ACE.PurchaseOrder.DataLayer
{
  public class ProjectDL : CommonForAllDL
  {
    #region Properties

    public int ProjectID { get; set; }

    public int EmployeeID { get; set; }

    public int CompanyID { get; set; }

    public string ProjectName { get; set; }

    public int ClientID { get; set; }

    public string ClientName { get; set; }

    public string Domain { get; set; }

    public string Technology { get; set; }

    public int ProjectPriorityID { get; set; }

    public string ProjectPriorityDescription { get; set; }

    public int ProjectStatusID { get; set; }

    public string ProjectStatusDescription { get; set; }

    public Nullable<DateTime> StartDate { get; set; }

    public Nullable<DateTime> EndDate { get; set; }

    public Decimal Budget { get; set; }

    public int BudgetInDays { get; set; }

    public Nullable<DateTime> ProjectSignDate { get; set; }

    public string ProjectDescription { get; set; }

    public int ProjectManagerID { get; set; }

    public string ProjectManager { get; set; }

    public string IsValid { get; set; }

    public int AuditUserID { get; set; }

    #endregion

    #region Constructors

    public ProjectDL()
    {
    }

    public ProjectDL(int projectID, int companyID, bool getAllProperties)
    {
      ProjectID = projectID;
      CompanyID = companyID;
      if (getAllProperties)
      {
        GetProject();
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Decides whether to Call Add/Edit/Delete method
    /// And Calls the appropriate method
    /// </summary>
    /// <returns>TransactionResult - Success / Failure</returns>
    public TransactionResult Commit()
    {
      TransactionResult result = null;
      Database db = DatabaseFactory.CreateDatabase(_myConnection.DatabaseName);
      using (DbConnection connection = db.CreateConnection())
      {
        connection.Open();
        DbTransaction transaction = connection.BeginTransaction();
        try
        {
          switch (ScreenMode)
          {
            case ScreenMode.Add:
              //Adding Or Editing Project
              result = AddEditProject(db, transaction);
              if (result.Status == TransactionStatus.Failure)
              {
                transaction.Rollback();
                return result;
              }
              break;
            case ScreenMode.Edit:
              break;
            case ScreenMode.Delete:
              result = DeleteProject(db, transaction);
              if (result.Status == TransactionStatus.Failure)
              {
                transaction.Rollback();
                return result;
              }
              break;
            case ScreenMode.View:
              break;
          }
          transaction.Commit();
          return result;
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          if (ScreenMode == ScreenMode.Add)
          {
            ErrorLog.LogErrorMessageToDB("", "ProjectDL.cs", "Commit For Add", ex.Message, _myConnection);
            System.Web.HttpContext.Current.Response.Redirect("~/OrderErrorPage.aspx");
            return new TransactionResult(TransactionStatus.Failure, "Failure Adding");
          }
          if (ScreenMode == ScreenMode.Edit)
          {
            ErrorLog.LogErrorMessageToDB("", "ProjectDL.cs", "Commit For Edit", ex.Message, _myConnection);
            System.Web.HttpContext.Current.Response.Redirect("~/OrderErrorPage.aspx");
            return new TransactionResult(TransactionStatus.Failure, "Failure Updating");
          }
          if (ScreenMode == ScreenMode.Delete)
          {
            ErrorLog.LogErrorMessageToDB("", "ProjectDL.cs", "Commit For Delete", ex.Message, _myConnection);
            System.Web.HttpContext.Current.Response.Redirect("~/OrderErrorPage.aspx");
            return new TransactionResult(TransactionStatus.Failure, "Failure Deleting");
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Get Project Getting in Dropdown List Control - Added new sp name on 23/06/2014
    /// </summary>
    /// <param name="ACEConnection">Instance of the ACEConnection Class</param>
    /// <returns>Return the data as Dataset</returns>
    public DataSet GetProjectList()
    {
      DataSet ds = new DataSet();
      try
      {
        Database db = DatabaseFactory.CreateDatabase(_myConnection.DatabaseName);
        string sqlCommand = "ProjectList_SELECT";//spGetProjectList
        DbCommand dbCommand = db.GetStoredProcCommand(sqlCommand);
        db.AddInParameter(dbCommand, "EmployeeID", DbType.Int32, EmployeeID);
        ds = db.ExecuteDataSet(dbCommand);
      }
      catch (Exception ex)
      {
        ErrorLog.LogErrorMessageToDB("", "ProjectDL.cs", "GetProjectList", ex.Message, _myConnection);
        System.Web.HttpContext.Current.Response.Redirect("~/OrderErrorPage.aspx");
      }
      return ds;
    }

    /// <summary>
    /// Gets the list of Projects Displaying the data in the grid, with Search Functionality - Added new sp name on 23/06/2014
    /// </summary>
    /// <param name="companyID"></param>
    /// <param name="projectID"></param>
    /// <param name="searchText"></param>
    /// <param name="projectManagerID"></param>
    /// <param name="isAdministrator"></param>
    /// <returns>DataSet Containing the List of Employees By CompanyID</returns>
    public DataSet GetProjectDetails( int projectID, string searchText,string isValid)
    {
      DataSet ds = new DataSet();
      try
      {
        Database db = DatabaseFactory.CreateDatabase(_myConnection.DatabaseName);
        DbCommand dbCommand = db.GetStoredProcCommand("Project_SELECT");//spGetProject
        dbCommand.Parameters.Clear();
        dbCommand.CommandTimeout = 300;
       
        db.AddInParameter(dbCommand, "ProjectID", DbType.Int32, projectID);
        db.AddInParameter(dbCommand, "SearchText", DbType.String, searchText);
        db.AddInParameter(dbCommand, "IsValid", DbType.String, isValid);
        ds = db.ExecuteDataSet(dbCommand);
      }
      catch (Exception ex)
      {
        ErrorLog.LogErrorMessageToDB("", "ProjectDL.cs", "GetProjectDetails", ex.Message.ToString(), _myConnection);
        System.Web.HttpContext.Current.Response.Redirect("~/OrderErrorPage.aspx");
      }
      return ds;
    }

    /// <summary>
    /// - Added new sp name on 23/06/2014
    /// </summary>
    /// <param name="resourceID"></param>
    /// <returns></returns>
    public DataSet GetProjectsByResourceID(int resourceID)
    {
      DataSet ds = new DataSet();
      try
      {
        Database db = DatabaseFactory.CreateDatabase(_myConnection.DatabaseName);
        string sqlCommand = "ProjectsByResourceID_SELECT";//spGetProjectsByResourceID
        DbCommand dbCommand = db.GetStoredProcCommand(sqlCommand);
        dbCommand.Parameters.Clear();
        dbCommand.CommandTimeout = 300;
        db.AddInParameter(dbCommand, "ResourceID", DbType.Int32, resourceID);
        ds = db.ExecuteDataSet(dbCommand);
      }
      catch (Exception ex)
      {
        ErrorLog.LogErrorMessageToDB("", "ProjectDL.cs", "spGetProjectsByResourceID", ex.Message, _myConnection);
        System.Web.HttpContext.Current.Response.Redirect("~/OrderErrorPage.aspx");
      }
      return ds;
    }

    /// <summary>
    /// Get Project - Added new sp name on 23/06/2014
    /// </summary>
    private void GetProject()
    {
      try
      {
        Database db = DatabaseFactory.CreateDatabase(_myConnection.DatabaseName);
        string sqlCommand = "ProjectByCompanyID_SELECT";//spGetProjectByCompanyID
        DbCommand dbCommand = db.GetStoredProcCommand(sqlCommand);
        db.AddInParameter(dbCommand, "ProjectID", DbType.String, ProjectID);
        db.AddInParameter(dbCommand, "CompanyID", DbType.String, CompanyID);
        DataSet ds = db.ExecuteDataSet(dbCommand);

        // Load Employee Info
        if (ds.Tables[0].Rows.Count > 0)
        {
          DataRow dRow = ds.Tables[0].Rows[0];
          ProjectID = Convert.ToInt32(dRow["ProjectID"]);
          ProjectName = Convert.ToString(dRow["ProjectName"]);
          ClientID = Convert.ToInt32(dRow["ClientID"]);
          ClientName = Convert.ToString(dRow["ClientName"]);
          Domain = Common.CheckNull(Convert.ToString(dRow["Domain"]));
          if (dRow["ProjectPriorityID"] != DBNull.Value)
            ProjectPriorityID = Convert.ToInt32(dRow["ProjectPriorityID"]);

          if (dRow["ProjectStatusID"] != DBNull.Value)
            ProjectStatusID = Convert.ToInt32(dRow["ProjectStatusID"]);

          if (dRow["StartDate"] != DBNull.Value)
            StartDate = Convert.ToDateTime(dRow["StartDate"]);
          else
            StartDate = null;

          if (dRow["EndDate"] != DBNull.Value)
            EndDate = Convert.ToDateTime(dRow["EndDate"]);
          else
            EndDate = null;

          if (dRow["Budget"] != DBNull.Value)
            Budget = Convert.ToDecimal(dRow["Budget"]);

          if (dRow["BudgetInDays"] != DBNull.Value)
            BudgetInDays = Convert.ToInt32(dRow["BudgetInDays"]);

          if (dRow["ProjectSignDate"] != DBNull.Value)
            ProjectSignDate = Convert.ToDateTime(dRow["ProjectSignDate"]);
          else
            ProjectSignDate = null;

          if (dRow["ProjectDescription"] != DBNull.Value)
            ProjectDescription = Common.CheckNull(Convert.ToString(dRow["ProjectDescription"]));

          if (dRow["ProjectManagerID"] != DBNull.Value)
            ProjectManagerID = Convert.ToInt32(dRow["ProjectManagerID"]);

          // IsValid = dRow["IsValid"];
        }
      }
      catch (Exception ex)
      {
        ErrorLog.LogErrorMessageToDB("", "ProjectDL.cs", "GetProject", ex.Message.ToString(), _myConnection);
        System.Web.HttpContext.Current.Response.Redirect("~/OrderErrorPage.aspx");
      }
    }

    /// <summary>
    /// To Add / Edit a Project - Added new sp name on 23/06/2014
    /// </summary>
    /// <param name="db"></param>
    /// <param name="transaction"></param>
    /// <returns>TransactionResult - Success / Failure</returns>
    internal TransactionResult AddEditProject(Database db, DbTransaction transaction)
    {
      int returnValue = 0;
      string sqlCommand = "Project_MERGE";//spAddEditProject
      DbCommand dbCommand = db.GetStoredProcCommand(sqlCommand);

      db.AddInParameter(dbCommand, "ProjectID", DbType.Int32, ProjectID);
      db.AddInParameter(dbCommand, "CompanyID", DbType.Int32, CompanyID);
      db.AddInParameter(dbCommand, "ProjectName", DbType.String, ProjectName);
      db.AddInParameter(dbCommand, "ClientID", DbType.Int32, ClientID);
      db.AddInParameter(dbCommand, "Domain", DbType.String, Domain);
      if (ProjectPriorityID != 0)
        db.AddInParameter(dbCommand, "ProjectPriorityID", DbType.Int32, ProjectPriorityID);
      if (ProjectStatusID != 0)
        db.AddInParameter(dbCommand, "ProjectStatusID", DbType.Int32, ProjectStatusID);
      if (StartDate != null)
        db.AddInParameter(dbCommand, "StartDate", DbType.DateTime, StartDate);
      if (EndDate != null)
        db.AddInParameter(dbCommand, "EndDate", DbType.DateTime, EndDate);

      db.AddInParameter(dbCommand, "Budget", DbType.Decimal, Budget);
      db.AddInParameter(dbCommand, "BudgetInDays", DbType.Int32, BudgetInDays);

      if (ProjectSignDate != null)
        db.AddInParameter(dbCommand, "ProjectSignDate", DbType.DateTime, ProjectSignDate);

      db.AddInParameter(dbCommand, "ProjectDescription", DbType.String, ProjectDescription);
      if (ProjectManagerID != 0)
        db.AddInParameter(dbCommand, "ProjectManagerID", DbType.Int32, ProjectManagerID);
      db.AddInParameter(dbCommand, "IsValid", DbType.String, IsValid);

      db.AddInParameter(dbCommand, "AddEditOption", DbType.Int16, AddEditOption);

      db.AddParameter(dbCommand, "Return Value", DbType.Int32, ParameterDirection.ReturnValue, "Return Value",
                      DataRowVersion.Default, returnValue);

      db.ExecuteNonQuery(dbCommand, transaction);
      returnValue = (Int32)db.GetParameterValue(dbCommand, "Return Value");

      ProjectID = returnValue;

      if (returnValue == -1)
      {
        if (AddEditOption == 1)
          return new TransactionResult(TransactionStatus.Failure, "Failure Updating");
        else
          return new TransactionResult(TransactionStatus.Failure, "Failure Adding");
      }
      if (returnValue == -99)
      {
        if (AddEditOption == 1)
          return new TransactionResult(TransactionStatus.Failure, "Failed Updating - Duplicate Client Name");
        else
          return new TransactionResult(TransactionStatus.Failure, "Failed Adding - Duplicate Client Name");
      }
      else
      {
        if (AddEditOption == 1)
        {
          return new TransactionResult(TransactionStatus.Success, "Successfully Updated");
        }
        else
        {
          return new TransactionResult(TransactionStatus.Success, "Successfully Added");
        }
      }
    }

    /// <summary>
    /// To Delete a Project - Added new sp name on 23/06/2014
    /// </summary>
    /// <param name="db"></param>
    /// <param name="transaction"></param>
    /// <returns>TransactionResult - Success / Failure</returns>
    private TransactionResult DeleteProject(Database db, DbTransaction transaction)
    {
      int returnValue = 0;
      DbCommand dbCommand = db.GetStoredProcCommand("Project_MERGE");//spDeleteProject
      db.AddInParameter(dbCommand, "ProjectID", DbType.Int32, ProjectID); //Added on 23/06/2014
      db.AddInParameter(dbCommand, "AddEditOption", DbType.Int16, AddEditOption); 
      db.AddParameter(dbCommand, "Return Value", DbType.Int32, ParameterDirection.ReturnValue, "Return Value",
                      DataRowVersion.Default, returnValue);

      db.ExecuteNonQuery(dbCommand, transaction);
      returnValue = (Int32)db.GetParameterValue(dbCommand, "Return Value");

      if (returnValue == -1)
        return new TransactionResult(TransactionStatus.Failure, "Failure Deleting");
      if (returnValue == 1)
        return new TransactionResult(TransactionStatus.Failure, "Failure Deleting!This Project is associated with Employee profile");

      else
        return new TransactionResult(TransactionStatus.Success, "Successfully Deleted");
    }

    #endregion
  }
}
